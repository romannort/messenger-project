using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using iMessenger.Helpers;

namespace iMessenger
{
    /// <summary>
    /// interaction logic for applcation window
    /// </summary>
    public partial class MainWindow
    {

        /// <summary>
        /// Reference to application Core
        /// </summary>
        public Core Core { get; set; }
        private readonly Roster<User> _roster;
        private int _inRenameState;

        /// <summary>
        /// Default constructor.
        /// Initializes all components of window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _roster = new Roster<User>();
        }

        /// <summary>
        /// Method calling after start of application
        /// </summary>
        /// <param name="sender"> Pointer at application </param>
        /// <param name="e"> RoutedEvent arguments </param>
        private void ChatLoaded(object sender, RoutedEventArgs e)
        {
            NickBox.Text = Core.User.Name;
            Core.SendMessage(GenerateMessage(String.Empty, MessageType.JoinCommon));
            MessageBox.Focus();
        }

        /// <summary>
        /// Prints message text in message area.
        /// </summary>
        /// <param name="message"> Message object to print </param>
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
                            for (int i = 1; i < Tabs.Items.Count - 1; i++)
                            {
                                if (((TabItem)Tabs.Items[i]).Tag.ToString() == message.ConferenceNumber)
                                {
                                    ShowAt(i, RunBuilder.DefaultRun(message));
                                    return;
                                }
                            }
                            CreateTab(message);
                            ShowAt(Tabs.Items.Count - 2, RunBuilder.DefaultRun(message));
                            break;
                        }
                    default:
                        {
                            for (int i = 0; i < Tabs.Items.Count - 1; i++)
                            {
                                ListBox lb = (ListBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[0];
                                foreach (CheckBox item in lb.Items)
                                {
                                    if (item.Content.ToString() == message.SenderName && item.IsChecked == true)
                                    {
                                        ShowAt(i, RunBuilder.SystemRun(message));
                                    }
                                }
                            }
                            break;
                        }
                }
            });
        }

        /// <summary>
        /// Shows text at given tab
        /// </summary>
        /// <param name="idx"> Tab index </param>
        /// <param name="run"> Parametirized row for showing </param>
        private void ShowAt(int idx, Run run)
        {
            RichTextBox rtb = (RichTextBox)((Grid)((TabItem)Tabs.Items[idx]).Content).Children[1];
            rtb.Document.Blocks.Add(new Paragraph(run));
            rtb.ScrollToEnd();
        }

        /// <summary>
        /// Operations on chat closing 
        /// </summary>
        /// <param name="sender"> Pointer at application </param>
        /// <param name="e"> CancelEvent arguments </param>
        private void ChatClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Core.SendMessage(GenerateMessage(String.Empty, MessageType.LeaveCommon));
            Environment.Exit(0x0);
        }

        /// <summary>
        /// Processing click at SendButton
        /// </summary>
        /// <param name="sender"> POinter at SendButton </param>
        /// <param name="e"> RoutedEvent arguments </param>
        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(MessageBox.Text))
            {
                MessageType messageType = Tabs.SelectedIndex == 0 ? MessageType.Common : MessageType.Conference;
                Core.SendMessage(GenerateMessage(GetMessageText(), messageType));
            }
        }

        /// <summary>
        /// Processing keystrokes at MessageBox
        /// </summary>
        /// <param name="sender"> Pointer at MessageBox </param>
        /// <param name="e"> KeyEvent arguments </param>
        private void MessageBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendButtonClick(sender, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Gets message text from MessageBox
        /// </summary>
        /// <returns> Message as string </returns>
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


        /// <summary>
        /// Generates Message from text data and sets appropriate MessageType for it.
        /// </summary>
        /// <param name="data"> Message text </param>
        /// <param name="type"> Message type. </param>
        /// <returns>New Message object with all fields set. </returns>
        public Message GenerateMessage(String data, MessageType type)
        {
            return new Message
            {
                SenderName = Core.User.Name,
                SenderIP = Core.User.IP,
                Text = data,
                Type = type,
                Receivers = GetReceiversList(),
                ConferenceNumber = GetConferenceNumber()
            };
        }

        /// <summary>
        /// Call changing of nickname.
        /// </summary>
        private void NickBoxLostFocus(object sender, RoutedEventArgs e)
        {
            NicknameChanging();
        }

        /// <summary>
        /// Changes nickName
        /// </summary>
        private void NicknameChanging()
        {
            if (!String.IsNullOrEmpty(NickBox.Text) && NickBox.Text != Core.User.Name)
            {
                if (_roster.IndexOf(new User(NickBox.Text)) == -1)
                {
                    Core.SendMessage(GenerateMessage(NickBox.Text, MessageType.ChangeName));
                    Core.User.Name = NickBox.Text;
                    return;
                }
                //Dispatcher.Invoke((ThreadStart)(() => ShowAt(Tabs.SelectedIndex, RunBuilder.ErrorRun("This nickname is already in use!"))));
                throw new Exception("This nickname is already in use!");
            }
            NickBox.Text = Core.User.Name;
        }

        /// <summary>
        /// Adds line with user nickname to Connection list.
        /// </summary>
        /// <param name="newNick">String with user nickname.</param>
        public void AddToConnectList(string newNick)
        {
            Dispatcher.Invoke((ThreadStart)delegate
            {
                _roster.Add(new User(newNick));

                for (int i = 0; i < Tabs.Items.Count - 1; i++)
                {
                    CheckBox item = new CheckBox
                    {
                        Content = newNick,
                        IsChecked = i == 0,
                        Foreground = RandomColor()
                    };
                    ((ListBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[0]).Items.Add(item);
                }
            });
        }

        /// <summary>
        /// Generates random color.
        /// </summary>
        private static SolidColorBrush RandomColor()
        {
            Byte[] rgb = new Byte[3];
            Random r = new Random();
            r.NextBytes(rgb);
            for (int i = 0; i < 3; i++)
            {
                rgb[i] = (byte)(rgb[i] % 128);
            }

            return new SolidColorBrush(Color.FromRgb(rgb[0], rgb[1], rgb[2]));
        }

        /// <summary>
        /// Replaces the old entry in the Connection list to the new
        /// </summary>
        /// <param name="oldNick"> Old nickname </param>
        /// <param name="newNick"> New nickname </param>
        public void ChangeConnectList(string oldNick, string newNick)
        {
            Dispatcher.Invoke((ThreadStart)delegate
            {
                int idx = _roster.IndexOf(new User(oldNick));
                _roster.Change(new User(oldNick), new User(newNick));

                for (int i = 0; i < Tabs.Items.Count - 1; i++)
                {
                    ListBox lb = ((ListBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[0]);
                    CheckBox toChange = (CheckBox)lb.Items[idx];
                    lb.Items.RemoveAt(idx);
                    toChange.Content = newNick;
                    lb.Items.Insert(idx, toChange);
                }
            });
        }

        /// <summary>
        /// Remove the entry from the Connection list
        /// </summary>
        /// <param name="oldNick"> Old nickname </param>
        public void ReplaceConnectList(string oldNick)
        {
            Dispatcher.Invoke((ThreadStart)delegate
            {
                int idx = _roster.IndexOf(new User(oldNick));
                _roster.Delete(new User(oldNick));

                for (int i = 0; i < Tabs.Items.Count - 1; i++)
                {
                    ((ListBox)((Grid)((TabItem)Tabs.Items[i]).Content).Children[0]).Items.RemoveAt(idx);
                }
            });
        }

        /// <summary>
        /// Processing of keystrokes in NickBox
        /// </summary>
        /// <param name="sender"> Pointer at NickBox </param>
        /// <param name="e"> Key event arguments </param>
        private void NickBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }
            NicknameChanging();
            MessageBox.Focus();
        }

        /// <summary>
        /// Gets list of receivers from Connection list
        /// </summary>
        /// <returns> List of receivers. </returns>
        private List<String> GetReceiversList()
        {
            List<string> receiversList = new List<String>();
            Dispatcher.Invoke((ThreadStart)(() =>
                                            receiversList.AddRange(
                                                from CheckBox a in
                                                    ((ListBox) ((Grid) ((TabItem) Tabs.Items[Tabs.SelectedIndex]).Content).Children[0]).Items
                                                where a.IsChecked == true
                                                select a.Content.ToString())));
            return receiversList;
        }

        /// <summary>
        /// Gets conference number from current tab
        /// </summary>
        /// <returns> Conference number as a string </returns>
        private String GetConferenceNumber()
        {
            string res = "";
            Dispatcher.Invoke((ThreadStart)delegate
            {
                res = ((TabItem)Tabs.SelectedItem).Tag.ToString();
            });
            return res;
        }

        /// <summary>
        /// If selected the last tab creates a new 
        /// </summary>
        /// <param name="sender">Pointer at current tab</param>
        /// <param name="e">SelectionChangedEvent arguments</param>
        private void TabsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tabs.SelectedIndex != Tabs.Items.Count - 1)
            {
                return;
            }
            CreateTab(null);
            Tabs.SelectedIndex -= 1;
        }

        /// <summary>
        /// Creates a new tab
        /// </summary>
        /// <param name="message"> Received message </param>
        private void CreateTab(Message message)
        {
            Grid grid = new Grid
            {
                Margin = new Thickness(0, -1, 0, 26)
            };
            RichTextBox rtb = new RichTextBox
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

            ListBox lb = new ListBox
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

            String tag = message == null ? DateTime.Now.ToString("ddHHmmss") : message.ConferenceNumber;
            TabItem tabItem = new TabItem
            {
                Background = ((TabItem)Tabs.Items[0]).Background,
                Foreground = ((TabItem)Tabs.Items[0]).Foreground,
                Name = "TabItem" + (Tabs.Items.Count - 1),
                Tag = "Conference#" + tag,
                Content = grid
            };
            SetHeader(tabItem);
            Tabs.Items.Insert(Tabs.Items.Count - 1, tabItem);
        }

        /// <summary>
        /// Sets a header content and context menu
        /// </summary>
        /// <param name="tabItem"> Pointer at tab </param>
        private void SetHeader(TabItem tabItem)
        {
            ContextMenu popupMenu = new ContextMenu
            {
                Foreground = new SolidColorBrush(Colors.DarkRed),
                Name = "PopupMenu" + tabItem.Name.Replace("TabItem", "")
            };

            popupMenu.Items.Add(MenuItemBuilder.Build("Rename", ContextMenu_OnRenameClick));
            popupMenu.Items.Add(MenuItemBuilder.Build("Close", ContextMenu_OnCloseClick));

            tabItem.Header = new ContentControl
            {
                Content = tabItem.Tag,
                ContextMenu = popupMenu
            };

            ((ContentControl)tabItem.Header).MouseDoubleClick += TabItemMouseDoubleClick;
        }

        /// <summary>
        /// On Context menu close click method 
        /// </summary>
        /// <param name="menuItem"> Pointer at tab </param>
        /// <param name="e"> RoutedEvent arguments </param>
        private void ContextMenu_OnCloseClick(object menuItem, RoutedEventArgs e)
        {
            TabItem toDelete = null;
            for (int i = 1; i < Tabs.Items.Count - 1; i++)
            {
                if (((TabItem) Tabs.Items[i]).Name ==
                    ((ContextMenu) ((MenuItem) menuItem).Parent).Name.Replace("PopupMenu", "TabItem"))
                {
                    toDelete = (TabItem) Tabs.Items[i];
                    break;
                }
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

        /// <summary>
        /// On Context menu rename click method 
        /// </summary>
        /// <param name="menuItem"> Pointer at tab </param>
        /// <param name="e"> RoutedEvent arguments </param>
        private void ContextMenu_OnRenameClick(object menuItem, RoutedEventArgs e)
        {
            TabItem renameItem = Tabs.Items.Cast<TabItem>().First(item => item.Name == ((ContextMenu)((MenuItem)menuItem).Parent).Name.Replace("PopupMenu", "TabItem"));
            Tabs.SelectedItem = renameItem;
            _inRenameState = Tabs.SelectedIndex;
            CreateRenameBox((ContentControl)renameItem.Header);
        }

        /// <summary>
        /// Creates TextBox for rename tab
        /// </summary>
        /// <param name="sender"> Pointer at tab header </param>
        private void CreateRenameBox(ContentControl sender)
        {
            TextBox renameBox = new TextBox
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

        /// <summary>
        /// Changes conference name
        /// </summary>
        /// <param name="text"> New conference name </param>
        private void ChangeConferenceName(String text)
        {
            if (CheckUnique(text))
            {
                ResetHeader(text);
            }
            else
            {
                //ShowAt(_inRenameState, RunBuilder.ErrorRun("Conference with such name is already exist!"));

                Tabs.SelectedIndex = _inRenameState;
                SetHeader((TabItem)Tabs.SelectedItem);

                throw new Exception("Conference with such name already exist.");
            }
        }

        /// <summary>
        /// Resets header caption
        /// </summary>
        /// <param name="text"> New header caption </param>
        private void ResetHeader(String text)
        {
            ((TabItem)Tabs.Items[_inRenameState]).Header =
                ((TextBox)((ContentControl)((TabItem)Tabs.Items[_inRenameState]).Header).Content).Tag;
            ((ContentControl)((TabItem)Tabs.Items[_inRenameState]).Header).Content = text;
        }

        /// <summary>
        /// Checks the name for uniqueness
        /// </summary>
        /// <param name="newName"> New name </param>
        /// <returns> True if name is unique. Else false. </returns>
        private bool CheckUnique(String newName)
        {
            if (newName == "Common" || newName == "+")
            {
                return false;
            }
            foreach (TabItem item in Tabs.Items)
            { 
                if (item.Header.GetType().ToString().Contains("ContentControl")
                    && (((ContentControl)item.Header).Content.ToString() == newName)
                    && !item.Equals(Tabs.Items[_inRenameState]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Processing of keystrokes
        /// </summary>
        /// <param name="sender"> Pointer at RenameBox </param>
        /// <param name="e"> KeyEvent arguments </param>
        private void OnRenameBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }
            MessageBox.Focus();
        }

        /// <summary>
        /// Processing double click at tab header
        /// </summary>
        /// <param name="sender"> Pointer at header </param>
        /// <param name="e"> MouseButtonEvent arguments </param>
        private void TabItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!((ContentControl)sender).Content.GetType().ToString().Contains("TextBox"))
            {
                _inRenameState = Tabs.SelectedIndex;
                CreateRenameBox((ContentControl)sender);
            }
        }

        /// <summary>
        /// Processing of lost focus by RenameBox
        /// </summary>
        /// <param name="sender"> Pointer at RenameBox </param>
        /// <param name="e"> RoutedEvent arguments </param>
        private void OnRenameBoxLostFocus(object sender, RoutedEventArgs e)
        {
            ChangeConferenceName(((TextBox)sender).Text);
        }

        #endregion

        public void OnErrorWindowOpened(object sender, EventArgs e)
        {
            IsEnabled = false;
            Focusable = false;
        }

        public void OnErrorWindowClosed(object sender, EventArgs e)
        {
            IsEnabled = true;
            Focusable = true;
        }
    }
}
