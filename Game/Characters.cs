using System;
using System.Drawing;
using System.Collections.Generic;

namespace Game
{
    public interface ICharacter
    {
        void Move();
        Vector GetLocation();
        Vector GetPreviousLocation();
        string GetImage();
    }

    public class Player : ICharacter
    {
        public Vector Location;
        public Vector PreviousLocation;
        private readonly Game game;
        private int ReloadingFrequency = 5;
        public int Reloading;
        public int HP;
        public int Power;
        public string Name;
        public Queue<Direction> Directions;

        public Direction currentDirection = Direction.NoDirection;

        public Player(Point location, Game game)
        {
            Location = new Vector(location);
            this.game = game;
            HP = Game.GameFeatures.PlayerHP;
            Name = "Fighter for justice";
            Reload();
            Directions = new Queue<Direction>
            (
                new []{
                Direction.NoDirection,
                Direction.NoDirection,
                Direction.NoDirection
                }
            );
        }

        public void Move()
        {
            PreviousLocation = Location;
            Directions.Dequeue();
            Directions.Enqueue(currentDirection);
            switch (currentDirection)
            {
                case Direction.Down:
                    Location += Game.GameFeatures.BasicStep * Vector.Down;
                    break;
                case Direction.Up:
                    Location += Game.GameFeatures.BasicStep * Vector.Up;
                    break;
                case Direction.Left:
                    Location += Game.GameFeatures.BasicStep * Vector.Left;
                    break;
                case Direction.Right:
                    Location += Game.GameFeatures.BasicStep * Vector.Right;
                    break;
                case Direction.NoDirection:
                    return;
            }
            Location = Location.BoundTo(Game.GameFeatures.FieldSize);
        }

        public void Move(Direction direction)
        {
            currentDirection = direction;
            Move();
        }

        public void Shoot()
        {
            if (Reloading != ReloadingFrequency)
            {
                Reloading++;
                return;
            }
            if (Power < 1)
                game.CurrentGameState.PlayerBullets.Add(
                        new PlayerBullet(Location, Vector.Up, Game.GameFeatures.BasicStep * 2));


            if (Power >= 1)
            {
                game.CurrentGameState.PlayerBullets.Add(
                    new PlayerBullet(Location, Vector.Up.Rotate(Math.PI / 240), Game.GameFeatures.BasicStep * 2));
                game.CurrentGameState.PlayerBullets.Add(
                    new PlayerBullet(Location, Vector.Up.Rotate(-Math.PI / 240), Game.GameFeatures.BasicStep * 2));
            }

            if (Power >= 3)
            {
                game.CurrentGameState.PlayerBullets.Add(
                    new PlayerBullet(Location, Vector.Up.Rotate(Math.PI / 30), Game.GameFeatures.BasicStep * 2));
                game.CurrentGameState.PlayerBullets.Add(
                    new PlayerBullet(Location, Vector.Up.Rotate(-Math.PI / 30), Game.GameFeatures.BasicStep * 2));
            }

            if (Power >= 5)
            {
                game.CurrentGameState.PlayerBullets.Add(
                    new PlayerBullet(Location, Vector.Up.Rotate(Math.PI / 10), Game.GameFeatures.BasicStep * 2));
                game.CurrentGameState.PlayerBullets.Add(
                    new PlayerBullet(Location, Vector.Up.Rotate(-Math.PI / 10), Game.GameFeatures.BasicStep * 2));
            }
            Reloading = 0;
        }

        public void Reload()
        {
            Reloading = ReloadingFrequency;
        }

        public Vector GetPreviousLocation() => PreviousLocation;

        public Vector GetLocation() => Location;


        public string GetImage()
        {
            return ImageSelector.ChoosePlayerImage(this);
        }
    }


    public class Enemy : ICharacter
    {
        public Vector PreviousLocation;
        public Vector Location;
        public IMovingPattern MovingPattern;
        private readonly string imageName;
        private readonly Game game;
        private readonly double enemyStep;
        private readonly double bulletStep;
        public bool IsImmortal;

        public Enemy(Point location, Game game, IMovingPattern pattern, double enemyStep, double bulletStep,
            string imageName = "enemy.png", bool isImmortal = false)
        {
            Location = new Vector(location);
            this.game = game;
            MovingPattern = pattern;
            this.enemyStep = enemyStep;
            this.bulletStep = bulletStep;
            this.imageName = imageName;
            IsImmortal = isImmortal;
        }

        public void Move()
        {
            PreviousLocation = Location;
            Location = MovingPattern.Move(enemyStep);
        }

        public void Shoot(ShootingPattern pattern)
        {

            if (pattern.Pattern != null)
                game.CurrentGameState.EnemyBullets.Add(
                    new EnemyBullet(Location, pattern.Direction, bulletStep, pattern.BulletImage, pattern.Pattern));
            else if (pattern.BulletImage != null)
                game.CurrentGameState.EnemyBullets.Add(
                    new EnemyBullet(Location, pattern.Direction, bulletStep, pattern.BulletImage));
            else
                game.CurrentGameState.EnemyBullets.Add(
                    new EnemyBullet(Location, pattern.Direction, bulletStep));
        }

        public Vector GetLocation() => Location;

        public Vector GetPreviousLocation() => PreviousLocation;

        public string GetImage() => imageName;
    }

    public class PlayerBullet : ICharacter
    {
        public Vector Location;
        public Vector PreviousLocation;
        private readonly Vector velocity;
        private readonly double step;

        public PlayerBullet(Vector location, Vector velocity, double step)
        {
            Location = location;
            this.velocity = velocity;
            this.step = step;
        }

        public void Move()
        {
            PreviousLocation = Location;
            Location += velocity * step;
        }

        public Vector GetLocation() => Location;

        public Vector GetPreviousLocation() => PreviousLocation;

        public string GetImage() => "player-bullet.png";
    }

    public class EnemyBullet : ICharacter
    {
        public Vector Location;
        public Vector PreviousLocation;
        private readonly Vector velocity;
        private readonly double step;
        private readonly IMovingPattern pattern;
        private readonly string image;

        public EnemyBullet(Vector location, Vector velocity, double step, string imageName = "enemy-bullet.png", IMovingPattern pattern = null)
        {
            Location = location;
            this.velocity = velocity.Normalize();
            this.step = step;
            this.pattern = pattern;
            image = imageName;
        }

        public void Move()
        {
            PreviousLocation = Location;
            if (pattern != null)
                Location = pattern.Move(step);
            else
                Location += velocity * step;
        }

        public Vector GetLocation() => Location;

        public Vector GetPreviousLocation() => PreviousLocation;

        public string GetImage() => image;
    }

    public class Power : ICharacter
    {
        public Vector Location;
        public Vector PreviousLocation;
        private readonly Vector velocity;
        private readonly double step;
        private readonly Action<Player> updateStats;
        private readonly string image;

        public Power(Vector location, Action<Player> updateStats, string image)
        {
            Location = location;
            velocity = Vector.Down;
            step = 3;
            this.updateStats = updateStats;
            this.image = image;
        }

        public void Move()
        {
            PreviousLocation = Location;
            Location += velocity * step;
        }

        public Vector GetLocation() => Location;

        public Vector GetPreviousLocation() => PreviousLocation;

        public void UpdateStats(Player player)
        {
            updateStats(player);
        }

        public string GetImage() => image;
    }

    public class HpBar : Power
    {
        public HpBar(Vector location)
            : base(location, x => x.HP++, "hpbar.png")
        {
        }
    }

    public class PowerBar : Power
    {
        public PowerBar(Vector location)
            : base(location, x => x.Power++, "powerbar.png")
        {
        }
    }

    public class Boss : Enemy
    {
        public string Name;
        public int HP;

        public Boss(Point location, Game game, IMovingPattern pattern,
            double enemyStep, double bulletStep, bool isImmortal = false)
                : base(location, game, pattern, enemyStep, bulletStep, "Monster.png", isImmortal)
        {
        }
    }

}
