using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;

namespace TwitterClient
{
    public class GetTweets
    {
        ITwitterService service;

        public GetTweets(ITwitterService service)
        {
            this.service = service;
        }

        public TwitterUser GetUserInfo()
        {
            return service.GetUserProfile(new GetUserProfileOptions() { IncludeEntities = false, SkipStatus = false });
        }

        public void PublishTweet(string text, FileStream mediaFile)
        {
            if (mediaFile != null)
            {
                service.SendTweetWithMedia(new SendTweetWithMediaOptions
                {
                    Status = text,
                    Images = new Dictionary<string, Stream> { { "pic", mediaFile } }
                });
            }
            else
            {
                service.SendTweet(new SendTweetOptions
                {
                    Status = text
                });
            }
        }

        public IEnumerable<TwitterStatus> GetTweetsInLine()
        {
            return service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions());
        }

        public IEnumerable<TwitterStatus> GetMyTweets()
        {
            return service.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions());
        }

        public void RetweetTweet(long TweetId)
        {
            service.Retweet(new RetweetOptions
            {
                Id = TweetId
            });
        }
    }
}
