using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfSuperMario
{
    public class Fireball : Object, IIsLaunchable
    {
        private Vector2D _vector;           // fields for Fireball to randomly
        private double _zigzagLimit = 15.0; // move in a zigzag pattern
        private bool _isMoveable;

        public bool IsMoveable
        {
            get
            {
                return _isMoveable;
            }
        }

        public Fireball() : base(0, 0, new Bitmap("Fireball", "fireball1.png"))
        {
            _vector.X = -1;
            _vector.Y = Math.Pow(-1, Math.Abs(SplashKit.Rnd(2))) * _zigzagLimit;
            _isMoveable = true;
        }

        public override void Update()
        {
            if (X < -Bitmap.Width)
            {
                _isMoveable = false;
            }
            if (_isMoveable)
            {
                X += _vector.X;
                Y += _vector.Y;

                // Reverse the zigzag direction when it reaches a certain point
                if (Math.Abs(Y - 500) >= _zigzagLimit)
                {
                    _vector.Y *= -0.1;
                }
            }
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
        }
    }
}