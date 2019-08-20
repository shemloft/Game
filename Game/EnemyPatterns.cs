using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public interface IEnemyAct
    {
        GameState Act(GameState state);
        int GetHP();
    }

    public class EnemyFeatures
    {
        public int ShootingFrequency;
        public int SpawnFrequency;

        public double BulletVelocity;
        public double EnemyVelocity;

        public int Lifetime;
        public int Iteration;
        public int ShootingIteration;
        public int SpawnIteration;

        public Boss Boss; //выглядит очень глупо, но как-то же надо передавать хп босса в форму

        public bool IsBoss()
        {
            return Boss != null;
        }

        public bool IsLifetimeEnd()
        {
            if (IsBoss())
                return Boss.HP <= 0;
            return Iteration >= Lifetime;
        }
    }

    public class EnemyPattern
    {

        protected EnemyFeatures EnemyFeatures;

        protected Game Game;

        public PlayerBullet EnemyTouchedPlayerBullet(Enemy enemy, GameState state)
        {
            foreach (var bullet in state.PlayerBullets)
            {
                if (Game.CharactersCollided(enemy, bullet, Game.GameFeatures.EnemyHitbox) && !enemy.IsImmortal)
                    return bullet;
            }
            return null;
        }

        public int GetHP()
        {
            return EnemyFeatures.Boss.HP;
        }

        public bool EnemyKilledPlayer(Enemy enemy, GameState state)
        {
            if (enemy is Boss || enemy.IsImmortal)
                return false;
            var hitbox = Math.Max(Game.GameFeatures.PlayerHitbox, Game.GameFeatures.EnemyHitbox);
            return Game.CharactersCollided(enemy, Game.Player, hitbox);
        }

        public GameState MoveEnemies(
            GameState state,
            Game game,
            int shootingIteration,
            int shootingFrequency,
            bool needToShoot,
            Func<Enemy, ShootingPattern[]> shootingDirection)
        {
            var updatedEnemyHashset = new HashSet<Enemy>();
            foreach (var enemy in state.Enemies)
            {
                enemy.Move();
                if (EnemyKilledPlayer(enemy, state))
                {
                    game.AddDeadEnemy(enemy);
                    state.Enemies.Remove(enemy);
                    state.IsOver = true;
                    return state;
                }
                var bulletKilledEnemy = false; // такое название логичнее
                var bulletThatTouchedEnemy = EnemyTouchedPlayerBullet(enemy, state);
                if (bulletThatTouchedEnemy != null)
                {
                    state.PlayerBullets.Remove(bulletThatTouchedEnemy);
                    //game.CurrentGameState.DeadCharacters.Add(Tuple.Create(bulletThatTouchedEnemy.Location, 0));
                    if (enemy is Boss)
                    {
                        var boss = enemy as Boss;
                        boss.HP--;
                        //EnemyFeatures.Boss.HP--;
                        bulletKilledEnemy = boss.HP <= 0;
                        if (boss.HP <= 0)
                            game.AddDeadEnemy(enemy);
                    }
                    else
                    {
                        game.AddDeadEnemy(enemy);
                        game.Score += 100;
                        game.TryToAddPower(enemy);
                        bulletKilledEnemy = true;
                    }
                }
                if (!EnemyFeatures.IsBoss() && game.InField(enemy.Location) && !bulletKilledEnemy ||
                    EnemyFeatures.IsBoss() && !bulletKilledEnemy)
                    updatedEnemyHashset.Add(enemy);
                else continue;
                if (!needToShoot || shootingIteration != shootingFrequency)
                    continue;
                foreach (var vector in shootingDirection(enemy))
                    enemy.Shoot(vector);
            }
            state.Enemies = updatedEnemyHashset;
            return state;
        }

        public GameState Act(
            GameState state,
            Func<bool> spawnCondition,
            Action<HashSet<Enemy>> spawn,
            bool needToShoot, Func<Enemy, ShootingPattern[]> shootingDirection)
        {
            EnemyFeatures.SpawnIteration++;
            EnemyFeatures.ShootingIteration++;

            var updatedState = MoveEnemies(
                state, Game,
                EnemyFeatures.ShootingIteration,
                EnemyFeatures.ShootingFrequency,
                needToShoot, shootingDirection);

            if (updatedState.IsOver)
                return updatedState;

            var updatedEnemyHashset = updatedState.Enemies;

            if (spawnCondition())
                spawn(updatedEnemyHashset);


            if (EnemyFeatures.SpawnIteration == EnemyFeatures.SpawnFrequency) EnemyFeatures.SpawnIteration = 0;
            if (EnemyFeatures.ShootingIteration == EnemyFeatures.ShootingFrequency) EnemyFeatures.ShootingIteration = 0;

            EnemyFeatures.Iteration++;
            updatedState.Enemies = updatedEnemyHashset;

            return EnemyFeatures.IsLifetimeEnd() ? null : updatedState;
        }
    }

    public class EnemyPatternNothing : EnemyPattern, IEnemyAct
    {
        public EnemyPatternNothing(Game game, int lifetime)
        {
            Game = game;
            EnemyFeatures = new EnemyFeatures { Lifetime = lifetime };
        }

        public GameState Act(GameState state)
        {
            return Act(state, () => false, x => x.Add(null), false, x => new[] { new ShootingPattern {
                Direction = Vector.Down } });
        }
    }

    public class EnemyPatternHalfCircle : EnemyPattern, IEnemyAct
    {
        private readonly Point leftStart;
        private readonly Point rightStart;
        private readonly Func<Enemy, ShootingPattern[]> shootingDirection;


        public EnemyPatternHalfCircle(
            Point leftStart, Point rightStart, Game game,
            Func<Enemy, ShootingPattern[]> shootingDirection)
        {
            this.leftStart = leftStart;
            this.rightStart = rightStart;
            Game = game;

            EnemyFeatures = new EnemyFeatures
            {
                BulletVelocity = 5,
                EnemyVelocity = 3,
                Lifetime = 500,
                ShootingFrequency = 50,
                SpawnFrequency = 20
            };

            this.shootingDirection = shootingDirection;

        }

        public GameState Act(GameState state)
        {
            return Act(state, () => EnemyFeatures.SpawnIteration == EnemyFeatures.SpawnFrequency, x =>
            {
                x.Add(new Enemy(rightStart, Game, VectorMoves.MoveHalfCircleRight(new Vector(rightStart)),
                    EnemyFeatures.EnemyVelocity, EnemyFeatures.BulletVelocity));
                x.Add(new Enemy(leftStart, Game, VectorMoves.MoveHalfCircleLeft(new Vector(leftStart)),
                    EnemyFeatures.EnemyVelocity, EnemyFeatures.BulletVelocity));
            }, true, shootingDirection);
        }
    }

    public class EnemyPatternBombing : EnemyPattern, IEnemyAct
    {
        private readonly int enemyCount;
        private bool needToSpawn = true;
        private readonly Func<Enemy, ShootingPattern[]> shootingDirection;

        public EnemyPatternBombing(Game game, int enemyCount, EnemyFeatures features,
            Func<Enemy, ShootingPattern[]> shootingDirection)
        {
            Game = game;

            EnemyFeatures = features;

            this.enemyCount = enemyCount;
            this.shootingDirection = shootingDirection;
        }

        public GameState Act(GameState state)
        {
            return Act(state, () => needToSpawn, x =>
            {
                for (var i = Game.GameFeatures.FieldSize.Width / (enemyCount + 1); 
                         i < Game.GameFeatures.FieldSize.Width;
                         i += Game.GameFeatures.FieldSize.Width / (enemyCount + 1))
                {
                    x.Add(new Enemy(new Point(i, 0), Game,
                        VectorMoves.MoveForBombing(new Vector(i, 0)), EnemyFeatures.EnemyVelocity, EnemyFeatures.BulletVelocity));
                }
                needToSpawn = false;
            }, true, shootingDirection);
        }
    }

    public class EnemyPatternExplosion : EnemyPattern, IEnemyAct
    {
        private readonly int enemyCount;
        private bool needToSpawn = true;
        private readonly Vector[] directions = Directions.SimpleAround;
        //private readonly Func<Enemy, Vector[]> shootingDirection;

        public EnemyPatternExplosion(Game game, int enemyCount, int shootingFrequency, double bulletVelocity)
        {
            Game = game;
            EnemyFeatures = new EnemyFeatures
            {
                BulletVelocity = bulletVelocity,
                EnemyVelocity = 2,
                Lifetime = 150,
                ShootingFrequency = shootingFrequency
            };

            this.enemyCount = enemyCount;
        }

        public GameState Act(GameState state)
        {
            return base.Act(state, () => needToSpawn, x =>
            {
                for (var i = Game.GameFeatures.FieldSize.Width / (enemyCount + 1); 
                         i < Game.GameFeatures.FieldSize.Width;
                         i += Game.GameFeatures.FieldSize.Width / (enemyCount + 1))
                {
                    x.Add(new Enemy(new Point(i, 0), Game,
                        VectorMoves.MoveForBombing(new Vector(i, 0)), EnemyFeatures.EnemyVelocity, EnemyFeatures.BulletVelocity));
                }
                needToSpawn = false;
            }, true, x => directions.Select(z => new ShootingPattern { Direction = z }).ToArray());
        }
    }

    public class EnemyPatternDiagonal : EnemyPattern, IEnemyAct
    {
        private readonly Point leftStart;
        private readonly Point rightStart;

        public EnemyPatternDiagonal(Game game, Point leftStart, Point rightStart)
        {
            Game = game;
            this.leftStart = leftStart;
            this.rightStart = rightStart;

            EnemyFeatures = new EnemyFeatures
            {
                BulletVelocity = 3,
                EnemyVelocity = 4,
                Lifetime = 250,
                ShootingFrequency = 20,
                SpawnFrequency = 20
            };
        }

        public GameState Act(GameState state)
        {
            return base.Act(state, () => EnemyFeatures.SpawnIteration == EnemyFeatures.SpawnFrequency, x =>
            {
                x.Add(new Enemy(leftStart, Game,
                    VectorMoves.EightLeft(new Vector(leftStart)), EnemyFeatures.EnemyVelocity, EnemyFeatures.BulletVelocity));
                x.Add(new Enemy(rightStart, Game,
                   VectorMoves.EightRight(new Vector(rightStart)), EnemyFeatures.EnemyVelocity, EnemyFeatures.BulletVelocity));
            }, true, x => new[] { new ShootingPattern { Direction = Vector.Down } });
        }
    }

}
