using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfSuperMario
{
    public interface IIsLaunchable  // interface for Bug, Arrow, Fireball and Spells
    {
        public double X
        {
            get;
            set;
        }

        public double Y
        {
            get;
            set;
        }

        public Bitmap Bitmap
        {
            get;
        }

        public bool IsMoveable
        {
            get;
        }
    }
}