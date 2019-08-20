using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Game
{
    public class DialogInfo
    {
        public Point BossStartPoint;
        public Point BossSpawnPoint;
        public string BossName;

        public DialogInfo(Point bossSpawnPoint, Point bossStartPoint, string bossName)
        {
            BossSpawnPoint = bossSpawnPoint;
            BossStartPoint = bossStartPoint;
            BossName = bossName;
        }
    }

    public class Dialog : EnemyPattern, IEnemyAct
    {
        private int phraseIteration;
        public Queue<Tuple<ICharacter, string, string>> Phrases;
        public Tuple<ICharacter, string, string> CurrentPhrase; //тип персонажа, имя персонажа, фраза
        private int phraseLifetime;
        private Boss boss;
        private bool needToSpawn = true;

        public Dialog(Game game, DialogInfo info, string[] playerPhrases, string[] bossPhrases)
        {
            Game = game;
            var count = 0;
            boss = new Boss(info.BossSpawnPoint, game, VectorMoves.MoveToTarget(new Vector(info.BossSpawnPoint),
                new Vector(info.BossStartPoint)), 1, 1, true)
            {
                Name = info.BossName,
                HP = 10
            };
            Phrases = new Queue<Tuple<ICharacter, string, string>>(playerPhrases
                .Zip(bossPhrases, (x, y) => x + "\r" + y)
                .SelectMany(x => x.Split('\r'))
                .ToList()
                .Select(x => Tuple.Create(
                    ++count % 2 == 0 ? (ICharacter)boss : (ICharacter)game.Player,
                    count % 2 == 0 ? boss.Name : game.Player.Name, x)));
            phraseLifetime = 200;
            EnemyFeatures = new EnemyFeatures
            {
                Lifetime = Phrases.Count * phraseLifetime
            };
        }

        public GameState Act(GameState state)
        {
            phraseIteration++;
            if (CurrentPhrase == null && Phrases.Count > 0)
                CurrentPhrase = Phrases.Dequeue();
            if (phraseIteration >= phraseLifetime && Phrases.Count > 0)
            {
                phraseIteration = 0;
                CurrentPhrase = Phrases.Dequeue();
            }
            if (EnemyFeatures.Iteration + 1 == EnemyFeatures.Lifetime)
                state.Enemies.Remove(boss);
            return Act(state, () => needToSpawn, x => {
                x.Add(boss);
                needToSpawn = false;
            }, false, x => new[] { new ShootingPattern {
                Direction = Vector.Down } });
        }
    }
}
