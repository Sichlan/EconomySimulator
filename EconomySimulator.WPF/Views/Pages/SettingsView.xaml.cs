using System.Windows.Controls;
using EconomySimulator.WPF.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace EconomySimulator.WPF.Views.Pages;

public partial class SettingsView : INavigableView<SettingsViewModel>
{
    public SettingsView(SettingsViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }

    public SettingsViewModel ViewModel { get; }
}