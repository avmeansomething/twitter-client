using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using TweetSharp;

namespace TwitterClient
{
    public class TwitterService
    {
        public ITwitterService service;

        public TwitterService(string consumerKey, string consumerSecret)
        {
            service = new TweetSharp.TwitterService(consumerKey, consumerSecret);
        }
    }
}
