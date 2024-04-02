   using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfSuperMario
{
    public class Princess : Character
    {
        private Wand _wand;

        public Wand Wand
        {
             get
            {
                return _wand;
            }
        }

        public Princess() : base(970, 515, false, new Bitmap("Princess", "princess1.png"))
        {
            _wand = new Wand();
            Weapon = _wand;
        }

        public override void Update()
        {
            Weapon.X = X + 20;
            Weapon.Y = Y;
            if (Moveable)   // Rescued
            {
                Weapon.Update();
                Attack();
            }
        }

        public override void Move()
        {
        }

        public override bool Attack()
        {
            Weapon.Strike();
            return true;
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
            Wand.Draw();
        }
    }
}