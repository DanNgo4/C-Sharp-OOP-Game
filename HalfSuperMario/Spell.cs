using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfSuperMario
{
    public class Spell : Object, IIsLaunchable
    {
        private Vector2D _vector;           // fields for Spell to be randomly
        private double _zigzagLimit = 20.0; // spawned from the top of the screen
        private bool _isMoveable;

        public bool IsMoveable
        {
            get
            {
                return _isMoveable;
            }
        }

        public Spell(Bitmap bmp) : base(SplashKit.Rnd(1068), 0, bmp)
        {
            _vector.X = Math.Pow(-1, Math.Abs(SplashKit.Rnd(2))) * _zigzagLimit;
            _vector.Y = +1;
            _isMoveable = true;
        }

        public override void Update()
        {
            if (Y > SplashKit.ScreenHeight() + Bitmap.Height)
            {
                _isMoveable = false;
            }
            if (_isMoveable)
            {
                X += _vector.X;
                Y += _vector.Y;

                // Reverse the zigzag direction when it reaches a certain point
                if (Math.Abs(X + 500) >= _zigzagLimit)
                {
                    _vector.X *= -0.1;
                }
            }
        }

        public override void Draw()
        {
            SplashKit.FillCircle(Color.RGBAColor(255, 255, 153, 100), X + (Bitmap.Width / 2), Y + (Bitmap.Width / 2), Bitmap.Width/2);
        }
    }
}
