using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace iMessenger
{

    public partial class MainWindow : Window
    {

        public Core Core { get; set; }
        public TabList<Tab> ConferenceList;

        public MainWindow()
        {
            InitializeComponent();
            ConferenceList = new TabList<Tab>();
            ConferenceList.Add(new Tab("Common", ConnectList0, ChatArea0));
        }

        private void Chat_Loaded(object sender, RoutedEventArgs e)
        {
            try{
                NickBox.Text = Core.UserName;
                Core.SendMessage( GenerateMessage("", MessageType.JoinCommon));
                MessageBox.Focus();    
            }catch( NullReferenceException exeption){

            }
        }

    
        public void ShowMessage(Message message)
        {
            try{
                    switch (message.Type)
                    {
                        case MessageType.Common:
                            {
                                Dispatcher.Invoke((ThreadStart)delegate
                                {
                                    ConferenceList[0].rtb.Document.Blocks.Add(new Paragraph(new Run(message.getMessageString())));
                                    ConferenceList[0].rtb.ScrollToEnd();
                                });
                                break;
                            }
                        case MessageType.Conference:
                            {
                                foreach(Tab tab in ConferenceList)
                                {
                                    if (tab.Name.ToString() == message.ConferenceNum)
                                    {
                                        Dispatcher.Invoke((ThreadStart)delegate
                                        {
                                            tab.rtb.Document.Blocks.Add(new Paragraph(new Run(message.getMessageString())));
                                            tab.rtb.ScrollToEnd();
                                        });
                                        return;
                                    }
                                }

                                Dispatcher.Invoke((ThreadStart)delegate
                                {
                                    CreateTab(message.Text.Remove(8));

                                    ConferenceList[ConferenceList.Count - 1].rtb.Document.Blocks.Add(new Paragraph(new Run(message.getMessageString())));
                                    ConferenceList[ConferenceList.Count-1].rtb.ScrollToEnd();
                                });
                                break;
                            }
                        case MessageType.LeaveConference:
                            {
                                foreach(Tab tab in ConferenceList)
                                {
                                    if (tab.Name.ToString() == message.Text)
                                    {
                                        Dispatcher.Invoke((ThreadStart)delegate
                                        {
                                            tab.rtb.Document.Blocks.Add(new Paragraph(generateStylyzedRun(message.getMessageString())));
                                            tab.rtb.ScrollToEnd();
                                        });
                                        return;
                                    }
                                }
                                break;
                            }
                        default:
                            {
                                foreach (Tab tab in ConferenceList)
                                {
                                    foreach (CheckBox listItem in tab.lb.Items)
                                    {
                                        Dispatcher.Invoke((ThreadStart)delegate
                                        {
                                            if (listItem.Content.ToString() == message.SenderName &&
                                                listItem.IsChecked == true)
                                            {
                                                tab.rtb.Document.Blocks.Add(new Paragraph(generateStylyzedRun(message.getMessageString())));
                                                tab.rtb.ScrollToEnd();
                                            }
                                        });
                                    }
                                }
                                break;
                            }
                    }
            }catch( NullReferenceException e){

            }
        }
       
        private Run generateStylyzedRun(String text){
            return new Run() { Text = text, Foreground = Brushes.DarkGreen, FontStyle = FontStyles.Italic };
        }

        private void Chat_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Core.SendMessage(GenerateMessage("", MessageType.LeaveCommon));
            Environment.Exit(0x0);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(MessageBox.Text))
            {
                if(Tabs.SelectedIndex == 0)
                    Core.SendMessage(GenerateMessage(GetMessageText(), MessageType.Common));
                else
                    Core.SendMessage(GenerateMessage(GetMessageText(), MessageType.Conference));
            }
        }

        private void MessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SendButton_Click(sender, new RoutedEventArgs());
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

        public Message GenerateMessage(String data, MessageType Type)
        {
            return new Message()
            {
                SenderName = Core.UserName,
                SenderIP = Core.UserIP,
                Text = data,
                Type = Type,
                Receivers = GetReceiversList(),
                ConferenceNum = GetConferenceNum()
            };
        }
        
        private void NickBox_LostFocus(object sender, RoutedEventArgs e)
        {
            NicknameChanging();
        }
        
        private void NicknameChanging()
        {
            if (!String.IsNullOrEmpty(NickBox.Text) && NickBox.Text != Core.UserName)
            {
                CheckBox item = new CheckBox();
                bool flag = true;
                for (int i = 0; i < ConferenceList[0].lb.Items.Count; i++)
                {
                    item = (CheckBox)ConferenceList[0].lb.Items[i];
                    if (item.Content.ToString() == NickBox.Text)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    Core.SendMessage(GenerateMessage(NickBox.Text, MessageType.ChangeName));
                    Core.UserName = NickBox.Text;
                    return;
                }
                else
                {
                    Dispatcher.Invoke((ThreadStart)delegate
                    {
                        Run run = new Run("This nickname is already in use!");
                        run.Foreground = Brushes.Red;
                        ConferenceList[0].rtb.Document.Blocks.Add(new Paragraph(run));
                        ConferenceList[0].rtb.ScrollToEnd();
                    });
                }
            }
            NickBox.Text = Core.UserName;
        }

        public void AddAtConnectList(string newNick)
        {
            Dispatcher.Invoke((ThreadStart)delegate
            {
                for (int i = 0; i < ConferenceList[0].lb.Items.Count; i++)
                {
                    if(((CheckBox)ConferenceList[0].lb.Items[i]).Content.ToString() == newNick)
                        return;
                }
                CheckBox item = new CheckBox();
                item.Content = newNick;
                item.IsChecked = true;
                byte[] RGB = new byte[3]; 
                Random r = new Random();
                r.NextBytes(RGB);
                for(int i=0; i<3; i++)
                    RGB[i] = (byte)(RGB[i] % 128);
                item.Foreground = new SolidColorBrush(Color.FromRgb(RGB[0], RGB[1], RGB[2]));
                ConferenceList[0].lb.Items.Add(item);
                CheckBox ToFor;
                for (int i = 0; i < ConferenceList.Count; i++)
                {
                    ToFor = new CheckBox();
                    ToFor.Content = item.Content;
                    ToFor.IsChecked = false;
                    ToFor.Foreground = item.Foreground;
                    ConferenceList[0].lb.Items.Add(ToFor);
                }
            });
        }

        public void ChangeConnectList(string oldNick, string newNick)
        {
            try
            {
                Dispatcher.Invoke((ThreadStart)delegate
                {
                    CheckBox item;
                    for (int i = 0; i < ConferenceList.Count; i++)
                    {
                        for (int j = 0; j < ConferenceList[i].lb.Items.Count; j++)
                        {
                            item = (CheckBox)ConferenceList[i].lb.Items[j];
                            if (item.Content.ToString() == oldNick)
                            {
                                ConferenceList[i].lb.Items.Remove(item);
                                item.Content = newNick;
                                ConferenceList[i].lb.Items.Insert(j, item);
                                break;
                            }
                        }
                    }
                });
            }catch(ArgumentOutOfRangeException e)
            {

            }            
        }

        public void ReplaceConnectList(string oldNick)
        {
            Dispatcher.Invoke((ThreadStart)delegate
            {
                CheckBox item;
                for (int i = 0; i < ConferenceList.Count; i++)
                {
                    for (int j = 0; j < ConferenceList[i].lb.Items.Count; j++)
                    {
                        item = (CheckBox)ConferenceList[i].lb.Items[j];
                        if (item.Content.ToString() == oldNick)
                        {
                            ConferenceList[i].lb.Items.Remove(item);
                            break;
                        }
                    }
                }
            });
        }

        private void NickBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NicknameChanging();
                MessageBox.Focus();
            }
        }

        private List<String> GetReceiversList()
        {
            List<String> receiversList = new List<String>();
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

        private String GetConferenceNum()
        {
            String res = "";
            Dispatcher.Invoke((ThreadStart)delegate
            {
                if (Tabs.SelectedIndex == 0)
                    res = "Common";
                else
                    res = ConferenceList[Tabs.SelectedIndex-1].Name;
            });
            return res;
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tabs.SelectedIndex == Tabs.Items.Count - 1)
            {
                CreateTab(null);
                Tabs.SelectedIndex -= 1; // Set  current selected tab to new created tab.
            }
        }

        private void CreateTab(String message)
        {
            Tab newTab = new Tab(message, ConnectList0, ChatArea0);
            newTab.lb.Name = "ChatArea" + (ConferenceList.Count+1).ToString();
            newTab.rtb.Name = "ConnectList" + (ConferenceList.Count + 1).ToString();
            ConferenceList.Add(newTab);
            
            TabItem tabItem = new TabItem()
            {
                Background = ((TabItem)Tabs.Items[0]).Background,
                Foreground = ((TabItem)Tabs.Items[0]).Foreground,
                Content = newTab.grid,
            };
            tabItem.Name = "TabItem" + (Tabs.Items.Count - 1).ToString();
            tabItem.Header = "Conference#" + newTab.Name;
            tabItem.ContextMenu = SetPopupMenu(tabItem);
            Tabs.Items.Insert(Tabs.Items.Count - 1, tabItem);
        }

        private ContextMenu SetPopupMenu(TabItem tabItem)
        {
            ContextMenu PopupMenu = new ContextMenu();
            PopupMenu.Foreground = new SolidColorBrush(Colors.DarkRed);

            MenuItem Rename = new MenuItem();
            Rename.IsCheckable = false;
            Rename.Header = "Rename";
            Rename.Click += new RoutedEventHandler(ContextMenu_OnRenameClick);
            PopupMenu.Items.Add(Rename);

            MenuItem Close = new MenuItem();
            Close.IsCheckable = false;
            Close.Header = "Close";
            Close.Click += new RoutedEventHandler(ContextMenu_OnCloseClick);
            PopupMenu.Items.Add(Close);
            PopupMenu.Name = "PopupMenu" + tabItem.Name.Replace("TabItem", "");
            return PopupMenu;
        }

        private void ContextMenu_OnRenameClick(object menuItem, RoutedEventArgs e)
        {
            return;
        }

        private void ContextMenu_OnCloseClick(object menuItem, RoutedEventArgs e)
        {
            TabItem ToDelete = null;
            for (int i = 1; i < Tabs.Items.Count - 1; i++)
                if (((TabItem)Tabs.Items[i]).Name == ((ContextMenu)((MenuItem)menuItem).Parent).Name.Replace("PopupMenu", "TabItem"))
                {
                    ToDelete = (TabItem)Tabs.Items[i];
                    Core.SendMessage(GenerateMessage(ToDelete.Name.ToString(), MessageType.LeaveConference));
                    ConferenceList.Delete(i - 1);
                    break;
                }
            Tabs.SelectedItem = Tabs.Items[0];
            Tabs.Items.Remove(ToDelete);
            return;
        }
    }
    
}
