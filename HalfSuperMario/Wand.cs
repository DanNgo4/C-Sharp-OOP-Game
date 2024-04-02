   using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalfSuperMario
{
    public class Wand : Weapon
    {
        private List<Spell> _spells;
        private SpellTypes _sType;

        public List<Spell> Spells
        {
            get
            {
                return _spells;
            }
        }

        public Wand() : base(new Bitmap("Wand", "wand1.png"))
        {
            _spells = new List<Spell>();
        }

        public override void Strike()
        {
            _sType = (SpellTypes)SplashKit.Rnd(3);
            float num = SplashKit.Rnd();
            Spell? spell = null;

            if (num < 0.002)
            {
                switch (_sType)
                {
                    case SpellTypes.ChangeWeapon:
                        spell = new ChangeWeapon();
                        break;
                    case SpellTypes.Heal:
                        spell = new Heal();
                         break;
                     case SpellTypes.Armour: 
                        spell = new Armour();
                        break;
                }
                spell.X = X + 45;
                spell.Y = Y + 15;
                _spells.Add(spell);
            }
        }

        public override void Update()
        {
            foreach (Spell s in _spells)
            {
                s.Update();
            }
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
            foreach (Spell s in _spells)
            {
                s.Draw();
            }
        }
    }
}
