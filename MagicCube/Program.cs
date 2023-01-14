namespace MagicCube
{
    class Program
    {
        public static void Main(string[] args)
        {
            using (Game game = new Game())
            {
                game.Run(800, 600, 60, @"assets\icon.png");
            }
        }
    }
}