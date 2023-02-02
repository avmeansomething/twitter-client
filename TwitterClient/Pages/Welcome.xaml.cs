using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для Welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {
        TwitterService twitter;

        UserAuthorization userAuthorization;

        public Welcome()
        {
            InitializeComponent();
            twitter = new TwitterService("LunW98nTNR2EnAaJLuVXvZZAY",
                "LQXZ4JgwJn07097PXv6MHEmreVXz943yZbBXYJTM0IVPz5ZsOA");
            this.twitter = twitter;
            this.userAuthorization = new UserAuthorization(this.twitter.service);
        }

        private void Authorization_Click(object sender, RoutedEventArgs e)
        {
            Authorization authorization = new Authorization(twitter);
            authorization.Owner = this;
            authorization.ShowDialog();
            authorization.Close();

            if (userAuthorization.CheackAuthorization())
            {
                MainMenu main = new MainMenu(twitter);
                this.Hide();
                main.ShowDialog();
                this.Close();
            }
        }
    }
}
