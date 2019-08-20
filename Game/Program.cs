using System.Drawing;
using System.Windows.Forms;

namespace Game
{
    class Program
    {
        public static void Main()
        {
            var game = new Game(new Size(500, 600), new Point(250, 600), 5);
            Application.Run(new GameForm(game) { ClientSize = new Size(700, 620) });
        }
    }
}
