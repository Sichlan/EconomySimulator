using EconomySimulator.WPF.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace EconomySimulator.WPF.Views.Pages;

public partial class SimulationMainView : INavigableView<SimulationMainViewModel>
{
    public SimulationMainViewModel ViewModel { get; }
    public SimulationMainView(SimulationMainViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }
}