using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using SplashKitSDK;
using HalfSuperMario;
using System.IO;
using static System.Formats.Asn1.AsnWriter;

namespace HalfSuperMario
{
    public class HalfSuperMario
    {
        private bool _lost = false;
        private bool _won = false;

        private Map _map;
        private Hero _hero;
        private bool _striked = true;
        private Enemy _enemy;
        private List<Bug> _bugs;

        private List<IIsLaunchable> _isLaunchable;
        private Font _font;

        private List<Spell> _spells;
        private SpellTypes _sType;
        private Princess _princess;
        private Cage _cage;

        public bool Lost
        {
            get
            { 
                return _lost; 
            }
        }

        public bool Won
        { 
            get 
            { 
                return _won; 
            } 
        }

        public HalfSuperMario()
        {
            _map = new Map();
            _hero = new Hero();
            _enemy = new Enemy();
            _bugs = new List<Bug>();

            _isLaunchable = new List<IIsLaunchable>();
            _font = new Font("Font", "Font.ttf");

            _spells = new List<Spell>();
            _sType = new SpellTypes();
            _princess = new Princess();
            _cage = new Cage();
        }

        public void Update()
        {
            Status();   // check game's status (Win, Lost or RescuePrincess)
            _isLaunchable.Clear();
            float num = SplashKit.Rnd();

            _hero.Update();
            _enemy.Update();
            foreach (Bug b in _bugs)
            {
                b.Update();
            }
            LaunchBug(num);

            LaunchSpell(num);
            foreach (Spell s in _spells)
            {
                s.Update();
            }
            _isLaunchable.AddRange(_enemy.Staff.Fireballs);     // Add fireballs,
            _isLaunchable.AddRange(_bugs);                      // bugs
            _isLaunchable.AddRange(_spells);                    // and spells to this List of interface called IIsLaunchable
            UseSpell();

            Remove();
            AttackHero();
        }

        private void Status()
        {
            if (_hero.HP <= 0)
            {
                SplashKit.DrawText("You Lost!", SplashKit.ColorRed(), _font, 100, SplashKit.ScreenWidth() / 3, SplashKit.ScreenHeight() / 5);
                _lost = true;
            }

            if ((_hero.Score >= 100) && (_hero.Score < 300))
            {
                RescuePrincess();
            }
            else if (_hero.Score >= 300 && _princess.Moveable)
            {
                SplashKit.DrawText("You Won!", SplashKit.ColorRed(), _font, 100, SplashKit.ScreenWidth() / 3, SplashKit.ScreenHeight() / 5);
                _won = true;
            }
        }

        private void LaunchBug(float num)
        {
            Bug bug;

            if (num < 0.006)
            {
                bug = new Bug();
                _bugs.Add(bug);
            }
        }

        private void LaunchSpell(float num)
        {
            _sType = (SpellTypes)SplashKit.Rnd(3);
            Spell? spell = null;

            if (num < 0.001)
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
                _spells.Add(spell);
            }
        }

        private void UseSpell()
        {
            List<IIsLaunchable> toRemove = new List<IIsLaunchable>();

            foreach (Spell s in _spells)
            {
                if (SplashKit.BitmapCollision(_hero.Bitmap, _hero.X, _hero.Y, s.Bitmap, s.X, s.Y))
                {
                    if (s is ChangeWeapon)
                    {
                        toRemove.Add(s);
                        _hero.ChangeWeapon();
                        break;
                    }
                    else if (s is Heal)
                    {
                        if (_hero.HP < 100)
                        {
                            toRemove.Add(s);
                            _hero.HP += 20;
                            break;
                        }
                    }
                    else if (s is Armour)
                    {
                        toRemove.Add(s);
                        _hero.HasArmour = true;
                        _hero.Armour = 100;
                        break;
                    }
                }
            }
            RemoveSpell(toRemove);  // Remove the Spell that Hero hits so one cannot be used multiple times
        }

        private void RemoveSpell(List<IIsLaunchable> toRemove)
        {
            foreach (Spell s in _spells)
            {
                if (!s.IsMoveable)  // Remove the Spell that moves out of the screen to save memory
                {
                    toRemove.Add(s);
                }
            }
            RemoveIsLaunchables(toRemove);
        }

        private void Remove()
        {
            List<IIsLaunchable> toRemove = new List<IIsLaunchable>();
            _striked = true;

            foreach (IIsLaunchable l in _isLaunchable)
            {
                toRemove.Add(RemoveOutOfScreen(l));     // Remove Bug, Fireball, Arrow that move out of the screen to save memory
                if (_hero.WeaponType == WeaponTypes.Sword)
                {
                    toRemove.Add(RemoveWhenSword(l));   // Remove Bug, Fireball that are hit by a Sword
                }
                else
                {
                    foreach (Arrow a in _hero.Bow.Arrows)
                    {
                        if (SplashKit.BitmapCollision(a.Bitmap, a.X, a.Y, l.Bitmap, l.X, l.Y))
                        {
                            toRemove.Add(a);    // Remove Arrow when hits Bug or Fireball
                            toRemove.Add(l);    // Remove Bug when hits Arrow
                            KillBug(l as Bug);  // Add 5 points to Hero's Score when successfully killed a Bug by Bow
                        }
                        else if (SplashKit.BitmapCollision(a.Bitmap, a.X, a.Y, _enemy.Bitmap, _enemy.X, _enemy.Y))
                        {
                            _enemy.HP -= 100;   // Kill Enemy if hit by an Arrow
                            toRemove.Add(a);    // Remove Arrow when hits Enemy
                            break;
                        }
                    }

                    foreach (IIsLaunchable f in _isLaunchable) 
                    {
                        toRemove.Remove(f as Fireball);     // Clear all Fireballs in the List so Fireball can destroy Arrow
                    }
                }
            }
            RemoveIsLaunchables(toRemove);
        }

        private IIsLaunchable RemoveOutOfScreen(IIsLaunchable l)
        {
            if (!l.IsMoveable)
            {
                return l;
            }
            return null;
        }

        private IIsLaunchable RemoveWhenSword(IIsLaunchable l)
        {
            if (SplashKit.BitmapCollision(_hero.Sword.SlashBitmap, _hero.Sword.X, _hero.Sword.Y, _enemy.Bitmap, _enemy.X, _enemy.Y) && _hero.Attack() && _striked)
            {
                _enemy.HP -= 50;
                _striked = false;   // Ensure _enemy.HP can only be reduced once per Sword hit
            }

            if (SplashKit.BitmapCollision(_hero.Sword.SlashBitmap, _hero.Sword.X, _hero.Sword.Y, l.Bitmap, l.X, l.Y) && _hero.Attack())
            {
                KillBug(l as Bug); // Add 5 points to Hero's Score when successfully killed a Bug by Sword
                return l;
            }
            return null;
            
        }

        private void RemoveIsLaunchables(List<IIsLaunchable> toRemove)
        {
            foreach (IIsLaunchable? l in toRemove)
            {
                _isLaunchable.Remove(l);    // Clear the toRemove objects in the List of interface
                _bugs.Remove(l as Bug);                         // Clear all toRemove Bugs,
                _hero.Bow.Arrows.Remove(l as Arrow);            // Arrows,
                _enemy.Staff.Fireballs.Remove(l as Fireball);   // Fireballs,
                _spells.Remove(l as Spell);                     // Spells
            }
            KillEnemy();    // Add 10 points to Hero's Score when successfully killed an Enemy
        }

        private void KillEnemy()
        {
            if (_enemy.HP <= 0)
            {
                _enemy = null;
                _enemy = new Enemy();   // Launch a new Enemy when the previous one dies
                _hero.Score += 10;
            }
        }

        private void KillBug(Bug l)
        {
            if (l is Bug)
            {
                _hero.Score += 5;
            }
        }

        private void AttackHero()
        {
            List<IIsLaunchable> toRemove = new List<IIsLaunchable>();

            foreach (IIsLaunchable l in _isLaunchable)
            {
                if (SplashKit.BitmapCollision(_hero.Bitmap, _hero.X, _hero.Y, l.Bitmap, l.X, l.Y))  // Hero is hit by a Bug or Fireball
                {
                    if (l is Fireball)
                    {
                        if (_hero.HasArmour)
                        {
                            _hero.Armour -= 15;     // Reduces Hero's Armour if it has Armour
                            toRemove.Add(l);
                            break;
                        }
                        else
                        {
                            _hero.HP -= 15;     // Otherwise reduces Hero's HP
                            toRemove.Add(l);
                            break;
                        }

                    }
                    else if (l is Bug)
                    {
                        if (_hero.HasArmour)
                        {
                            _hero.Armour -= 10;
                            toRemove.Add(l);
                            break;
                        }
                        else
                        {
                            _hero.HP -= 10;
                            toRemove.Add(l);
                            break;
                        }
                    }
                }
            }
            RemoveIsLaunchables(toRemove);  // Remove all Bugs and Fireballs that hit Hero
        }

        private void RescuePrincess()
        {
            if (_hero.WeaponType == WeaponTypes.Sword)      // Only use a Sword to destroy the Cage
            {
                if (SplashKit.BitmapCollision(_hero.Sword.SlashBitmap, _hero.Sword.X, _hero.Sword.Y, _cage.Bitmap, _cage.X, _cage.Y) && _hero.Attack() && _striked)
                {
                    _cage.HP -= 10;
                    _striked = false;
                }
            }
            _princess.Draw();
            _cage.Draw();
            _cage.Update();
            if (_cage.Moveable)     // Final Round when the Cage is destroyed
            {
                FinalRound();
            }
        }

        private void FinalRound()
        {
            _princess.X = _hero.X - 20;     // Princess follows
            _princess.Y = _hero.Y;          // Hero when rescued
            _princess.Update();
            _spells.AddRange(_princess.Wand.Spells);    // Add Spells in the Princess's Wand to the List of Spell in the game
            _princess.Moveable = true;
        }

        public void Draw()
        {
            DrawGraphics();
            DrawObjects();
        }

        private void DrawGraphics()
        {
            Bitmap _mapBitmap = new Bitmap("Map", "map1.jpg");
            SplashKit.DrawBitmap(_mapBitmap, 0, 0);
            SplashKit.DrawText("HP: " + _hero.HP.ToString(), SplashKit.ColorRed(), _font, 26, 5, 5);
            SplashKit.DrawText("Armour: " + _hero.Armour.ToString(), SplashKit.ColorGray(), _font, 26, 150, 5);
            SplashKit.DrawText("Score: " + _hero.Score.ToString(), SplashKit.ColorBlack(), _font, 26, 900, 5);
        }

        private void DrawObjects()
        {
            _hero.Draw();
            _enemy.Draw();
            foreach (Bug b in _bugs)
            {
                b.Draw();
            }
            foreach (Spell s in _spells)
            {
                s.Draw();
            }

            for (int i = 0; i < _map.Maps.GetLength(0); i++)
            {
                for (int j = 0; j < _map.Maps.GetLength(1); j++)
                {
                    string tileType = _map.Maps[i, j];
                    if (tileType == "b")
                    {
                        SplashKit.DrawBitmap(new Bitmap("Block", "block.png"), j * 51, i * 51);
                    }
                }
            }
        }

        public void SaveAndLoad(string name)
        {
            List<string> records = new List<string>();
            string filename = "C:\\Users\\ducda\\OneDrive - Swinburne University\\Documents\\1. Study\\1. Swinburne\\2. Semester 2\\COS20007 - Object Oriented Programming\\CustomProgram\\Records.txt";
                               // Directory to the text file that stores the players' records

            // Read existing records
            using (StreamReader reader = new StreamReader(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    records.Add(line);
                }
            }

            // Add the new score before checking
            records.Add(name + ": " + _hero.Score.ToString());

            // Ensure only the latest 5 records are stored, by removing the oldest entries
            if (records.Count > 5)
            {
                // removing the oldest entries
                records = records.Skip(records.Count - 5).ToList();
            }

            // Write the latest 5 records back to the file
            using (StreamWriter writer = new StreamWriter(filename, false)) // false to overwrite the file
            {
                foreach (string record in records)
                {
                    writer.WriteLine(record);
                }
            }

            // Display records on the screen
            int i = SplashKit.ScreenHeight() / 3;
            foreach (string record in records)
            {
                SplashKit.DrawText(record, SplashKit.ColorDarkBlue(), _font, 30, SplashKit.ScreenWidth() / 2.5, i);
                i += 35;
            }

            SplashKit.RefreshScreen(70);
            Thread.Sleep(10000); // Pause to show scores
        }
    }
}