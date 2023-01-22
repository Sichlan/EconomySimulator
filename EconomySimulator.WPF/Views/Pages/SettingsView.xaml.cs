using EconomySimulator.WPF.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace EconomySimulator.WPF.Views.Pages;

public partial class SettingsView : INavigableView<SettingsViewModel>
{
    public SettingsViewModel ViewModel { get; }
    
    public SettingsView(SettingsViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }
}