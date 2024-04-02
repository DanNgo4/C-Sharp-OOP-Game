using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfSuperMario
{
    public class Weapon : Object
    {
        protected DateTime _strikeStartTime;
        protected bool _isStriking;

        public Weapon(Bitmap bmp) : base(0, 0, bmp)
        {
            _isStriking = false;
        }

        public virtual void Strike()
        {
            if (!_isStriking)   // time delaying function for Bow and Sword so that they cannot attack too fast
            {
                _strikeStartTime = DateTime.Now;
                _isStriking = true;
            }
        }

        public override void Update()
        {
        }

        public override void Draw()
        {
        }
    }
}