using SnakeGame;
using System;

namespace Snake.ConsoleView
{
    class Program
    {
        static void Main(string[] args)
        {
            var gameBoard = new SnakeGameBoard(10, 10);

            var game = gameBoard.GetGameBoard();
            //DrawBoard(game, gameBoard.GetScore(), gameBoard.GetLife()); ;

            try
            {
                var input = 's';
                do
                {

                    DrawBoard(gameBoard.GetGameBoard(), gameBoard.GetScore(), gameBoard.GetLife());
                    input = Console.ReadKey().KeyChar;
                    if (input == 'q') break;
                    var direction = GetDirection(input);
                    gameBoard.Update(direction);

                } while (input != 'q');
            }
            catch(Exception e)
            {
                Console.WriteLine("You Lost");
            }


            Console.ReadLine();
        }

        public static void DrawBoard(char [,] board, int score, int life)
        {
            Console.Clear();
            var output = "";
            for (int i = 0; i < board.GetLength(0); i++)
            {
                output += "|";
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    output += $" {board[j, i]} |";
                }
                output += "\n";
            }

            Console.WriteLine(output);

            Console.WriteLine();

            Console.WriteLine($"The score is: {score}");
            Console.WriteLine($"The life is: {life}");
        }

        public static Direction GetDirection(char input)
        {
            switch (input)
            {
                case 'w': return Direction.Up;
                case 's': return Direction.Down;
                case 'a': return Direction.Left;
                case 'd': return Direction.Right;
                default: return Direction.Up;
            }
        }
    }
}
