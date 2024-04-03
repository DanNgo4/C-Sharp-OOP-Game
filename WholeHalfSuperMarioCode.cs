using System;
using SplashKitSDK;
using System.Threading;

namespace HalfSuperMario
{
    public class Program
    {
        public static void Main()
        {
            Console.Write("Welcome to \"Half Super Mario\" Please enter your name: ");
            string? name = Console.ReadLine();
            Console.WriteLine("Welcome " + name + " , please enjoy the game.\n");

            Window w = new Window("Half Super Mario", 1067, 707);
            HalfSuperMario game = new HalfSuperMario();

            while (!w.CloseRequested)
            {
                SplashKit.ProcessEvents();
                SplashKit.ClearScreen();
                game.Draw();
                game.Update();
                SplashKit.RefreshScreen(70);
                if (game.Lost || game.Won)
                {
                    game.SaveAndLoad(name);
                    break;
                }
            }

            Console.WriteLine("Thanks " + name + " for playing!");
        }
    }

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

    public class Map
    {
        private string[,] _map;   // 2D array of string

        public string[,] Maps
        {
            get 
            { 
                return _map; 
            }
        }

        public Map(string[,] map)
        {
            _map = map;
        }

        public Map() : this(new string[,] {
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "b", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "b", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "b", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "b", "", "", "", "", "", "", "b", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""},
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" }
        })
        { }
    }

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

    public class Cage : GameObject
    {
        public Cage() : base(950, 500, false, new Bitmap("Cage", "cage1.png"))
        {
        }

        public override void Update()
        {
            if (HP <= 0)    // Cage is moved out of the Screen => Princess is rescued
            {
                X = 1200;
                Moveable = true;
            }
        }

        public override void Move()
        {
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
        }
    }

    public class Character : GameObject
    {
        private Weapon _weapon;

        protected virtual Weapon Weapon
        {
            get
            {
                return _weapon;
            }
            set
            {
                _weapon = value;
            }
        }
        

        protected Character(double x, double y, bool moveable, Bitmap bmp) : base(x, y, moveable, bmp)
        {
        }

        public override void Update()
        {
        }

        public override void Move()
        {
        }

        public virtual bool Attack()
        {
            return true;
        }

        public override void Draw()
        {
        }
    }

    public class Princess : Character
    {
        private Wand _wand;

        public Wand Wand
        {
             get
            {
                return _wand;
            }
        }

        public Princess() : base(970, 515, false, new Bitmap("Princess", "princess1.png"))
        {
            _wand = new Wand();
            Weapon = _wand;
        }

        public override void Update()
        {
            Weapon.X = X + 20;
            Weapon.Y = Y;
            if (Moveable)   // Rescued
            {
                Weapon.Update();
                Attack();
            }
        }

        public override void Move()
        {
        }

        public override bool Attack()
        {
            Weapon.Strike();
            return true;
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
            Wand.Draw();
        }
    }

    public class Enemy : Character
    {
        private Staff _staff;

        public Staff Staff
        {
            get
            {
                return _staff;
            }
        }

        public Enemy() : base(SplashKit.ScreenWidth(), 505, true, new Bitmap("Enemy", "enemy1.png"))
        {
            _staff = new Staff();
            Weapon = _staff;
        }

        public override void Update()
        {
            Move();
            Weapon.X = X + 12;
            Weapon.Y = Y + 25;
            Attack();
        }

        public override void Move()
        {
            if (X >= 800)   // Enemy can move to a certain point only and will stop there
            {
                X -= 0.2;
            }
        }

        public override bool Attack()
        {
            Staff.Update();
            Staff.Strike();
            return true;
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
            Staff.Draw();
        }
    }

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
            Weapon = _sword;            // default Weapon:
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

    public enum WeaponTypes
    {
        Sword,
        Bow,
        Staff,
        Wand
    }

    public class Weapon : Object
    {
        protected DateTime _strikeStartTime;
        protected bool _isStriking;

        public Weapon(Bitmap bmp) : base(0, 0, bmp)
        {
            _isStriking = false;
        }

        public virtual void Strike()
        {
            if (!_isStriking)   // time delaying function for Bow and Sword so that they cannot attack too fast
            {
                _strikeStartTime = DateTime.Now;
                _isStriking = true;
            }
        }

        public override void Update()
        {
        }

        public override void Draw()
        {
        }
    }

    public class Sword : Weapon
    {
        private Bitmap _slashBitmap;

        public Bitmap SlashBitmap   // Bitmap for displaying the sword's slashing effect and attacking the opponents
        {
            get
            {
                return _slashBitmap;
            }
        }

        public Sword() : base(new Bitmap("Sword", "sword1.png"))
        {
            _slashBitmap = new Bitmap("Slash", "slash1.png");
        }

        public override void Strike()
        {
            base.Strike();
            Update();
        }

        public override void Update()
        {
            if (_isStriking)
            {
                TimeSpan strikeDuration = DateTime.Now - _strikeStartTime;  // time delaying function for Sword so that it cannot attack too fast

                if (strikeDuration.TotalMilliseconds <= 500)
                {
                    SplashKit.DrawBitmap(_slashBitmap, X, Y);
                }
                else
                {
                    _isStriking = false;
                }
            }
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
        }
    }

    public class Bow : Weapon
    {
        private List<Arrow> _arrows;

        public List<Arrow> Arrows
        {
            get
            {
                return _arrows; 
            }
        }

        public Bow() : base(new Bitmap("Bow", "bow1.png"))
        {
            _arrows = new List<Arrow>();
        }

        public override void Strike()
        {
            base.Strike();

            if (_isStriking)    // time delaying function for Bow so that it cannot attack too fast
            {
                TimeSpan slashDuration = (DateTime.Now - _strikeStartTime) + TimeSpan.FromMilliseconds(0.9953);
               
                if (slashDuration.TotalMilliseconds <= 1)
                {
                    Arrow a = new Arrow();
                    a.X = X;
                    a.Y = Y;
                    _arrows.Add(a);
                }
                else
                {
                    _isStriking = false;
                }
            }
        }

        public override void Update()
        {
            foreach (Arrow a in _arrows)
            {
                a.Update();
            }
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
            foreach (Arrow a in _arrows)
            {
                a.Draw();
            }
        }
    }

    public class Staff : Weapon
    {
        private List<Fireball> _fireballs;

        public List<Fireball> Fireballs
        {
            get
            {
                return _fireballs;
            }
        }

        public Staff() : base(new Bitmap("Staff", "staff1.png"))
        {
            _fireballs = new List<Fireball>();
        }

        public override void Strike()
        {
            Fireball f;
            float num = SplashKit.Rnd();

            if (num < 0.005)
            {
                f = new Fireball();
                f.X = X;
                f.Y = Y - SplashKit.Rnd(3, 6);  // launch Fireball on a random Y-axis
                _fireballs.Add(f);
            }
        }

        public override void Update()
        {
            foreach (Fireball f in _fireballs)
            {
                f.Update();
            }
        }

        public override void Draw()
        {
            SplashKit.DrawBitmap(Bitmap, X, Y);
            foreach (Fireball f in _fireballs)
            {
                f.Draw();
            }
        }
    }

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

    public enum SpellTypes
    {
        ChangeWeapon,
        Heal,
        Armour
    }

    public class Bug : Object, IIsLaunchable
    {
        private Vector2D _vector;           // fields for Bug to randomly
        private double _zigzagLimit = 20.0; // move in a zigzag pattern
        private bool _isMoveable;

        public bool IsMoveable
        {
            get
            {
                return _isMoveable;
            }
        }

        public Bug() : base(SplashKit.ScreenWidth(), 505, new Bitmap("Bug", "bug1.png"))
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

    public class Heal : Spell
    {
        public Heal() : base(new Bitmap("Heal", "heal1.png"))
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

    public class Armour : Spell
    {
        public Armour() : base(new Bitmap("Armour", "armour1.png"))
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