using System;
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
