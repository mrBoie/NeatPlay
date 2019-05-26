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
            DrawBoard(game, gameBoard.GetScore()); ;

            try
            {
                var input = 's';
                do
                {

                    input = Console.ReadKey().KeyChar;
                    if (input == 'q') break;
                    var direction = GetDirection(input);
                    gameBoard.Update(direction);
                    DrawBoard(gameBoard.GetGameBoard(), gameBoard.GetScore());

                } while (input != 'q');
            }
            catch(Exception e)
            {
                Console.WriteLine("You Lost");
            }


            Console.ReadLine();
        }

        public static void DrawBoard(char [,] board, int score)
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
