using System.Collections.Generic;
using System.Drawing;
using System;
using System.Linq;

namespace Game
{
    public class GameFeatures //мне кажется удобнее хранить все эти штуки в одном месте
    {
        public Size FieldSize;
        public int PlayerHP;
        public int BossHP;
        public double PlayerHitbox;
        public double EnemyHitbox;
        public double BasicStep;
    }

    public class Game
    {
        public static GameFeatures GameFeatures;

        public Queue<IEnemyAct> Level;
        public GameState CurrentGameState;
        public IEnemyAct EnemyPattern;
        public readonly Player Player;

        public int Score;
        //public int Power;
        public int PowerReload;
        public int HPReload;

        public Game(Size size, Point playerStartPosition, double basicStep,
            double playerHitbox = 33 / 2, double enemyHitbox = 16)
        {
            GameFeatures = new GameFeatures
            {
                FieldSize = size,
                PlayerHP = 50,
                BossHP = 300,
                EnemyHitbox = enemyHitbox,
                PlayerHitbox = playerHitbox,
                BasicStep = basicStep
            };

            Player = new Player(playerStartPosition, this);
            CurrentGameState = new GameState { Player = Player };
        }


        public bool InField(Vector vector)
        {
            return vector.X >= 0 && vector.X <= GameFeatures.FieldSize.Width
                 && vector.Y >= 0 && vector.Y <= GameFeatures.FieldSize.Height;
        }

        public void SpawnEnemy(Enemy enemy)
        {
            if (enemy != null)
                CurrentGameState.Enemies.Add(enemy);
        }

        public void UpdateState()
        {
            CurrentGameState.DeadCharacters = UpdateDeadCharacters();
            if (Level == null)
            {
                Level = GetLevel();
                EnemyPattern = Level.Dequeue();
            }
            //else if (EnemyPattern == null) ;
            if (EnemyPattern != null)
            {
                var possibleState = EnemyPattern.Act(CurrentGameState);
                if (possibleState != null)
                    CurrentGameState = possibleState;
                else
                {
                    if (Level.Count == 0)
                    {
                        CurrentGameState.IsOver = true;
                        EnemyPattern = null;
                        return;
                    }
                    EnemyPattern = Level.Dequeue();
                }
            }
            else
                return;// да блин


            var updatedEnemyBullets = UpdateEnemyBullets();
            if (CurrentGameState.IsOver)
            {
                Player.HP--;
                if (Player.HP > 0)
                    CurrentGameState.IsOver = false;
                else
                    return;
            }
            var updatedPlayerBullets = UpdatePlayerBullets();
            var updatedPower = UpdatePower();

            CurrentGameState.PlayerBullets = updatedPlayerBullets;
            CurrentGameState.EnemyBullets = updatedEnemyBullets;
            CurrentGameState.Power = updatedPower;

        }

        private HashSet<Tuple<Vector, int, int>> UpdateDeadCharacters()
        {
            return new HashSet<Tuple<Vector, int, int>>(CurrentGameState.DeadCharacters
                .Select(x => Tuple.Create(x.Item1, x.Item2, x.Item3 + 1)) 
                .Select(x => x.Item3 > 2 ? Tuple.Create(x.Item1, x.Item2 + 1, 0) : x) //тут контроль количества повторений одной итерации
                .Where(x => x.Item2 < 5));//финальная итерация, можно вынести в константу

        }

        public void AddDeadEnemy(Enemy enemy)
        {
            CurrentGameState.DeadCharacters.Add(Tuple.Create(enemy.Location, 0, 0));
        }

        private HashSet<Power> UpdatePower()
        {
            var updatedPower = new HashSet<Power>();
            foreach (var power in CurrentGameState.Power)
            {
                power.Move();
                var playerTouchedPower = false;
                if (CharactersCollided(power, Player, GameFeatures.PlayerHitbox))
                {
                    power.UpdateStats(Player);
                    playerTouchedPower = true;
                }
                if (InField(power.Location) && !playerTouchedPower)
                    updatedPower.Add(power);
            }
            return updatedPower;
        }

        private HashSet<EnemyBullet> UpdateEnemyBullets()
        {
            var updatedEnemyBullets = new HashSet<EnemyBullet>();
            foreach (var bullet in CurrentGameState.EnemyBullets)
            {
                bullet.Move();
                var bulletTouchedPlayer = false;
                if (CharactersCollided(bullet, Player, GameFeatures.PlayerHitbox))
                {
                    //CurrentGameState.DeadCharacters.Add(Tuple.Create(bullet.Location, 0));
                    CurrentGameState.IsOver = true;
                    bulletTouchedPlayer = true;
                }

                if (InField(bullet.Location) && !bulletTouchedPlayer)
                    updatedEnemyBullets.Add(bullet);
            }
            return updatedEnemyBullets;
        }

        public bool CharactersCollided(ICharacter character, ICharacter otherCharacter, double hitbox)
        {
            var start = character.GetPreviousLocation();
            var end = character.GetLocation();
            var otherStart = otherCharacter.GetPreviousLocation();
            var otherEnd = otherCharacter.GetLocation();
            var otherMoved = otherStart != null && otherEnd != null;
            if (start == null || end == null)
                return false;
            var otherMovements = otherMoved
                ? Vector.SplitVector(otherStart, otherEnd)
                : new List<Vector> { otherCharacter.GetLocation() };
            foreach (var ghostCharacter in Vector.SplitVector(start, end))
            {
                foreach (var otherGhostCharacter in otherMovements)
                {
                    if (ghostCharacter.EqualWithInfelicity(otherGhostCharacter, hitbox))
                        return true;
                }
            }
            return false;
        }


        private HashSet<PlayerBullet> UpdatePlayerBullets()
        {
            var aliveEnemies = new HashSet<Enemy>(CurrentGameState.Enemies);
            var updatedPlayerBullets = new HashSet<PlayerBullet>();

            foreach (var bullet in CurrentGameState.PlayerBullets)
            {
                bullet.Move();
                var bulletTouchedEnemy = false;
                foreach (var enemy in aliveEnemies.OrderByDescending(e => e.Location.Y)) //чтобы начинать проверку с ближайшего врага
                {
                    if (!CharactersCollided(bullet, enemy, GameFeatures.EnemyHitbox) || enemy.IsImmortal)
                        continue;
                    //  CurrentGameState.DeadCharacters.Add(Tuple.Create(bullet.Location, 0));
                    bulletTouchedEnemy = true;
                    if (enemy is Boss)
                    {
                        var boss = enemy as Boss;
                        boss.HP--;// не понимаю почему это работает, помогите, я серьезно
                        if (boss.HP <= 0)
                        {
                            CurrentGameState.Enemies.Remove(enemy);
                            AddDeadEnemy(enemy);
                        }
                    }
                    else
                    {
                        AddDeadEnemy(enemy);
                        Score += 100;
                        TryToAddPower(enemy);
                        CurrentGameState.Enemies.Remove(enemy);
                    }
                    break;
                }
                if (InField(bullet.Location) && !bulletTouchedEnemy)
                    updatedPlayerBullets.Add(bullet);
            }
            return updatedPlayerBullets;
        }

        public void TryToAddPower(Enemy enemy)
        {
            if (PowerReload >= 5)
            {
                CurrentGameState.Power.Add(new PowerBar(enemy.Location));
                PowerReload = 0;
            }
            else if (HPReload >= 6)
            {
                CurrentGameState.Power.Add(new HpBar(enemy.Location));
                HPReload = 0;
            }

            PowerReload++;
            HPReload++;
        }

        public Queue<IEnemyAct> GetLevel()
        {
            return Levels.DefaultLevel(this);
        }
    }
}
