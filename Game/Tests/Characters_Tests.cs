using NUnit.Framework;
using System.Linq;
using System.Drawing;
using System;

namespace Game.Tests
{
    [TestFixture]
    class Player_Tests
    {
        Random rnd = new Random();

        public Vector CallMove(Player player, Direction[] steps)
        {
            for (var i = 0; i < steps.Length; i++)
                player.Move(steps[i]);
            return player.Location;
        }

        [Test]
        public void PlayerMoveRightTest()
        {
            var game = new Game(new Size(1, 1), new Point(0, 0), 1);
            var player = game.Player;
            player.Move(Direction.Right);
            Assert.AreEqual(new Vector(1, 0), player.Location);
        }

        [Test]
        public void PlayerMoveSequenceRightDestination()
        {
            var game = new Game(new Size(1, 1), new Point(0, 0), 1);
            var player = game.Player;
            Assert.AreEqual(new Vector(0, 0), CallMove(player,
                new[] { Direction.Right, Direction.Down, Direction.Up, Direction.Left }));
        }

        //нужно будет решить с границами
        [Test]
        public void PlayerMoveInBorders()
        {
            var mapSize = new Size(rnd.Next(500), rnd.Next(600));
            var game = new Game(mapSize, new Point(rnd.Next(mapSize.Width), rnd.Next(mapSize.Height)), 1);
            var player = game.Player;
            while (player.Location.X < Game.GameFeatures.FieldSize.Width)
                player.Move(Direction.Right);
            Assert.AreEqual(player.Location, CallMove(player,
               new Direction[] { Direction.Right }), "Вышел за правую границу");
            while (player.Location.Y < Game.GameFeatures.FieldSize.Height)
                player.Move(Direction.Down);
            Assert.AreEqual(player.Location, CallMove(player,
               new Direction[] { Direction.Down }), "Вышел за нижнюю границу");
            while (player.Location.X > 0)
                player.Move(Direction.Left);
            Assert.AreEqual(player.Location, CallMove(player,
               new Direction[] { Direction.Left }), "Вышел за левую границу");
            while (player.Location.Y > 0)
                player.Move(Direction.Up);
            Assert.AreEqual(player.Location, CallMove(player,
                new Direction[] { Direction.Up }), "Вышел за верхнюю границу");
        }

        [Test]
        public void PlayerShoot()
        {
            var game = new Game(new Size(500, 600), new Point(rnd.Next(500),
                rnd.Next(600)), 1);
            var player = game.Player;
            var bullets = game.CurrentGameState.PlayerBullets;
            var list = bullets.ToList();
            var count = bullets.Count;
            player.Shoot();
            Assert.AreEqual(count + 1, game.CurrentGameState.PlayerBullets.Count,
                "Пуля должна быть добавлена в сет");
            foreach (var item in list)
            {
                Assert.True(bullets.Contains(item), "Ничего не должно быть удалено из сета");
                bullets.Remove(item);
            }
            Assert.AreEqual(player.Location, bullets.First().Location,
                "Начальные координаты пули должны соответствовать начальным координатам игрока");
        }

        [Test]
        public void EnemyShoot()
        {
            var game = new Game(new Size(500, 600), new Point(rnd.Next(500),
                rnd.Next(600)), 1);
            var player = game.Player;
            var bullets = game.CurrentGameState.EnemyBullets;
            var list = bullets.ToList();
            var count = bullets.Count;
            var enemy = new Enemy(new Point(10, 10), game, new Immobile(new Vector(10, 10)), 1, 1);
            enemy.Shoot(new ShootingPattern { Direction = Vector.Up });
            Assert.AreEqual(count + 1, game.CurrentGameState.EnemyBullets.Count, "Пуля должна быть добавлена в сет");
            foreach (var item in list)
            {
                Assert.True(bullets.Contains(item), "Ничего не должно быть удалено из сета");
                bullets.Remove(item);
            };
        }

        [Test]
        public void EnemyShootCorrectDirection()
        {
            var game = new Game(new Size(2, 2), new Point(0, 0), 1, 1e-9, 1e-9);
            var enemy = new Enemy(new Point(2, 2), game, new Immobile(new Vector(2, 2)), 1, 1);
            game.SpawnEnemy(enemy);
            enemy.Shoot(new ShootingPattern { Direction = game.Player.Location - enemy.Location });
            game.UpdateState();
            var bullet = game.CurrentGameState.EnemyBullets.First();
            Assert.AreEqual(Math.PI / 4, bullet.Location.Angle, 1e-9);
        }

        [Test]
        public void ImmortalEnemyCantDieFromPlayerBullet()
        {
            var game = new Game(new Size(2, 2), new Point(0, 1), 1, 1e-9, 1e-9);
            var enemy = new Enemy(new Point(0, 0), game, new Immobile(new Vector(0, 0)), 1, 1,
                "", true);
            game.SpawnEnemy(enemy);
            game.Player.Shoot();
            game.UpdateState();
            Assert.True(game.CurrentGameState.Enemies.Contains(enemy));
        }

        [Test]
        public void ImmortalEnemyCantDieFromPlayerTouch()
        {
            var game = new Game(new Size(2, 2), new Point(0, 1), 1, 1e-9, 1e-9);
            var enemy = new Enemy(new Point(0, 0), game, new Immobile(new Vector(0, 0)), 1, 1,
                "", true);
            game.SpawnEnemy(enemy);
            game.Player.Move(Direction.Up);
            game.UpdateState();
            Assert.True(game.CurrentGameState.Enemies.Contains(enemy));
        }
    }
}
