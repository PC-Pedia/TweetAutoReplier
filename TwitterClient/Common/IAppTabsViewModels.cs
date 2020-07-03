﻿using TwitterClient.ViewModels;

namespace TwitterClient.Common
{
    public interface IAppTabsViewModels
    {
        MainTabViewModel MainTabViewModel { get; }
        MessageTabViewModel MessageTabViewModel { get; }
        LogTabViewModel LogTabViewModel { get; }
    }
}
