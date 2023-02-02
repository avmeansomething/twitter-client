using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TwitterClient.Pages
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        TwitterService twitter;

        UserAuthorization userAuthorization;

        public Authorization(TwitterService twitter)
        {
            InitializeComponent();
            this.twitter = twitter;
            this.userAuthorization = new UserAuthorization(this.twitter.service);
        }

        private void SendPin_Click(object sender, RoutedEventArgs e)
        {
            userAuthorization.VerifyAuthorization(PinTextBox.Text);
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            userAuthorization.PreAuthorization();
        }

        private void PinTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
