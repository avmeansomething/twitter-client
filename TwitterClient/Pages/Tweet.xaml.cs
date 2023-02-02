using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TweetSharp;

namespace TwitterClient.Pages
{
    /// #FF673AB7
    /// <summary>
    /// Логика взаимодействия для Tweet.xaml
    /// </summary>

    public delegate void RetweetHandler(long TweetId);

    public partial class Tweet : UserControl
    {
        public event RetweetHandler Retweet;

        TwitterStatus tweet;

        public string Author { get; set; }
        public string Date { get; set; }
        public string TweetText { get; set; }
        public ImageSource TweetImage { get; set; }

        public Tweet(TwitterStatus tweet)
        {
            InitializeComponent();

            this.tweet = tweet;

            Author = tweet.Author.ScreenName;
            Date = tweet.CreatedDate.ToLocalTime().ToString("dd MMMM yyyy HH:mm");

            if (tweet.Text.Contains("http"))
            {
                TweetText = tweet.TextDecoded.Substring(0, tweet.TextDecoded.IndexOf("http"));
                TweetHyperLink.NavigateUri = new Uri(tweet.Text.Substring(tweet.Text.IndexOf("http"), tweet.Text.Length - tweet.Text.IndexOf("http")));
                TweetHyperLinkText.Text = TweetHyperLink.NavigateUri.AbsoluteUri;
            }
            else
            {
                TweetText = tweet.TextDecoded;
            }

            if (tweet.Entities.Media.Count > 0)
            {
                TweetImage = GetImage(tweet.Entities.Media[0].MediaUrlHttps);
            }
            else
            {
                (img.Parent as Grid).Children.Remove(img);
                Grid.SetRow(RetweetButton, 0);
                Grid.SetRowSpan(RetweetButton, 2);
            }

            DataContext = this;
        }

        private ImageSource GetImage(string path)
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

        private void RetweetButton_Click(object sender, RoutedEventArgs e)
        {
            Retweet?.Invoke(tweet.Id);
        }

        private void TweetHyperLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
