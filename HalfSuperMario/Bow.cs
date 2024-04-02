using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfSuperMario
{
    public class Bow : Weapon
    {
        private List<Arrow> _arrows;

        public List<Arrow> Arrows
        {
            get
            {
                return _arrows; 
            }
        }

        public Bow() : base(new Bitmap("Bow", "bow1.png"))
        {
            _arrows = new List<Arrow>();
        }

        public override void Strike()
        {
            base.Strike();

            if (_isStriking)    // time delaying function for Bow so that it cannot attack too fast
            {
                TimeSpan slashDuration = (DateTime.Now - _strikeStartTime) + TimeSpan.FromMilliseconds(0.9953);
               
                if (slashDuration.TotalMilliseconds <= 1)
                {
                    Arrow a = new Arrow();
                    a.X = X;
                    a.Y = Y;
                    _arrows.Add(a);
                }
                else
                {
                    _isStriking = false;
                }
            }
        }

        public override void Update()
        {
            foreach (Arrow a in _arrows)
            {
                a.Update();
            }
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
            foreach (Arrow a in _arrows)
            {
                a.Draw();
            }
        }
    }
}