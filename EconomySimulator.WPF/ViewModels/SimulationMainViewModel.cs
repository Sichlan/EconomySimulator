using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EconomySimulator.BusinessLogic.Models.Simulation.Layers;
using EconomySimulator.BusinessLogic.Services.SimulationServices;
using EconomySimulator.WPF.ViewModels.Map;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace EconomySimulator.WPF.ViewModels;

public partial class SimulationMainViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;
    
    [Localizable(false)] private readonly string _standardConfigurationFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EconomySimulator\\Configurations";

    private readonly ISimulationContainerService _simulationContainerService;
    private readonly IDialogService _dialogService;
    private readonly Random _random;
    [ObservableProperty] private MapMainViewModel? _mapMainViewModel;
    private readonly ILogger<DrawVectorLayerFrameworkElement<GisCellsLayer>> _cellsLayerLogger;
    private readonly ILogger<DrawVectorLayerFrameworkElement<GisRiverLayer>> _riversLayerLogger;

    public SimulationMainViewModel(ISimulationContainerService simulationContainerService, IDialogService dialogService, Random random, ILogger<DrawVectorLayerFrameworkElement<GisCellsLayer>> cellsLayerLogger, ILogger<DrawVectorLayerFrameworkElement<GisRiverLayer>> riversLayerLogger)
    {
        _simulationContainerService = simulationContainerService;
        _dialogService = dialogService;
        _random = random;
        _cellsLayerLogger = cellsLayerLogger;
        _riversLayerLogger = riversLayerLogger;

        NewSimulationConfigurationCommand = new RelayCommand(NewSimulationConfiguration);
        SaveSimulationConfigurationCommand = new RelayCommand(SaveSimulationConfiguration);
        LoadSimulationConfigurationCommand = new AsyncRelayCommand(LoadSimulationConfiguration, CanLoadSimulationConfiguration);
        CancelClosePopupCommand = new RelayCommand(ClosePopup);
        SaveClosePopupCommand = new RelayCommand(SaveClosePopup);

        StopSimulationCommand = new AsyncRelayCommand(StopSimulation, CanStopSimulation);
        RewindSimulationCommand = new AsyncRelayCommand(RewindSimulation, CanRewindSimulation);
        PlaySimulationCommand = new AsyncRelayCommand(PlaySimulation, CanPlaySimulation);
        FastForwardSimulationCommand = new AsyncRelayCommand(FastForwardSimulation, CanFastForwardSimulation);
        RestartSimulationCommand = new AsyncRelayCommand(RestartSimulation, CanRestartSimulation);

        _simulationContainerService.OnSimulationConfigChanged += _ =>
        {
            CommandManager.InvalidateRequerySuggested();
        };
    }

    #region Navigation

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    private void InitializeViewModel()
    {
        MapMainViewModel = new MapMainViewModel(_simulationContainerService, _random, _cellsLayerLogger, _riversLayerLogger);

        if (!Directory.Exists(_standardConfigurationFilePath))
            Directory.CreateDirectory(_standardConfigurationFilePath);
        
        _isInitialized = true;
    }

    public void OnNavigatedFrom()
    {
        
    }

    #endregion

    #region Commands

    public IRelayCommand NewSimulationConfigurationCommand { get; }
    public IRelayCommand SaveSimulationConfigurationCommand { get; }
    public IAsyncRelayCommand LoadSimulationConfigurationCommand { get; }
    public IRelayCommand CancelClosePopupCommand { get; }
    public IRelayCommand SaveClosePopupCommand { get; }
    public IAsyncRelayCommand StopSimulationCommand { get; }
    public IAsyncRelayCommand RewindSimulationCommand { get; }
    public IAsyncRelayCommand PlaySimulationCommand { get; }
    public IAsyncRelayCommand FastForwardSimulationCommand { get; }
    public IAsyncRelayCommand RestartSimulationCommand { get; }

    private async void NewSimulationConfiguration()
    {
        var dialog = _dialogService.GetDialogControl();
        dialog.Content = new NewSimulationConfigurationViewModel(_simulationContainerService);
        dialog.ButtonLeftClick += (sender, args) => SaveClosePopup();
        dialog.ButtonRightClick += (sender, args) => ClosePopup();
        
        await dialog.ShowAndWaitAsync();
    }

    private void SaveSimulationConfiguration()
    {
        if(_simulationContainerService.SimulationConfiguration == null)
            return;
        
        var saveFileDialog = new SaveFileDialog
        {
            InitialDirectory = _standardConfigurationFilePath,
            RestoreDirectory = true,
            Title = Resources.Localization.Resources.SaveDialogTitle,
            DefaultExt = "zip",
            CheckFileExists = false,
            CheckPathExists = true,
            FileName = Resources.Localization.Resources.DefaultSaveConfigurationFileName
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            _simulationContainerService.SaveSimulationConfiguration(saveFileDialog.FileName);
        }
    }

    private async Task LoadSimulationConfiguration()
    {
        var openFileDialog = new OpenFileDialog
        {
            InitialDirectory = _standardConfigurationFilePath,
            RestoreDirectory = true,
            Title = Resources.Localization.Resources.OpenDialogTitle,
            CheckFileExists = true,
        };

        if (openFileDialog.ShowDialog() == true)
        {
            await _simulationContainerService.LoadSimulationConfiguration(openFileDialog.FileName);
            _simulationContainerService.InitSimulation();
        }
    }

    private bool CanLoadSimulationConfiguration()
    {
        return true;
    }

    private async void SaveClosePopup()
    {
        var dialog = _dialogService.GetDialogControl();

        if (dialog.Content is NewSimulationConfigurationViewModel viewModel)
        {
            await _simulationContainerService.LoadSimulationConfiguration(viewModel.ToSimulationConfig());
            _simulationContainerService.InitSimulation();
        }
        
        ClosePopup();
    }

    private void ClosePopup()
    {
        var dialog = _dialogService.GetDialogControl();
        dialog.Hide();
    }

    private Task StopSimulation(CancellationToken arg)
    {
        throw new NotImplementedException();
    }

    private bool CanStopSimulation()
    {
        return _simulationContainerService.SimulationConfiguration != null
               && _simulationContainerService.CurrentSimulationWorkflowState != null;
    }

    private Task RewindSimulation(CancellationToken arg)
    {
        throw new NotImplementedException();
    }

    private bool CanRewindSimulation()
    {
        return _simulationContainerService.SimulationConfiguration != null
               && _simulationContainerService.CurrentSimulationWorkflowState != null;
    }

    private Task PlaySimulation(CancellationToken arg)
    {
        throw new NotImplementedException();
    }

    private bool CanPlaySimulation()
    {
        return _simulationContainerService.SimulationConfiguration != null;
    }

    private Task FastForwardSimulation(CancellationToken arg)
    {
        throw new NotImplementedException();
    }

    private bool CanFastForwardSimulation()
    {
        return _simulationContainerService.SimulationConfiguration != null;
    }

    private Task RestartSimulation(CancellationToken arg)
    {
        throw new NotImplementedException();
    }

    private bool CanRestartSimulation()
    {
        return _simulationContainerService.SimulationConfiguration != null
            && _simulationContainerService.CurrentSimulationWorkflowState != null;
    }

    #endregion
}