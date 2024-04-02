using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfSuperMario
{
    public class Staff : Weapon
    {
        private List<Fireball> _fireballs;

        public List<Fireball> Fireballs
        {
            get
            {
                return _fireballs;
            }
        }

        public Staff() : base(new Bitmap("Staff", "staff1.png"))
        {
            _fireballs = new List<Fireball>();
        }

        public override void Strike()
        {
            Fireball f;
            float num = SplashKit.Rnd();

            if (num < 0.005)
            {
                f = new Fireball();
                f.X = X;
                f.Y = Y - SplashKit.Rnd(3, 6);  // launch Fireball on a random Y-axis
                _fireballs.Add(f);
            }
        }

        public override void Update()
        {
            foreach (Fireball f in _fireballs)
            {
                f.Update();
            }
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
            foreach (Fireball f in _fireballs)
            {
                f.Draw();
            }
        }
    }
}