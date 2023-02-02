using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using TweetSharp;

namespace TwitterClient.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        TwitterService twitter;

        GetTweets getTweets;

        FileStream mediaFile = null;

        bool checkImage = false;

        public ImageSource UserImage { get; set; }
        public int UserTweets { get; set; }
        public int UserFollowing { get; set; }
        public int UserFollowers { get; set; }
        public string UserName { get; set; }
        public static string UserID { get; set; }

        public MainMenu(TwitterService twitter)
        {
            InitializeComponent();
            this.twitter = twitter;
            this.getTweets = new GetTweets(this.twitter.service);
            ShowUserInfo(getTweets.GetUserInfo());
            DataContext = this;
        }

        public void ShowUserInfo(TwitterUser user)
        {
            UserImage = GetImage(user.ProfileImageUrlHttps);
            UserTweets = user.StatusesCount;
            UserFollowing = user.FriendsCount;
            UserFollowers = user.FollowersCount;
            UserName = user.Name;
            UserID = user.Id.ToString();
        }

        public void UpdateInfo(TwitterUser user)
        {
            UserTweets = user.StatusesCount;
            TweetCount.Text = UserTweets.ToString();
            UserFollowing = user.FriendsCount;
            FollowingCount.Text = UserFollowing.ToString();
            UserFollowers = user.FollowersCount;
            FollowersCount.Text = UserFollowers.ToString();
        }

        public void HideSendTweetComponents()
        {
            SendTweetGrid.Visibility = Visibility.Hidden;
            ListBoxTweets.Visibility = Visibility.Visible;
        }

        public void ShowTweetComponents()
        {
            SendTweetGrid.Visibility = Visibility.Visible;
            ListBoxTweets.Visibility = Visibility.Hidden;
        }

        public ImageSource GetImage(string path)
        {
            var image = new BitmapImage();
            int BytesToRead = 100;

            WebRequest request = WebRequest.Create(new Uri(path, UriKind.Absolute));
            request.Timeout = -1;
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            BinaryReader reader = new BinaryReader(responseStream);
            MemoryStream memoryStream = new MemoryStream();

            byte[] bytebuffer = new byte[BytesToRead];
            int bytesRead = reader.Read(bytebuffer, 0, BytesToRead);

            while (bytesRead > 0)
            {
                memoryStream.Write(bytebuffer, 0, bytesRead);
                bytesRead = reader.Read(bytebuffer, 0, BytesToRead);
            }

            image.BeginInit();
            memoryStream.Seek(0, SeekOrigin.Begin);

            image.StreamSource = memoryStream;
            image.EndInit();

            return image;
        }

        public void LoadSettingsForUser()
        {
            bool checkID = false;

            XDocument xDoc = XDocument.Load("../../Files/Settings.xml");
            XElement root = xDoc.Element("Settings");

            foreach (XElement xElement in root.Elements("Save").ToList())
            {
                if (xElement.Element("UserID").Value == UserID)
                {
                    checkID = true;

                    if (xElement.Element("Theme").Value == "1")
                    {
                        Uri uri = new Uri($"pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml");
                        Application.Current.Resources.MergedDictionaries.RemoveAt(0);
                        Application.Current.Resources.MergedDictionaries.Insert(0, new ResourceDictionary() { Source = uri });
                    }
                    else
                    {
                        Uri uri = new Uri($"pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Dark.xaml");
                        Application.Current.Resources.MergedDictionaries.RemoveAt(0);
                        Application.Current.Resources.MergedDictionaries.Insert(0, new ResourceDictionary() { Source = uri });
                    }

                    if (xElement.Element("Color").Value == "1")
                    {
                        Uri uri = new Uri($"pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Cyan.xaml");
                        Application.Current.Resources.MergedDictionaries.RemoveAt(2);
                        Application.Current.Resources.MergedDictionaries.Insert(2, new ResourceDictionary() { Source = uri });
                    }

                    if (xElement.Element("Color").Value == "2")
                    {
                        Uri uri = new Uri($"pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.DeepPurple.xaml");
                        Application.Current.Resources.MergedDictionaries.RemoveAt(2);
                        Application.Current.Resources.MergedDictionaries.Insert(2, new ResourceDictionary() { Source = uri });
                    }

                    if (xElement.Element("Color").Value == "3")
                    {
                        Uri uri = new Uri($"pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Teal.xaml");
                        Application.Current.Resources.MergedDictionaries.RemoveAt(2);
                        Application.Current.Resources.MergedDictionaries.Insert(2, new ResourceDictionary() { Source = uri });
                    }
                }
            }

            if (checkID == false)
            {
                root.Add(new XElement("Save",
                    new XElement("UserID", UserID),
                    new XElement("Theme", "1"),
                    new XElement("Color", "1")));

                xDoc.Save("../../Files/Settings.xml");
            }
        }

        public async void ShowTweets(IEnumerable<TwitterStatus> tweets)
        {
            if (tweets == null)
            {
                return;
            }

            foreach (var tweet in tweets)
            {
                await Task.Run(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        Tweet TweetInList = new Tweet(tweet);
                        TweetInList.Retweet += getTweets.RetweetTweet;
                        ListBoxTweets.Items.Add(TweetInList);
                    });
                });
            }
        }

        private void AddFile()
        {
            mediaFile = null;

            OpenFileDialog openFile = new OpenFileDialog
            {
                Filter = "(*.jpg)|*.jpg|All files (*.*)|*.*"
            };

            bool? result = openFile.ShowDialog();

            if (result == true)
            {
                try
                {
                    mediaFile = new FileStream(openFile.FileName, FileMode.Open);
                }
                catch (Exception ex)
                {
                    ShowNotificationAnimation("Ошибка! Попробуйте еще раз");
                }
            }

            if (mediaFile != null)
            {
                GreenMark.Source = new BitmapImage(new Uri("/Images/GreenCheckMark.png", UriKind.Relative));
            }
        }

        private void ShowNotificationAnimation(string text)
        {
            LabelNotification.Content = text;

            DoubleAnimation animation = new DoubleAnimation();

            animation.From = NotificationBorder.ActualHeight;
            animation.To = 40;
            animation.Duration = TimeSpan.FromSeconds(1);
            animation.AutoReverse = true;
            NotificationBorder.BeginAnimation(Border.HeightProperty, animation);
        }

        private void AddFileToTweet_Click(object sender, RoutedEventArgs e)
        {
            AddFile();
        }

        private void ShowTweetsInLine_Click(object sender, RoutedEventArgs e)
        {
            HideSendTweetComponents();

            ListBoxTweets.Items.Clear();
            ShowTweets(getTweets.GetTweetsInLine());
            UpdateInfo(getTweets.GetUserInfo());
        }

        private void ShowMyTweets_Click(object sender, RoutedEventArgs e)
        {
            HideSendTweetComponents();

            ListBoxTweets.Items.Clear();
            ShowTweets(getTweets.GetMyTweets());
            UpdateInfo(getTweets.GetUserInfo());
        }

        private void ShowSendTweetComponents_Click(object sender, RoutedEventArgs e)
        {
            ShowTweetComponents();

            UpdateInfo(getTweets.GetUserInfo());
        }

        private void SendTweet_Click(object sender, RoutedEventArgs e)
        {
            if ((TweetContentTextBox.Text != "") || (mediaFile != null))
            {
                getTweets.PublishTweet(TweetContentTextBox.Text, mediaFile);
                TweetContentTextBox.Clear();
                ShowNotificationAnimation("Ваш твит отправлен");
                UserTweets++;
                TweetCount.Text = UserTweets.ToString();
                GreenMark.Source = null;
                mediaFile = null;
            }
            else
            {
                ShowNotificationAnimation("Введите текст для нового твита");
            }
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
        }

        private void GreenMark_MouseEnter(object sender, MouseEventArgs e)
        {
            if (GreenMark.Source != null)
            {
                GreenMark.Source = new BitmapImage(new Uri("/Images/RedCross.png", UriKind.Relative));
                checkImage = true;
            }
        }

        private void GreenMark_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (checkImage == true)
            {
                mediaFile.Dispose();
                mediaFile = null;
                GreenMark.Source = null;
            }
        }

        private void GreenMark_MouseLeave(object sender, MouseEventArgs e)
        {
            if (GreenMark.Source != null)
            {
                GreenMark.Source = new BitmapImage(new Uri("/Images/GreenCheckMark.png", UriKind.Relative));
                checkImage = false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettingsForUser();
        }

        private void TweetContentTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((TweetContentTextBox.Text.Length == 0) && (e.Key == Key.Space))
                e.Handled = true;
        }

        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"..\..\Files\TwitterClientHelp.chm");
        }
    }
}
