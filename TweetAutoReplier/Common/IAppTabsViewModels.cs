using TweetAutoReplier.ViewModels;

namespace TweetAutoReplier.Common
{
    public interface IAppTabsViewModels
    {
        Tweetinvi.TwitterClient Client { get; } 
        MainTabViewModel MainTabViewModel { get; }
        MessageTabViewModel MessageTabViewModel { get; }
        LogTabViewModel LogTabViewModel { get; }
    }
}
