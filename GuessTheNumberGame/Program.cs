using GameCore;
using GameCore.Services;


namespace Program
{
    class RunGame
    {
        static void Main(string[] args)
        {
            var userInputService = new ConsoleUserInput();
            Play play = new(userInputService);
            play.Start();
        }
    }
}
