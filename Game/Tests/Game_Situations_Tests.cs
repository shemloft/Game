using NUnit.Framework;
using System.Drawing;
using System.Linq;

namespace Game.Tests
{
    [TestFixture]
    class Game_Situations_Tests
    {

        [Test]
        public void PlayerShouldDieFromEnemyBullet()
        {
            var game = new Game(new Size(1, 1), new Point(0, 0), 1, 1e-9, 1e-9);
            var enemy = new Enemy(new Point(1, 0), game, new Immobile(new Vector(1, 0)), 1, 1);
            var currentPlayerHp = game.Player.HP;
            game.SpawnEnemy(enemy);
            enemy.Shoot(new ShootingPattern { Direction = Vector.Left });
            game.UpdateState();
            Assert.Less(game.Player.HP, currentPlayerHp);
        }

        [Test]
        public void PlayerShouldDieFromEnemyTouch()
        {
            var game = new Game(new Size(1, 1), new Point(0, 0), 1, 1e-9, 1e-9);
            var currentPlayerHp = game.Player.HP;
            game.SpawnEnemy(new Enemy(new Point(1, 0), game, new Immobile(new Vector(1, 0)), 1, 1));
            game.Player.Move(Direction.Right);
            game.UpdateState();
            Assert.Less(game.Player.HP, currentPlayerHp);
        }

        [Test]
        public void EnemyDiesFromPlayerBullet()
        {
            var game = new Game(new Size(1, 1), new Point(0, 1), 1, 1e-9, 1e-9);
            var enemy = new Enemy(new Point(0, 0), game, new Immobile(new Vector(0, 0)), 1, 1);
            game.SpawnEnemy(enemy);
            Assert.True(game.CurrentGameState.Enemies.Contains(enemy));
            game.Player.Shoot();
            game.UpdateState();
            Assert.False(game.CurrentGameState.Enemies.Contains(enemy));
        }

        [Test]
        public void PlayerCanDodgeEnemyBullet()
        {
            var game = new Game(new Size(1, 1), new Point(0, 1), 1, 1e-9, 1e-9);
            var enemy = new Enemy(new Point(0, 0), game, new Immobile(new Vector(0, 0)), 1, 1);
            var currentPlayerHp = game.Player.HP;
            game.SpawnEnemy(enemy);
            enemy.Shoot(new ShootingPattern { Direction = Vector.Down });
            game.Player.Move(Direction.Right);
            game.UpdateState();
            Assert.AreEqual(currentPlayerHp, game.Player.HP);
        }

        [Test]
        public void PlayerMovesEnemyStands()
        {
            var game = new Game(new Size(2, 2), new Point(0, 0), 2, 1e-9, 1e-9);
            var enemy = new Enemy(new Point(1, 0), game, new Immobile(new Vector(1, 0)), 1, 1);
            var currentPlayerHp = game.Player.HP;
            game.SpawnEnemy(enemy);
            game.Player.Move(Direction.Right);
            game.UpdateState();
            Assert.False(game.CurrentGameState.Enemies.Contains(enemy));
            Assert.Less(game.Player.HP, currentPlayerHp);
        }

        [Test]
        public void EnemyMovesPlayerStands()
        {
            var game = new Game(new Size(2, 2), new Point(1, 0), 2, 1e-9, 1e-9);
            var enemy = new Enemy(new Point(0, 0), game,
                new VectorMove(new Vector(0, 0), 1, new[] { Vector.Right }), 2, 1);
            var currentPlayerHp = game.Player.HP;
            game.SpawnEnemy(enemy);
            game.UpdateState();
            Assert.False(game.CurrentGameState.Enemies.Contains(enemy));
            Assert.Less(game.Player.HP, currentPlayerHp);
        }

        [Test]
        public void PlayerMovesThroughEnemyBullet()
        {
            var game = new Game(new Size(2, 2), new Point(0, 0), 2, 1e-9, 1e-9);
            var bullet = new EnemyBullet(new Vector(1, 0), new Vector(0, 0), 0);
            var currentPlayerHp = game.Player.HP;
            game.CurrentGameState.EnemyBullets.Add(bullet);
            game.Player.Move(Direction.Right);
            game.UpdateState();
            Assert.False(game.CurrentGameState.EnemyBullets.Contains(bullet));
            Assert.Less(game.Player.HP, currentPlayerHp);
        }

        [Test]
        public void EnemyBulletMovesThroughPlayer()
        {
            var game = new Game(new Size(2, 2), new Point(1, 0), 2, 1e-9, 1e-9);
            var bullet = new EnemyBullet(new Vector(0, 0), Vector.Right, 2);
            var currentPlayerHp = game.Player.HP;
            game.CurrentGameState.EnemyBullets.Add(bullet);
            game.UpdateState();
            Assert.False(game.CurrentGameState.EnemyBullets.Contains(bullet));
            Assert.Less(game.Player.HP, currentPlayerHp);
        }

        [Test]
        public void PlayerBulletMovesThroughEnemy()
        {
            var game = new Game(new Size(2, 2), new Point(0, 0), 2, 1e-9, 1e-9);
            var enemy = new Enemy(new Point(1, 1), game, new Immobile(new Vector(1, 1)), 1, 1);
            var bullet = new PlayerBullet(new Vector(1, 2), Vector.Up, 2);
            game.SpawnEnemy(enemy);
            game.CurrentGameState.PlayerBullets.Add(bullet);
            game.UpdateState();
            Assert.False(game.CurrentGameState.Enemies.Contains(enemy));
            Assert.False(game.CurrentGameState.PlayerBullets.Contains(bullet), "asd");
        }

        [Test]
        public void EnemyMovesThroughPlayerBullet()
        {
            var game = new Game(new Size(2, 2), new Point(2, 2), 2, 1e-9, 1e-9);
            var enemy = new Enemy(new Point(0, 0), game,
                new VectorMove(new Vector(0, 0), 1, Vector.Right), 2, 1);
            var bullet = new PlayerBullet(new Vector(1, 0), Vector.Up, 0);
            game.SpawnEnemy(enemy);
            game.CurrentGameState.PlayerBullets.Add(bullet);
            game.UpdateState();
            Assert.False(game.CurrentGameState.Enemies.Contains(enemy));
            Assert.False(game.CurrentGameState.PlayerBullets.Contains(bullet), "asd");
        }

        [Test]
        public void BulletKillOneEnemy()
        {
            var game = new Game(new Size(1, 3), new Point(1, 3), 1, 1e-9, 1e-9);
            var enemy1 = new Enemy(new Point(1, 1), game, new Immobile(new Vector(1, 1)), 1, 0);
            var enemy2 = new Enemy(new Point(1, 2), game, new Immobile(new Vector(1, 2)), 1, 0);

            game.SpawnEnemy(enemy1);
            game.SpawnEnemy(enemy2);

            game.Player.Shoot();
            game.UpdateState();

            Assert.AreEqual(1, game.CurrentGameState.Enemies.Count);
            CollectionAssert.Contains(game.CurrentGameState.Enemies, enemy1);
        }

        [Test]
        public void KillingEnemyIncreaseScore()
        {
            var game = new Game(new Size(1, 4), new Point(1, 4), 1, 1e-9, 1e-9);
            var enemy1 = new Enemy(new Point(1, 1), game, new Immobile(new Vector(1, 1)), 1, 0);
            var enemy2 = new Enemy(new Point(1, 2), game, new Immobile(new Vector(1, 2)), 1, 0);
            var enemy3 = new Enemy(new Point(1, 3), game, new Immobile(new Vector(1, 3)), 1, 0);

            game.SpawnEnemy(enemy1);
            game.SpawnEnemy(enemy2);
            game.SpawnEnemy(enemy3);

            for (var i = 0; i < 4; i++)
            {
                game.Player.Reload();
                game.Player.Shoot();
                game.UpdateState();
            }

            Assert.AreEqual(300, game.Score);

        }

        [Test]
        public void SimpleGameSituation()
        {
            var game = new Game(new Size(4, 4), new Point(2, 4), 1, 1e-9, 1e-9);
            var enemy1 = new Enemy(new Point(0, 0), game,
                new VectorMove(new Vector(0, 0), 1, Vector.Down), 1, 0);
            var enemy2 = new Enemy(new Point(2, 0), game,
                new VectorMove(new Vector(2, 0), 1, Vector.Down), 1, 0);
            var enemy3 = new Enemy(new Point(4, 0), game,
                new VectorMove(new Vector(4, 0), 1, Vector.Down), 1, 0);
            game.SpawnEnemy(enemy1);
            game.SpawnEnemy(enemy2);
            game.SpawnEnemy(enemy3);

            game.Player.Shoot();
            game.Player.Move(Direction.Right);
            game.UpdateState();

            game.Player.Move(Direction.Right);
            game.UpdateState();

            game.Player.Reload();
            game.Player.Shoot();
            game.Player.Move(Direction.Left);
            game.UpdateState();

            Assert.AreEqual(1, game.CurrentGameState.Enemies.Count);
            Assert.AreEqual(new Vector(3, 4), game.Player.Location);
            Assert.AreEqual(200, game.Score);
            Assert.Zero(game.CurrentGameState.PlayerBullets.Count);


        }

        [Test]
        public void PlayerCanKillBoss()
        {
            var game = new Game(new Size(1, 1), new Point(0, 1), 1, 1e-9, 1e-9);
            var boss = new Boss(new Point(0, 0), game, new Immobile(Vector.Zero), 1, 1)
            {
                HP = 1
            };
            game.SpawnEnemy(boss);
            game.Player.Shoot();
            game.UpdateState();
            Assert.False(game.CurrentGameState.Enemies.Contains(boss));
        }

        [Test]
        public void PlayerCanHitBossCorrectly()
        {
            var game = new Game(new Size(1, 1), new Point(0, 1), 1, 1e-9, 1e-9);
            var boss = new Boss(new Point(0, 0), game, new Immobile(Vector.Zero), 1, 1)
            {
                HP = 2
            };
            game.SpawnEnemy(boss);
            game.Player.Shoot();
            game.UpdateState();
            Assert.True(game.CurrentGameState.Enemies.Contains(boss), "Он умер");
            Assert.AreEqual(1, boss.HP, "Чет с его хп не так");
        }

        [Test]
        public void BossMovesThroughPlayerBullet()
        {
            var game = new Game(new Size(1, 1), new Point(0, 1), 1, 1e-9, 1e-9);
            var boss = new Boss(new Point(1, 0), game, new VectorMove(new Vector(1, 0), 1, Vector.Left),
                1, 1)
            {
                HP = 2
            };
            game.SpawnEnemy(boss);
            game.CurrentGameState.PlayerBullets.Add(new PlayerBullet(Vector.Zero, Vector.Zero, 1));
            game.UpdateState();
            Assert.True(game.CurrentGameState.Enemies.Contains(boss), "Он умер");
            Assert.AreEqual(1, boss.HP, "Чет с его хп не так");
        }
    }
}
