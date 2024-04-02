using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfSuperMario
{
    public class Enemy : Character
    {
        private Staff _staff;

        public Staff Staff
        {
            get
            {
                return _staff;
            }
        }

        public Enemy() : base(SplashKit.ScreenWidth(), 505, true, new Bitmap("Enemy", "enemy1.png"))
        {
            _staff = new Staff();
            Weapon = _staff;
        }

        public override void Update()
        {
            Move();
            Weapon.X = X + 12;
            Weapon.Y = Y + 25;
            Attack();
        }

        public override void Move()
        {
            if (X >= 800)   // Enemy can move to a certain point only and will stop there
            {
                X -= 0.2;
            }
        }

        public override bool Attack()
        {
            Staff.Update();
            Staff.Strike();
            return true;
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
            Staff.Draw();
        }
    }
}