namespace MagicCube
{
    class Program
    {
        public static void Main()
        {
            using (Game game = new Game("Magic Cube", @"assets\icon.png", 800, 800, 60))
            {
                game.Run();
            }
        }
    }
}