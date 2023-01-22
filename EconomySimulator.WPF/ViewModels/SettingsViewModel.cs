using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common.Interfaces;

namespace EconomySimulator.WPF.ViewModels;

public partial class SettingsViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [ObservableProperty] private string _appVersion = string.Empty;
    [ObservableProperty] private ThemeType _currentTheme = ThemeType.Unknown;

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    public void OnNavigatedFrom()
    {
    }

    private void InitializeViewModel()
    {
        CurrentTheme = Theme.GetAppTheme();
        AppVersion = $"{Resources.Localization.Resources.ApplicationTitle} - {GetAssemblyVersion()}";

        _isInitialized = true;
    }

    private string GetAssemblyVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
    }

    [RelayCommand]
    private void ChangeTheme(string parameter)
    {
        switch (parameter)
        {
            case "theme_light":
                if (CurrentTheme == ThemeType.Light)
                    break;

                Theme.Apply(ThemeType.Light);

                break;

            default:
                if (CurrentTheme == ThemeType.Dark)
                    break;

                Theme.Apply(ThemeType.Dark);

                break;
        }
        CurrentTheme = Theme.GetAppTheme();
    }
}