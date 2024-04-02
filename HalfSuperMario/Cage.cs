using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfSuperMario
{
    public class Cage : GameObject
    {
        public Cage() : base(950, 500, false, new Bitmap("Cage", "cage1.png"))
        {
        }

        public override void Update()
        {
            if (HP <= 0)    // Cage is moved out of the Screen => Princess is rescued
            {
                X = 1200;
                Moveable = true;
            }
        }

        public override void Move()
        {
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
        }
    }
}
