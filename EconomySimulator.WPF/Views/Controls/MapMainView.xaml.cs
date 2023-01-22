using System;
using System.Windows;
using System.Windows.Controls;
using EconomySimulator.WPF.ViewModels.Map;

namespace EconomySimulator.WPF.Views.Controls;

public partial class MapMainView : UserControl
{
    public MapMainView()
    {
        InitializeComponent();
    }

    private void MapMainView_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MapMainViewModel mapMainViewModel)
            throw new InvalidCastException();
        
        mapMainViewModel.OnMapUpdated += OnMapMainViewModelOnOnMapUpdated;
    }

    private void OnMapMainViewModelOnOnMapUpdated(MapMainViewModel mapMainViewModel)
    {
        MapGrid.Children.Clear();

        foreach (var frameworkElement in mapMainViewModel.LayerFrameworkElements)
        {
            MapGrid.Children.Add(frameworkElement);
        }
    }
}