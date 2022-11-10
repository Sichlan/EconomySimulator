using System.Windows.Controls;
using EconomySimulator.WPF.ViewModels;
using Wpf.Ui.Common.Interfaces;

namespace EconomySimulator.WPF.Views.Pages;

public partial class TestView : INavigableView<TestViewModel>
{
    public TestView(TestViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }

    public TestViewModel ViewModel { get; }
}