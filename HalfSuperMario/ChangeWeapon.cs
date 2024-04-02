using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfSuperMario
{
    public class ChangeWeapon : Spell
    {
        public ChangeWeapon() : base(new Bitmap("Change Weapon", "changeweapon1.png"))
        { 
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
            base.Draw();
        }
    }
}