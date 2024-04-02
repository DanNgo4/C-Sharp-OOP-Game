using System;
using SplashKitSDK;
using System.Threading;

namespace HalfSuperMario
{
    public class Program
    {
        public static void Main()
        {
            Console.Write("Welcome to \"Half Super Mario\" Please enter your name: ");
            string? name = Console.ReadLine();
            Console.WriteLine("Welcome " + name + " , please enjoy the game.\n");

            Window w = new Window("Half Super Mario", 1067, 707);
            HalfSuperMario game = new HalfSuperMario();

            while (!w.CloseRequested)
            {
                SplashKit.ProcessEvents();
                SplashKit.ClearScreen();
                game.Draw();
                game.Update();
                SplashKit.RefreshScreen(70);
                if (game.Lost || game.Won)
                {
                    game.SaveAndLoad(name);
                    break;
                }
            }

            Console.WriteLine("Thanks " + name + " for playing!");
        }
    }
}