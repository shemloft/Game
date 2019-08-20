using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Game
{
    public class BossFight : EnemyPattern, IEnemyAct
    {
        private bool needToSpawn = true;
        private Point start;
        private int phase;
        private int phaseIteration;
        private Random rnd = new Random();
        private int breakLen;
        private bool isBreak;
        private int phaseCount;

        public BossFight(Game game, EnemyFeatures features, Point start, int hp)
        {
            Game = game;
            EnemyFeatures = features;
            EnemyFeatures.Boss = new Boss(start, Game, VectorMoves.BossMoves(new Vector(start)),
                    EnemyFeatures.EnemyVelocity, EnemyFeatures.BulletVelocity)
            {
                HP = hp,
            };
            this.start = start;
            breakLen = 200;
            phaseCount = 5;
        }

        public GameState Act(GameState state)
        {
            phaseIteration++;
            if (isBreak && phaseIteration > breakLen)
            {
                isBreak = false;
                phaseIteration = 0;
            }
            if (phaseIteration > 350)
            {
                phase++;
                isBreak = true;
                phaseIteration = 0;
            }
            if (phase > phaseCount) phase = 0;
            return base.Act(state, () => needToSpawn, x =>
            {
                x.Add(EnemyFeatures.Boss);
                needToSpawn = false;
            }, true, GenerateShootingDirections());
        }

        public Func<Enemy, ShootingPattern[]> GenerateShootingDirections()
        {
            if (isBreak)
                return x => new ShootingPattern[0];
            var randomPicture = rnd.Next(2, 6);
            var picture = $"boss-bullet-{randomPicture}.png";

            switch (phase)
            {
                case 0: //
                    var random = rnd.Next(0, 2);
                    if (random == 1)
                    {
                        return x => Directions.GetAroundVectors(16).Skip(16).Take(8)
                        .Select(z => new ShootingPattern
                        {
                            Direction = z,
                            BulletImage = picture
                        }).ToArray();
                    }
                    else
                    {
                        return x => Directions.GetAroundVectors(16).Skip(8).Take(8)
                        .Select(z => new ShootingPattern
                        {
                            Direction = z,
                            BulletImage = picture
                        }).ToArray();
                    }

                case 1:
                    return x => Directions.GetAroundVectors(16)
                    .Select(z => new ShootingPattern
                    {
                        Direction = z,
                        BulletImage = picture
                    }).ToArray();

                case 2:
                    return x => new[] {
                        (Game.Player.Location - x.Location).Rotate(Math.PI / 20),
                        Game.Player.Location - x.Location,
                        (Game.Player.Location - x.Location).Rotate(-Math.PI / 20)}
                        .Select(z => new ShootingPattern
                        {
                            Direction = z,
                            BulletImage = picture
                        }).ToArray();

                case 3:
                    return x => Directions.GetAroundVectors(16)
                    .Select(z => new ShootingPattern
                    {
                        Pattern = VectorMoves.FlyAndFall(x.Location, z),
                        Direction = Vector.Zero,
                        BulletImage = picture
                    }).ToArray();

                case 4:
                    return x => Directions.GetAroundVectors(5)
                    .Select(z => new ShootingPattern
                    {
                        Pattern = VectorMoves.FlyAndToTarget(x.Location, z, Game.Player.Location - x.Location),
                        Direction = Vector.Zero,
                        BulletImage = picture
                    }).ToArray();

                default:
                    return x => Directions.GetRow(10, x.Location.Y)
                    .Select(z => new ShootingPattern
                    {
                        Pattern = new VectorMove(z, 10, Game.Player.Location - x.Location),
                        Direction = Vector.Zero,
                        BulletImage = "boss-bullet-1.png"
                    }).ToArray();
            }
        }
    }
}
