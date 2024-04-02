using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfSuperMario
{
    public class Character : GameObject
    {
        private Weapon _weapon;

        protected virtual Weapon Weapon
        {
            get
            {
                return _weapon;
            }
            set
            {
                _weapon = value;
            }
        }
        

        protected Character(double x, double y, bool moveable, Bitmap bmp) : base(x, y, moveable, bmp)
        {
        }

        public override void Update()
        {
        }

        public override void Move()
        {
        }

        public virtual bool Attack()
        {
            return true;
        }

        public override void Draw()
        {
        }
    }
}