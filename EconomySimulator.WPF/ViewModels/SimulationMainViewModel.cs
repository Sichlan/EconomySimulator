using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Common.Interfaces;

namespace EconomySimulator.WPF.ViewModels;

public partial class SimulationMainViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    private void InitializeViewModel()
    {
        //TODO: Initialize view model
        
        _isInitialized = true;
    }

    public void OnNavigatedFrom()
    {
        
    }

    #region Commands

    [RelayCommand]
    public void NewSimulationConfiguration()
    {
        
    }
    
    [RelayCommand]
    public void SaveSimulationConfiguration()
    {
        
    }
    
    [RelayCommand]
    public void LoadSimulationConfiguration()
    {
        
    }

    #endregion
}