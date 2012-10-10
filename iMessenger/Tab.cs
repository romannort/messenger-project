using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace iMessenger
{
    public class Tab
    {
        public Grid grid;
        public ListBox lb;
        public RichTextBox rtb;
        public String Name;

        // Передавать String вместо Message !
        public Tab(Message m, ListBox ConList, RichTextBox ChatArea)
        {
            grid = new Grid()
            {
                Margin = new Thickness(0, -1, 0, 26)
            };

            rtb = new RichTextBox()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, -24),
                Width = 417,
                AllowDrop = false,
                IsReadOnly = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                AcceptsReturn = false,
                IsUndoEnabled = false,
                Resources = ChatArea.Resources
            };

            lb = new ListBox()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(424, 0, 0, -24),
                Width = 204
            };

            foreach (CheckBox cb in ConList.Items)
            {

                lb.Items.Add(new CheckBox()
                {
                    Content = cb.Content,
                    Foreground = cb.Foreground,
                    IsChecked = true,
                    IsEnabled = false
                });
            }

            grid.Children.Add(lb);
            grid.Children.Add(rtb);
            // Передавать String вместо Message
            if (m != null)
                Name = m.Type == MessageType.Common ? "Common" : m.Text.Remove(8);
            else
                Name = DateTime.Now.ToString("ddHHmmss");
        }
    }
}
