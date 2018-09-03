using TwitterClient.ViewModel;

namespace TwitterClient.Contracts
{
    public interface IAppTabsViewModels
    {
        MainTabViewModel MainTabViewModel { get; }
        MessageTabViewModel MessageTabViewModel { get; }
        LogTabViewModel LogTabViewModel { get; }
    }
}
