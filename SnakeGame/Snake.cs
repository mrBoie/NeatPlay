using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeGame
{
    public class Snake
    {
        private List<(int X, int Y)> _snake;

        public const char SnakeHead = 'H';

        public const char SnakeBody = 'B';
        private readonly int maxLife;
        private int _snakeLenght = 1;

        public int Life { get; private set; }

        public Snake(int x, int y, int maxLife)
        {
            _snake = new List<(int X, int Y)>();

            _snake.Add((x, y));
            this.maxLife = maxLife;
            Life = maxLife;
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
            if (newBox.Symbol == Fruit.Symbol)
            {
                _snakeLenght += 1;
                Life = maxLife;
            }


            for(int i = 0; i < _snake.Count -1 ; i++)
            {
                if (_snake[i].X == newBox.X && _snake[i].Y == newBox.Y) throw new Exception("ATE YOURSELF");
            }

            _snake.Insert(0, (newBox.X, newBox.Y));
            

            if(_snake.Count > _snakeLenght)
            {
                _snake.RemoveAt(_snake.Count() - 1);
            }
            Life--;

            if (Life == 0) throw new Exception("STARVED");
        }
    }
}
