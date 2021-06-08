using System;

namespace Alfapet
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Alfapet())
                game.Run();
        }
    }
}