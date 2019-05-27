namespace SnakeGame
{
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
}
