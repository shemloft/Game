using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class GameState
    {
        public HashSet<EnemyBullet> EnemyBullets;
        public HashSet<Enemy> Enemies;
        public HashSet<PlayerBullet> PlayerBullets;
        public HashSet<Power> Power;
        public HashSet<Tuple<Vector, int, int>> DeadCharacters { get; set; } //место, итерация и счетчик повторений взрыва
        public Player Player;
        public bool IsOver;

        public GameState()
        {
            PlayerBullets = new HashSet<PlayerBullet>();
            Enemies = new HashSet<Enemy>();
            EnemyBullets = new HashSet<EnemyBullet>();
            DeadCharacters = new HashSet<Tuple<Vector, int, int>>();
            Power = new HashSet<Power>();
        }

        public List<ICharacter> GetAllCharacters()
        {
            var result = new List<ICharacter>();
            result = result
                .Concat(EnemyBullets)
                .Concat(PlayerBullets)
                .Concat(Enemies)
                .Concat(Power)
                .ToList();
            result.Add(Player);
            return result;
        }
    }
}
