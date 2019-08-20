using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    class ImageSelector
    {
        public static int imageIteration;
        public static int imageFrequency = 20;
        public static int imageIndex = 1;

        public static string ChoosePlayerImage(Player player)
        {
            imageIteration++;
            if (imageIteration == imageFrequency)
            {
                imageIndex++;
                if (imageIndex > 4)
                    imageIndex = 1;
                imageIteration = 0;
            }

            int count;
            switch (player.currentDirection)
            {
                case Direction.Left:
                    player.currentDirection = Direction.NoDirection;
                    count = player.Directions.Where(x => x == Direction.Left).Count();
                    switch (count)
                    {
                        case 0:
                            return "leftLight.png";
                        case 1:
                            return "leftMedium.png";
                        case 2:
                            return "leftHard.png";
                        default:
                            return $"playerLeft{imageIndex}.png";
                    }
                case Direction.Right:
                    player.currentDirection = Direction.NoDirection;
                    count = player.Directions.Where(x => x == Direction.Right).Count();
                    switch (count)
                    {
                        case 0:
                            return "rightLight.png";
                        case 1:
                            return "rightMedium.png";
                        case 2:
                            return "rightHard.png";
                        default:
                            return $"playerRight{imageIndex}.png";
                    }
                default:
                    player.currentDirection = Direction.NoDirection;
                    return $"player{imageIndex}.png";
            }
        }
    }
}
