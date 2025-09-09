using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSellBilet.Hardik.Dop
{
    public class AllNeded
    {
        public static async Task Message(string message, Window pap)
        {
            var diolo = new Window
            {
                Title = "Внимание",
                Content = message,
                Width =500,
                Height =300
            };
            await diolo.ShowDialog(pap);
        }

        public static async Task Worning(string message, Window pap)
        {
            var diolo = new Window
            {
                Title = "Ошибка",
                Content = message,
                Width = 500,
                Height = 300
            };
            await diolo.ShowDialog(pap);
        }
    }
}
