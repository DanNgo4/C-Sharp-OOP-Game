using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfSuperMario
{
    public abstract class Object
    {
        private double _x;
        private double _y;
        private Bitmap _bitmap;

        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        public Bitmap Bitmap
        {
            get
            {
                return _bitmap;
            }
        }

        protected Object(double x, double y, Bitmap bmp)
        {
            _x = x;
            _y = y;
            _bitmap = bmp;
        }

        public abstract void Update();

        public abstract void Draw();
    }
}