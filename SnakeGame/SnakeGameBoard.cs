using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeGame
{
    public class SnakeGameBoard
    {
        private int _x;
        private int _y;
        private Random _rand;

        private List<Fruit> _fruits;
        private Snake _snake;

        public SnakeGameBoard(int x, int y)
        {
            _x = x;
            _y = y;

            _snake = new Snake(2, 3);

            _fruits = new List<Fruit>();

            _fruits.Add(new Fruit(4, 5));
            _rand = new Random(23);
        }

        public int GetScore()
        {
            return _snake.GetLength();
        }

        public void Update(Direction direction)
        {
            var game = GetGameBoard();
            var snakePosition = _snake.GetSnake();
            var snakeHead = (snakePosition[0].X, snakePosition[0].Y);

            (int X, int Y) newPosition;
            switch (direction)
            {
                case Direction.Up:
                    newPosition = (snakeHead.X, snakeHead.Y - 1);
                    break;
                case Direction.Down:
                    newPosition = (snakeHead.X, snakeHead.Y + 1);
                    break;
                case Direction.Left:
                    newPosition = (snakeHead.X - 1, snakeHead.Y);
                    break;
                case Direction.Right:
                    newPosition = (snakeHead.X + 1, snakeHead.Y);
                    break;
                default:
                    newPosition = snakeHead;
                    break;
            }

            if(game[newPosition.X, newPosition.Y] == Fruit.Symbol)
            {
                GenerateNewFruit(game, newPosition);
            }


            _snake.MoveSnake((newPosition.X, newPosition.Y, game[newPosition.X, newPosition.Y]));
        }

        private void GenerateNewFruit(char[,] game, (int X, int Y) newPosition)
        {
            _fruits.Remove(_fruits.First(f => f.X == newPosition.X && f.Y == newPosition.Y));
            var validPositions = new List<(int X, int Y)>();
            for (int y = 0; y < game.GetLength(0); y++)
            {
                for (int x = 0; x < game.GetLength(1); x++)
                {
                    switch (game[x, y])
                    {
                        case Snake.SnakeHead:
                        case Snake.SnakeBody:
                        case Fruit.Symbol:
                            break;
                        default:
                            validPositions.Add((X: x, Y: y));
                            break;
                    }
                }
            }
            var newFruitPosition = validPositions[_rand.Next(validPositions.Count)];
            _fruits.Add(new Fruit(newFruitPosition));
        }

        public char[,] GetGameBoard()
        {
            var game = new char[_x, _y];

            foreach(var fruit in _fruits)
            {
                game[fruit.X, fruit.Y] = Fruit.Symbol;
            }


            var snake = _snake.GetSnake();
            foreach(var part in snake)
            {
                game[part.X, part.Y] = part.Symbol;
            }

            return game;
        }
    }

    public class Fruit
    {
        public const char Symbol = 'f';

        public int X { get; }

        public int Y { get; }

        public Fruit(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Fruit((int X, int Y) newFruit) : this(newFruit.X, newFruit.Y) { }
    }

    public class Snake
    {
        private List<(int X, int Y)> _snake;

        public const char SnakeHead = 'H';

        public const char SnakeBody = 'B';

        private int _snakeLenght = 1;

        public Snake(int x, int y)
        {
            _snake = new List<(int X, int Y)>();

            _snake.Add((x, y));
        }

        public int GetLength() => _snake.Count();

        public (int X,int Y, char Symbol)[] GetSnake()
        {
            return _snake.Select((s,i) =>
            {
                return i == 0 ? (s.X, s.Y, SnakeHead) : (s.X, s.Y, SnakeBody);
            }).ToArray();
        }

        public void MoveSnake((int X, int Y, char Symbol) newBox)
        {
            if (newBox.Symbol == Fruit.Symbol) _snakeLenght += 1;

            for(int i = 0; i < _snake.Count -1 ; i++)
            {
                if (_snake[i].X == newBox.X && _snake[i].Y == newBox.Y) throw new Exception();
            }

            _snake.Insert(0, (newBox.X, newBox.Y));
            

            if(_snake.Count > _snakeLenght)
            {
                _snake.RemoveAt(_snake.Count() - 1);
            }
        }
    }

    public enum Direction
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }
}
