using NUnit.Framework;
using System.Drawing;
using System.Linq;


namespace Game.Tests
{
    class Characters_collided_Tests
    {
        [Test]
        public void OneMoveThroughOther()
        {
            var game = new Game(new Size(2, 2), new Point(0, 0), 2, 1e-9, 1e-9);
            var enemy = new Enemy(new Point(0, 1), game, new Immobile(new Vector(0, 1)), 1, 1);
            game.Player.Move(Direction.Down);
            Assert.True(game.CharactersCollided(game.Player, enemy, 1e-9));
        }

        [Test]
        public void OneMeetOther()
        {
            var game = new Game(new Size(2, 2), new Point(0, 0), 2, 1e-9, 1e-9);
            var enemy = new Enemy(new Point(0, 2), game, new VectorMove(new Vector(0, 2), 1, Vector.Up), 2, 1);
            game.Player.Move(Direction.Down);
            enemy.Move();
            Assert.True(game.CharactersCollided(game.Player, enemy, 1e-9));
        }
    }
}
