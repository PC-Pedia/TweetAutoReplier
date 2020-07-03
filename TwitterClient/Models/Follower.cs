using System.Collections.Generic;

namespace TwitterClient.Models
{
    public class Follower
    {
        public string ScreenName { get; set; }
        public string IdStr { get; set; }
        public string NoOfReplies { get; set; }
        public string Filter { get; set; }
        public string DisplayTime { get; set; }
        public List<string> Messages { get; set; }
        public Follower()
        {
            Messages = new List<string>();
        }
    }
}
