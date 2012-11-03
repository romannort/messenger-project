using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace iMessenger.Helpers
{
    public static class RunBuilder
    {
        public static Run SystemRun(String text)
        {
            return new Run
            {
                Text = text,
                Foreground = Brushes.DarkGreen,
                FontStyle = FontStyles.Italic
            };
        }

        public static Run SystemRun( Message message)
        {
            return SystemRun(message.GetMessageString());
        }

        public static Run ErrorRun(String text)
        {
            return new Run
            {
                Text = "### " + text + " ###",
                Foreground = Brushes.Red,
                FontStyle = FontStyles.Oblique
            };
        }

        public static Run ErrorRun(Message message)
        {
            return ErrorRun(message.GetMessageString());
        }

        public static Run DefaultRun(String text)
        {
            return new Run(text);
        }

        public static Run DefaultRun(Message message)
        {
            return (new Run(message.GetMessageString()));
        }
    }
}
