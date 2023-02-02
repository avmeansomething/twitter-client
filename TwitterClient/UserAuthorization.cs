using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TweetSharp;

namespace TwitterClient
{
    public class UserAuthorization
    {
        ITwitterService service;

        OAuthRequestToken requestToken;

        public UserAuthorization(ITwitterService service)
        {
            this.service = service;
        }

        public void PreAuthorization()
        {
            requestToken = service.GetRequestToken();
            Uri uri = service.GetAuthorizationUri(requestToken);
            Process.Start(uri.ToString());
        }

        public void VerifyAuthorization(string verifier)
        {
            OAuthAccessToken access = service.GetAccessToken(requestToken, verifier);
            service.AuthenticateWith(access.Token, access.TokenSecret);
        }

        public bool CheackAuthorization()
        {
            if (service.GetAccountSettings() == null)
            {
                MessageBox.Show("Ошибка авторизации!");              
                return false;
            }
            else
                return true;
        }
    }
}
