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

            _snake = new Snake(2, 3, 10);

            _fruits = new List<Fruit>();

            _fruits.Add(new Fruit(4, 5));
            _rand = new Random(23);
        }

        public int GetScore()
        {
            return _snake.GetLength();
        }

        public int GetLife()
        {
            return _snake.Life;
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
}
