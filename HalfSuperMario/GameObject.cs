using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;

namespace HalfSuperMario
{
    public abstract class GameObject : Object
    {
        private int _hp;
        private bool _moveable;

        public int HP
        {
            get
            {
                return _hp;
            }
            set
            {
                _hp = value;
            }
        }

        public bool Moveable
        {
            get
            {
                return _moveable;
            }
            set
            {
                _moveable = value;
            }
        }

        protected GameObject(double x, double y, bool moveable, Bitmap bmp) : base(x, y, bmp)
        {
            _hp = 100;
            _moveable = moveable;
        }

        public abstract void Move();
    }
}