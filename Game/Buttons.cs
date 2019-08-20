using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Game
{
    class Buttons
    {
        public static Font ButtonFont = new Font(FontFamily.Families[0], 30);
        private static int buttonWidth = 400;
        private static int buttonHeight = 100;
        public static Color RestColor = Color.MediumVioletRed;
        public static Color PushColor = Color.Purple;

        public static Button RetryButton = new Button
        {
            Location = new Point((Game.GameFeatures.FieldSize.Width - buttonWidth) / 2, //не могу поставить в середине, как взять рамера окна?
                (Game.GameFeatures.FieldSize.Height - buttonHeight) / 2 + 27), // у меня получилось с:
            Size = new Size(buttonWidth, buttonHeight),
            Font = ButtonFont,
            Text = "TRY AGAIN",
            ForeColor = RestColor,
            BackColor = Color.Transparent
        };

        public static Button StartButton = new Button
        {
            Location = new Point(
                (Game.GameFeatures.FieldSize.Width - buttonWidth) / 2,
                (Game.GameFeatures.FieldSize.Height - buttonHeight) / 2 - buttonHeight / 2),
            Size = new Size(buttonWidth, buttonHeight),
            Font = ButtonFont,
            Text = "START",
            ForeColor = RestColor,
            BackColor = Color.Transparent
        };

        public static Button ExitButton = new Button
        {
            Location = new Point(
                (Game.GameFeatures.FieldSize.Width - buttonWidth) / 2,
                (Game.GameFeatures.FieldSize.Height - buttonHeight) / 2 + buttonHeight / 2),
            Size = new Size(buttonWidth, buttonHeight),
            Font = ButtonFont,
            Text = "EXIT",
            ForeColor = RestColor,
            BackColor = Color.Transparent
        };




    }
}
