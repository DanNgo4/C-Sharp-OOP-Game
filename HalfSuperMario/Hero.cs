using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfSuperMario
{
    public class Hero : Character
    {
        private Sword _sword;
        private Bow _bow;
        private WeaponTypes _wType;

        private int _score;
        private int _armour;
        private bool _hasArmour;

        private bool _jumping;      // attribues
        private double _jumpSpeed;  // for Hero
        private double _gravity;    // to implement
        private double _initialY;   // Jumping Method

        public Sword Sword
        {
            get
            {
                return _sword;
            }
        }

        public Bow Bow
        {
            get
            {
                return _bow;
            }
        }

        public WeaponTypes WeaponType
        {
            get
            { 
                return _wType; 
            }
        }

        public int Score
        {
            get
            {
                return _score;
            }
            set
            {
                _score = value;
            }
        }

        public int Armour
        { 
            get 
            { 
                return _armour; 
            } 
            set 
            { 
                _armour = value; 
            } 
        }

        public bool HasArmour
        {
            get
            {
                return _hasArmour;
            }
            set
            {
                _hasArmour = value;
            }
        }

        public Hero() : base(50, 515, true, new Bitmap("Hero", "hero1.png"))
        {
            _sword = new Sword();
            _bow = new Bow();
            Weapon = _bow;            // default Weapon:
            _wType = WeaponTypes.Sword; // Sword

            _score = 0;
            _armour = 0;
            _hasArmour = false;

            _jumping = false;
            _initialY = Y;
            _gravity = 0.5f;
        }

        public override void Update()
        {
            if (HP > 100)
            {
                HP = 100;   // maximum HP of a Hero
            }
            if (Armour <= 0)
            {
                HasArmour = false;
            }
            Move();
            Weapon.X = X + 35;
            Weapon.Y = Y + 5;
            Attack();
        }

        public override void Move()
        {
            if (SplashKit.KeyDown(KeyCode.RightKey) && X <= 1020)
            {
                X += 2;
            }
            else if (SplashKit.KeyDown(KeyCode.LeftKey) && X >= 0)
            {
                X -= 2;
            }
            Jumping();
        }

        private void Jumping()
        {
            if (SplashKit.KeyDown(KeyCode.UpKey) && !_jumping)
            {
                _jumping = true;
                _initialY = Y;
                _jumpSpeed = -8;
            }
            if (_jumping)
            {
                Y += _jumpSpeed;
                _jumpSpeed += _gravity;
            }
            if (Y >= _initialY)
            {
                Y = _initialY;
                _jumping = false;
            }
        }

        public void ChangeWeapon()
        {
            if (_wType == WeaponTypes.Sword)
            {
                _wType = WeaponTypes.Bow;
                Weapon = _bow;
            }
            else
            {
                _wType = WeaponTypes.Sword;
                Weapon = _sword;
            }
        }

        public override bool Attack()
        {
            if (_wType == WeaponTypes.Bow)
            {
                Weapon.Update();
            }
            if (SplashKit.KeyTyped(KeyCode.SpaceKey))
            {
                Weapon.Strike();
                return true;
            }
            return false;
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
            Weapon.Draw();
        }
    }
}