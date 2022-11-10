using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
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

        };
    }

    private ObservableCollection<INavigationControl> GenerateNavigationFooter()
    {
        return new ObservableCollection<INavigationControl>()
        {

        };
    }

    private ObservableCollection<MenuItem> GenerateTrayMenuItems()
    {
        return new ObservableCollection<MenuItem>()
        {

        };
    }
}