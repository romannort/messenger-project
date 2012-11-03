using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace iMessenger.Helpers
{
    public static class MenuItemBuilder
    {
        public static MenuItem Build(String header, RoutedEventHandler clickEvent)
        {
            MenuItem menuItem = new MenuItem
                                    {
                                        IsCheckable = false,
                                        Header = header

                                    };
            menuItem.Click += clickEvent;
            return menuItem;
        }

        
    }
}
