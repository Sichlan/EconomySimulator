using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using EconomySimulator.BusinessLogic.Models.Simulation.Layers;
using EconomySimulator.BusinessLogic.Services.SimulationServices;
using Mars.Core.Executor.Implementation;

namespace EconomySimulator.WPF.ViewModels.Map;

[ObservableObject]
public partial class MapMainViewModel
{
    private readonly ISimulationContainerService _simulationContainerService;
    
    [ObservableProperty] private ObservableCollection<DrawVectorLayerFrameworkElement> _layerFrameworkElements;
    [ObservableProperty] private bool _isLoading;
    
    public event Action<MapMainViewModel> OnMapUpdated;

    public MapMainViewModel(ISimulationContainerService simulationContainerService)
    {
        LayerFrameworkElements = new();
        _simulationContainerService = simulationContainerService;
        
        _simulationContainerService.OnSimulationInitiated += SimulationContainerServiceOnOnSimulationInitiated;
        _simulationContainerService.OnSimulationStep += SimulationContainerServiceOnOnSimulationStep;
    }

    private void SimulationContainerServiceOnOnSimulationStep(ISimulationContainerService sender)
    {
        throw new NotImplementedException();
    }

    private void SimulationContainerServiceOnOnSimulationInitiated(ISimulationContainerService sender)
    {
        IsLoading = true;
        
        try
        {
            if (sender.CurrentSimulationWorkflowState?.Model is RuntimeModelImpl runtimeModelImpl)
            {
                foreach (var layer in runtimeModelImpl.AllLayers)
                {
                    switch (layer)
                    {
                        case GisCellsLayer gisCellsLayer:
                            LayerFrameworkElements.Add(new DrawVectorLayerFrameworkElement<GisCellsLayer>(gisCellsLayer, 
                                _ => Brushes.PaleGreen, 
                                x => x.VectorStructured.Data.TryGetValue("type", out var value) ? value.ToString() != "island" : false));

                            break;
                        case GisRiverLayer gisRiverLayer:
                            LayerFrameworkElements.Add(new DrawVectorLayerFrameworkElement<GisRiverLayer>(gisRiverLayer, 
                                _ => Brushes.LightSkyBlue, 
                                x => false));

                            break;
                    }
                }
            }
        
            UpdateLayout();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateLayout()
    {
        OnMapUpdated?.Invoke(this);
    }
}