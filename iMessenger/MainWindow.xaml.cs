using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace iMessenger
{
    public partial class MainWindow
    {

        public Core Core { get; set; }
        readonly Roster<User> _roster;

        public MainWindow()
        {
            InitializeComponent();
            _roster = new Roster<User>();
        }

        private void ChatLoaded(object sender, RoutedEventArgs e)
        {
            NickBox.Text = Core.UserName;
            Core.SendMessage(GenerateMessage("", MessageType.JoinCommon));
            MessageBox.Focus();
        }

        public void ShowMessage(Message message)
        {
            Dispatcher.Invoke((ThreadStart)delegate
            {
                switch (message.Type)
                {
                    case MessageType.Common:
                        {
                            ShowAt(0, new Run(message.GetMessageString()));
                            break;
                        }
                    case MessageType.Conference:
                        {
                            for (var i = 1; i < Tabs.Items.Count - 1; i++)
                            {
                                if (((TabItem)Tabs.Items[i]).Tag.ToString() == message.ConferenceNumber)
                                {
                                    ShowAt(i, new Run(message.GetMessageString()));
                                    return;
                                }
                            }
                            CreateTab(message);
                            ShowAt(Tabs.Items.Count - 2, new Run(message.GetMessageString()));
                            break;
                        }
                    default:
                        {
                            for (var i = 0; i < Tabs.Items.Count - 1; i++)
                            {
                                var lb = (ListBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[0];
                                foreach (CheckBox item in lb.Items)
                                {
                                    if (item.Content.ToString() == message.SenderName && item.IsChecked == true)
                                    {
                                        ShowAt(i, GenerateSystemRun(message.GetMessageString()));
                                    }
                                }
                            }
                            break;
                        }
                }
            });
        }

        private void ShowAt(int idx, Run run)
        {
            var rtb = (RichTextBox)((Grid)((TabItem)Tabs.Items[idx]).Content).Children[1];
            rtb.Document.Blocks.Add(new Paragraph(run));
            rtb.ScrollToEnd();
        }   

        private Run GenerateSystemRun(String text)
        {
            return new Run
            {
                Text = text, 
                Foreground = Brushes.DarkGreen, 
                FontStyle = FontStyles.Italic
            };
        }

        private Run GenerateErrorRun(String text)
        {
            return new Run
            {
                Text = "### " + text + " ###",
                Foreground = Brushes.Red,
                FontStyle = FontStyles.Oblique
            };
        }

        private void ChatClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Core.SendMessage(GenerateMessage("", MessageType.LeaveCommon));
            Environment.Exit(0x0);
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(MessageBox.Text))
            {
                MessageType messageType = Tabs.SelectedIndex == 0 ? MessageType.Common : MessageType.Conference;
                Core.SendMessage(GenerateMessage(GetMessageText(), messageType));
            }
        }

        private void MessageBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendButtonClick(sender, new RoutedEventArgs());
        }
        
        private String GetMessageText()
        {
            if (!String.IsNullOrEmpty(MessageBox.Text))
            {
                String data = MessageBox.Text;
                MessageBox.Clear();
                MessageBox.Focus();
                return data;
            }  
            return null;
        }

        public Message GenerateMessage(String data, MessageType type)
        {
            return new Message
            {
                SenderName = Core.UserName,
                SenderIP = Core.UserIP,
                Text = data,
                Type = type,
                Receivers = GetReceiversList(),
                ConferenceNumber = GetConferenceNumber()
            };
        }
        
        private void NickBoxLostFocus(object sender, RoutedEventArgs e)
        {
            NicknameChanging();
        }
        
        private void NicknameChanging()
        {
            if (!String.IsNullOrEmpty(NickBox.Text) && NickBox.Text != Core.UserName)
            {
                if (_roster.IndexOf(new User(NickBox.Text)) == -1)
                {
                    Core.SendMessage(GenerateMessage(NickBox.Text, MessageType.ChangeName));
                    Core.UserName = NickBox.Text;
                    return;
                }
                Dispatcher.Invoke((ThreadStart)(() => ShowAt(Tabs.SelectedIndex, GenerateErrorRun("This nickname is already in use!"))));
            }
            NickBox.Text = Core.UserName;
        }

        public void AddAtConnectList(string newNick)
        {
            Dispatcher.Invoke((ThreadStart)delegate
            {
                _roster.Add(new User(newNick));

                for (var i = 0; i < Tabs.Items.Count-1; i++)
                {
                    var item = new CheckBox
                    {
                        Content = newNick,
                        IsChecked = i == 0,
                        Foreground = RandomColor()
                    };
                    ((ListBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[0]).Items.Add(item);
                }
            });
        }

        public SolidColorBrush RandomColor()
        {
            var rgb = new byte[3];
            var r = new Random();
            r.NextBytes(rgb);
            for (var i = 0; i < 3; i++)
                rgb[i] = (byte)(rgb[i] % 128);

            return new SolidColorBrush(Color.FromRgb(rgb[0], rgb[1], rgb[2]));
        }

        public void ChangeConnectList(string oldNick, string newNick)
        {
            Dispatcher.Invoke((ThreadStart)delegate
            {
                var idx = _roster.IndexOf(new User(oldNick));
                _roster.Change(new User(oldNick), new User(newNick));

                for (var i = 0; i < Tabs.Items.Count-1; i++)
                {
                    var lb = ((ListBox) ((Grid) ((TabItem) Tabs.Items[i]).Content).Children[0]);
                    var toChange = (CheckBox)lb.Items[idx];
                    lb.Items.RemoveAt(idx);
                    toChange.Content = newNick;
                    lb.Items.Insert(idx, toChange);
                }
            });
        }

        public void ReplaceConnectList(string oldNick)
        {
            Dispatcher.Invoke((ThreadStart)delegate
            {
                var idx = _roster.IndexOf(new User(oldNick));
                _roster.Delete(new User(oldNick));

                for (var i = 0; i < Tabs.Items.Count - 1; i++)
                {
                   ((ListBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[0]).Items.RemoveAt(idx);
                }
            });
        }

        private void NickBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            NicknameChanging();
            MessageBox.Focus();
        }

        private List<String> GetReceiversList()
        {
            var receiversList = new List<String>();
            Dispatcher.Invoke((ThreadStart)delegate
            {
                foreach (CheckBox a in ((ListBox)((Grid)((TabItem)Tabs.Items[Tabs.SelectedIndex]).Content).Children[0]).Items)
                {
                    if (a.IsChecked == true)
                    {
                        receiversList.Add(a.Content.ToString());
                    }
                }   
            });
            return receiversList;
        }

        private String GetConferenceNumber()
        {
            var res = "";
            Dispatcher.Invoke((ThreadStart)delegate
            {
                res = ((TabItem)Tabs.SelectedItem).Tag.ToString();
            });
            return res;
        }

        private void TabsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tabs.SelectedIndex != Tabs.Items.Count - 1) return;
            CreateTab(null);
            Tabs.SelectedIndex -= 1; // Set  current selected tab to new created tab.
        }

        private void CreateTab(Message message)
        {
            var grid = new Grid
            {
                Margin = new Thickness(0, -1, 0, 26)
            };

            var rtb = new RichTextBox
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, -24),
                Width = 417,
                AllowDrop = false,
                IsReadOnly = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                AcceptsReturn = false,
                IsUndoEnabled = false,
                Resources = ChatArea0.Resources
            };

            var lb = new ListBox
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(424, 0, 0, -24),
                Width = 204
            };

            foreach (CheckBox cb in ConnectList0.Items)
            {

                lb.Items.Add(new CheckBox
                {
                    Content = cb.Content,
                    Foreground = cb.Foreground,
                    IsChecked = true,
                    IsEnabled = false
                });
            }

            grid.Children.Add(lb);
            grid.Children.Add(rtb);

            var tag = (message == null ? DateTime.Now.ToString("ddHHmmss") : message.ConferenceNumber);
            var tabItem = new TabItem
            {
                Background = ((TabItem)Tabs.Items[0]).Background,
                Foreground = ((TabItem)Tabs.Items[0]).Foreground,
                Name = "TabItem" + (Tabs.Items.Count - 1),
                Tag = "Conference#" + tag,
                Content = grid
            };
            tabItem.Header = SetHeader(tabItem);
            tabItem.MouseDoubleClick += TabItemMouseDoubleClick;
            Tabs.Items.Insert(Tabs.Items.Count - 1, tabItem);
        }

        private ContentControl SetHeader(HeaderedContentControl tabItem)
        {
            var popupMenu = new ContextMenu
            {
                Foreground = new SolidColorBrush(Colors.DarkRed),
                Name = "PopupMenu" + tabItem.Name.Replace("TabItem", "")
            };

            var rename = new MenuItem
            {
                IsCheckable = false, 
                Header = "Rename"
            };
            rename.Click += ContextMenu_OnRenameClick;
            popupMenu.Items.Add(rename);

            var close = new MenuItem
            {
                IsCheckable = false, 
                Header = "Close"
            };
            close.Click += ContextMenu_OnCloseClick;
            popupMenu.Items.Add(close);

            return new ContentControl
            {
                Content = tabItem.Tag,
                ContextMenu = popupMenu
            };
        }

        private void ContextMenu_OnCloseClick(object menuItem, RoutedEventArgs e)
        {
            TabItem toDelete = null;
            for (var i = 1; i < Tabs.Items.Count - 1; i++)
                if (((TabItem)Tabs.Items[i]).Name == ((ContextMenu)((MenuItem)menuItem).Parent).Name.Replace("PopupMenu", "TabItem"))
                {
                    toDelete = (TabItem)Tabs.Items[i];
                    //Core.SendMessage(GenerateMessage(ToDelete.Name.ToString(), MessageType.LeaveConference));
                    break;
                }
            if( Equals(Tabs.SelectedItem, toDelete))
            {
                Tabs.SelectedIndex--;
            }
            if (toDelete != null)
            {
                Tabs.Items.Remove(toDelete);
            }
        }

        #region Rename conference

        private void ContextMenu_OnRenameClick(object menuItem, RoutedEventArgs e)
        {
            TabItem renameItem = Tabs.Items.Cast<TabItem>().First(item => item.Name == ((ContextMenu)((MenuItem)menuItem).Parent).Name.Replace("PopupMenu", "TabItem"));
            inRenameState = Tabs.SelectedIndex;
            CreateRenameBox(renameItem);
        }

        private void ChangeConferenceName(String text)
        {
            if (CheckUnique(text))
            {
                SetHeader(text);
            }
            else
            {
                ShowAt(inRenameState, GenerateErrorRun("Conference with such name is already exist!"));
                Tabs.SelectedIndex = inRenameState;
                ThreadPool.QueueUserWorkItem(a =>
                {
                    Thread.Sleep(10);
                    Dispatcher.Invoke(new Action(() => ((TextBox)((ContentControl)((TabItem)Tabs.Items[inRenameState]).Header).Content).Focus()));
                });
            }
        }

        private void SetHeader(String text)
        {
            ((TabItem)Tabs.Items[inRenameState]).Header =
                ((TextBox)((ContentControl)((TabItem)Tabs.Items[inRenameState]).Header).Content).Tag;
            ((ContentControl)((TabItem)Tabs.Items[inRenameState]).Header).Content = text;
        }

        private bool CheckUnique(String newName)
        {
            foreach (TabItem item in Tabs.Items)
                if ((item.Header.GetType().ToString().Contains("ContentControl")
                    && ((ContentControl)item.Header).Content.ToString() == newName)
                    && item.Equals(Tabs.Items[inRenameState])
                    || newName == "Common" || newName == "+")
                        return false;
            return true;
        }

        private void OnRenameBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            MessageBox.Focus();
        }

        private void TabItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!((ContentControl)sender).Content.GetType().ToString().Contains("TextBox"))
            {
                inRenameState = Tabs.SelectedIndex;
                CreateRenameBox((ContentControl)sender);
            }
        }

        private void CreateRenameBox(ContentControl sender)
        {
            var renameBox = new TextBox
            {
                SelectedText = sender.Content.ToString(),
                Tag = sender,
            };
            renameBox.KeyDown += OnRenameBoxKeyDown;
            renameBox.LostFocus += OnRenameBoxLostFocus;
            sender.Content = renameBox;
            ThreadPool.QueueUserWorkItem(a =>
                  {
                      Thread.Sleep(10);
                      renameBox.Dispatcher.Invoke(new Action(() => renameBox.Focus()));
                  });
        }

        private void OnRenameBoxLostFocus(object sender, RoutedEventArgs e)
        {
            ChangeConferenceName(((TextBox)sender).Text);
        }

        #endregion
    }
    
}
