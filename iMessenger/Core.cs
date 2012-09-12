using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iMessenger
{
    class Core
    {
        private MainWindow window;

        public Core()
        {
            window = new MainWindow();
            window.Show();
        }
    }
}
