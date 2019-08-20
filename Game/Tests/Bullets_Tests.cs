using NUnit.Framework;
using System;
using System.Drawing;
using System.Linq;

namespace Game.Tests
{
    class Bullets_Tests
    {
        Random rnd = new Random();

        [Test]
        public void PlayerBulletMove()
        {
            var game = new Game(new Size(500, 600), new Point(0, 0), 1);
            var start = new Vector(rnd.Next(1, Game.GameFeatures.FieldSize.Width - 1),
                rnd.Next(1, Game.GameFeatures.FieldSize.Height - 1));
            var bullet = new PlayerBullet(start, Vector.Up, 1);
            bullet.Move();
            var finish = bullet.Location;
            Assert.AreEqual(start.Y, finish.Y + 1,
                "я думаю тут старт должен быть больше финиша," +
                "потому что пуля движется вверх," +
                "а начало координат в верхнем левом углу"); // я думаю тут старт должен быть больше финиша, потому что пуля движется вверх,
                                                            // а начало координат в верхнем левом углу
            Assert.AreEqual(start.X, finish.X, "Координата икс пули должна быть неизменной");
        }

        [Test]
        public void PlayerBulletsDisappear()
        {
            var game = new Game(new Size(1, 1), new Point(0, 0), 1);
            var player = game.Player;
            player.Shoot();
            game.UpdateState();
            game.UpdateState();
            Assert.Zero(game.CurrentGameState.PlayerBullets.Count);
        }

        [Test]
        public void PlayerBulletsDisappearAfterKillingEnemies()
        {
            var game = new Game(new Size(1, 1), new Point(0, 1), 1, 1e-9);
           // game.PlayerHitbox = 1e-9;
            var player = game.Player;
            var enemy = new Enemy(new Point(0, 0), game, new Immobile(new Vector(0, 0)), 1, 1);
            game.SpawnEnemy(enemy);
            player.Shoot();
            game.UpdateState();
            Assert.Zero(game.CurrentGameState.PlayerBullets.Count);
        }

        [Test]
        public void EnemyBulletsDisappearWhenNotInField()
        {
            var game = new Game(new Size(1, 1), new Point(0, 1), 1);
            var enemy = new Enemy(new Point(0, 0), game, new Immobile(new Vector(0, 0)), 1, 1);
            game.SpawnEnemy(enemy);
            enemy.Shoot(new ShootingPattern { Direction = Vector.Right });
            game.UpdateState();
            game.UpdateState();
            Assert.Zero(game.CurrentGameState.EnemyBullets.Count);
        }

        [Test]
        public void EnemyBulletDisappearAfterHittingPlayer()
        {
            var game = new Game(new Size(2, 2), new Point(0, 2), 1, 1e-9);
           // game.PlayerHitbox = 1e-9;
            var enemy = new Enemy(new Point(0, 1), game, new Immobile(new Vector(0, 1)), 1, 1);
            game.SpawnEnemy(enemy);
            enemy.Shoot(new ShootingPattern { Direction = Vector.Down });
            game.UpdateState();
            Assert.Zero(game.CurrentGameState.EnemyBullets.Count);
        }
    }
}
