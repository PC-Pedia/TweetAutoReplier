using TweetAutoReplier.ViewModels;

namespace TweetAutoReplier.Common
{
    public interface IAppTabsViewModels
    {
        MainTabViewModel MainTabViewModel { get; }
        MessageTabViewModel MessageTabViewModel { get; }
        LogTabViewModel LogTabViewModel { get; }
    }
}
