using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game
{
    class Levels
    {
        public static DialogInfo DefaultLevelDialogInfo = new DialogInfo(new Point(250, 0),
            new Point(250, 120), "Gay-Nazi from anime");

        public static Func<Game, Queue<IEnemyAct>> DefaultLevel = game =>
        {
            return new Queue<IEnemyAct>(new List<IEnemyAct> {
                new EnemyPatternNothing(game, 100),
                new EnemyPatternBombing(game, 9,
                new EnemyFeatures
                {
                    BulletVelocity = 4,
                    EnemyVelocity = 2,
                    Lifetime = 150,
                    ShootingFrequency = 30
                }, x => new[]{ new ShootingPattern { Direction = Vector.Down } }),
                new EnemyPatternNothing(game, 100),
                new EnemyPatternHalfCircle(new Point(200, 0), new Point(300, 0), game,
                    x => new[]{ new ShootingPattern { Direction = game.Player.Location - x.Location } }),
                new EnemyPatternNothing(game, 80),
                new EnemyPatternExplosion(game, 4, 30, 2),
                new EnemyPatternNothing(game, 100),
                new EnemyPatternDiagonal(game, new Point(100, 0), new Point(400, 0)),
                new EnemyPatternNothing(game, 300),
                new EnemyPatternBombing(game, 4,
                    new EnemyFeatures
                    {
                        BulletVelocity = 3,
                        EnemyVelocity = 2,
                        Lifetime = 150,
                        ShootingFrequency = 10
                    }, x => new[]{ new ShootingPattern { Direction = game.Player.Location - x.Location } }),
                new EnemyPatternNothing(game, 200),
                new EnemyPatternHalfCircle(new Point(200, 0), new Point(300, 0), game,
                    x => new[] {
                        (game.Player.Location - x.Location).Rotate(Math.PI / 6),
                        game.Player.Location - x.Location,
                        (game.Player.Location - x.Location).Rotate(-Math.PI / 6)}
                    .Select(z => new ShootingPattern { Direction = z }).ToArray()),
                new EnemyPatternNothing(game, 200),
                new Dialog(game, DefaultLevelDialogInfo,
                    Phrases.DefaultLevelPlayerPhrases, Phrases.DefaultLevelBossPhrases),
                new BossFight(game, new EnemyFeatures
                {
                    BulletVelocity = 3,
                    EnemyVelocity = 2,
                    ShootingFrequency = 40
                }, DefaultLevelDialogInfo.BossStartPoint, 300),
                new EnemyPatternNothing(game, 200)
            });
        };
    }
}
