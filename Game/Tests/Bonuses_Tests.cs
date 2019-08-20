using NUnit.Framework;
using System;
using System.Drawing;
using System.Linq;

namespace Game.Tests
{
    class Bonuses_Tests
    {
        [Test]
        public void BonusMustFall()
        {
            var game = new Game(new Size(1, 1), new Point(0, 1), 1, 1e-9, 1e-9);
            var bonus = new Power(new Vector(0, 0), x => x.Power++, "");
            game.CurrentGameState.Power.Add(bonus);
            game.UpdateState();
            Assert.Zero(bonus.Location.X);
            Assert.Greater(bonus.Location.Y, 0);
        }

        [Test]
        public void BonusMustDisappearAfterPlayerTouch()
        {
            var game = new Game(new Size(10, 10), new Point(0, 1), 1, 1e-9, 1e-9);
            var bonus = new Power(new Vector(0, 0), x => x.Power++, "");
            game.CurrentGameState.Power.Add(bonus);
            game.UpdateState();
            Assert.False(game.CurrentGameState.Power.Contains(bonus));
        }

        [Test]
        public void BonusMustDisappearWhenNotInField()
        {
            var game = new Game(new Size(1, 1), new Point(1, 1), 1, 1e-9, 1e-9);
            var bonus = new Power(new Vector(0, 1), x => x.Power++, "");
            game.CurrentGameState.Power.Add(bonus);
            game.UpdateState();
            Assert.False(game.CurrentGameState.Power.Contains(bonus));
        }

        [Test]
        public void BonusAffectsPlayer()
        {
            var game = new Game(new Size(10, 10), new Point(0, 1), 1, 1e-9, 1e-9);
            var currentPlayerHP = game.Player.HP;
            var bonus = new Power(new Vector(0, 0), x => x.HP++, "");
            game.CurrentGameState.Power.Add(bonus);
            game.UpdateState();
            Assert.Greater(game.Player.HP, currentPlayerHP);
        }

        [Test]
        public void PowerBarGivesPower()
        {
            var game = new Game(new Size(1, 1), new Point(0, 1), 1, 1e-9, 1e-9);
            var currentPlayerPower = game.Player.Power;
            game.CurrentGameState.Power.Add(new PowerBar(new Vector(0, 0)));
            game.UpdateState();
            Assert.Greater(game.Player.Power, currentPlayerPower);
        }

        [Test]
        public void HpBarGivesHP()
        {
            var game = new Game(new Size(1, 1), new Point(0, 1), 1, 1e-9, 1e-9);
            var currentPlayerPower = game.Player.HP;
            game.CurrentGameState.Power.Add(new HpBar(new Vector(0, 0)));
            game.UpdateState();
            Assert.Greater(game.Player.HP, currentPlayerPower);
        }

        [Test]
        public void BonusMovesThroughPlayer()
        {
            var game = new Game(new Size(5, 5), new Point(0, 2), 2, 1e-9, 1e-9);
            var currentPlayerPower = game.Player.Power;
            game.CurrentGameState.Power.Add(new PowerBar(new Vector(0, 1)));
            game.UpdateState();
            Assert.Greater(game.Player.Power, currentPlayerPower);
        }

        [Test]
        public void PlayerMovesThroughBonus()
        {
            var game = new Game(new Size(5, 5), new Point(0, 2), 2, 1e-9, 1e-9);
            var currentPlayerPower = game.Player.Power;
            game.CurrentGameState.Power.Add(new PowerBar(new Vector(0, 0)));
            game.Player.Move(Direction.Up);
            game.UpdateState();
            Assert.Greater(game.Player.Power, currentPlayerPower);
        }
    }
}
