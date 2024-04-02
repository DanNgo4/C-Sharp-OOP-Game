using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfSuperMario
{
    public class Arrow : Object, IIsLaunchable
    {
        private bool _isMoveable;

        public bool IsMoveable
        {
            get
            {
                return _isMoveable;
            }
        }

        public Arrow() : base(0, 0, new Bitmap("Arrow", "arrow1.png"))
        {
            _isMoveable = true;
        }

        public override void Update()
        {
            if (X > SplashKit.ScreenWidth())
            {
                _isMoveable = false;
            }
            if (_isMoveable)
            {
                X += 2.5;
            }
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
        }
    }
}
