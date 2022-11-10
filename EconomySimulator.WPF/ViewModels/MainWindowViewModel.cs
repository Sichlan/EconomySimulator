using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using EconomySimulator.WPF.Views.Pages;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace EconomySimulator.WPF.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private bool _isInitialized = false;

    [ObservableProperty] private ObservableCollection<INavigationControl> _navigationItems = new();
    [ObservableProperty] private ObservableCollection<INavigationControl> _navigationFooter = new();
    [ObservableProperty] private ObservableCollection<MenuItem> _trayMenuItems = new();

    public MainWindowViewModel(INavigationService navigationService)
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    private void InitializeViewModel()
    {
        NavigationItems = GenerateNavigationItems();
        NavigationFooter = GenerateNavigationFooter();
        TrayMenuItems = GenerateTrayMenuItems();

        _isInitialized = true;
    }

    private ObservableCollection<INavigationControl> GenerateNavigationItems()
    {
        return new ObservableCollection<INavigationControl>()
        {
#if DEBUG
            new NavigationItem()
            {
                Content = Resources.Localization.Resources.TestViewMenuTitle,
                PageTag = "debug",
                Icon = SymbolRegular.Box24,
                PageType = typeof(TestView)
            }
#endif
        };
    }

    private ObservableCollection<INavigationControl> GenerateNavigationFooter()
    {
        return new ObservableCollection<INavigationControl>()
        {
            new NavigationItem()
            {
                Content = Resources.Localization.Resources.SettingsViewMenuTitle,
                PageTag = "settings",
                Icon = SymbolRegular.Settings24,
                PageType = typeof(SettingsView)
            }
        };
    }

    private ObservableCollection<MenuItem> GenerateTrayMenuItems()
    {
        return new ObservableCollection<MenuItem>()
        {
            new MenuItem
            {
                Header = "Home",
                Tag = "tray_home"
            }

        };
    }
}