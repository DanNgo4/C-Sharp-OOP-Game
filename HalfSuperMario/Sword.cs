using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfSuperMario
{
    public class Sword : Weapon
    {
        private Bitmap _slashBitmap;

        public Bitmap SlashBitmap   // Bitmap for displaying the sword's slashing effect and attacking the opponents
        {
            get
            {
                return _slashBitmap;
            }
        }

        public Sword() : base(new Bitmap("Sword", "sword1.png"))
        {
            _slashBitmap = new Bitmap("Slash", "slash1.png");
        }

        public override void Strike()
        {
            base.Strike();
            Update();
        }

        public override void Update()
        {
            if (_isStriking)
            {
                TimeSpan strikeDuration = DateTime.Now - _strikeStartTime;  // time delaying function for Sword so that it cannot attack too fast

                if (strikeDuration.TotalMilliseconds <= 500)
                {
                    SplashKit.DrawBitmap(_slashBitmap, X, Y);
                }
                else
                {
                    _isStriking = false;
                }
            }
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
        }
    }
}