using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using EconomySimulator.BusinessLogic.Models.Simulation.Layers;
using EconomySimulator.BusinessLogic.Services.SimulationServices;
using Mars.Components.Layers;
using Mars.Core.Executor.Implementation;
using Mars.Interfaces.Layers;
using Microsoft.Extensions.Logging;

namespace EconomySimulator.WPF.ViewModels.Map;

[ObservableObject]
public partial class MapMainViewModel
{
    private readonly ISimulationContainerService _simulationContainerService;
    
    [ObservableProperty] private ObservableCollection<DrawLayerFrameworkElement> _layerFrameworkElements;
    [ObservableProperty] private bool _isLoading;
    private readonly Random _random;
    private readonly ILogger<DrawVectorLayerFrameworkElement<GisCellsLayer>> _cellsLayerLogger;
    private readonly ILogger<DrawVectorLayerFrameworkElement<GisRiverLayer>> _riversLayerLogger;

    public event Action<MapMainViewModel> OnMapUpdated;

    public MapMainViewModel(ISimulationContainerService simulationContainerService, Random random, ILogger<DrawVectorLayerFrameworkElement<GisCellsLayer>> cellsLayerLogger, ILogger<DrawVectorLayerFrameworkElement<GisRiverLayer>> riversLayerLogger)
    {
        LayerFrameworkElements = new();
        _simulationContainerService = simulationContainerService;
        _random = random;
        _cellsLayerLogger = cellsLayerLogger;
        _riversLayerLogger = riversLayerLogger;

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
                var allFeatures = runtimeModelImpl.AllLayers
                    .SelectMany(x => x is VectorLayer vectorLayer ? vectorLayer.Features : Enumerable.Empty<IVectorFeature>())
                    .ToList();
                
                double xMin = allFeatures.Min(x => x.VectorStructured.Geometry.Coordinates.Min(y => y.X)),
                    xMax = allFeatures.Max(x => x.VectorStructured.Geometry.Coordinates.Max(y => y.X)),
                    yMin = allFeatures.Min(x => x.VectorStructured.Geometry.Coordinates.Min(y => y.Y)),
                    yMax = allFeatures.Max(x => x.VectorStructured.Geometry.Coordinates.Max(y => y.Y));
                
                foreach (var layer in runtimeModelImpl.AllLayers)
                {
                    switch (layer)
                    {
                        case GisCellsLayer gisCellsLayer:
                            LayerFrameworkElements.Add(new DrawVectorLayerFrameworkElement<GisCellsLayer>(gisCellsLayer,
                                xMin, xMax, yMin, yMax,
                                _cellsLayerLogger,
                                x => CellsLayerFilter(x),
                                x => CellsLayerFillBrush(x),
                                x => CellsLayerPen(x)));

                            break;
                        case GisRiverLayer gisRiverLayer:
                            LayerFrameworkElements.Add(new DrawVectorLayerFrameworkElement<GisRiverLayer>(gisRiverLayer,
                                xMin, xMax, yMin, yMax, 
                                _riversLayerLogger,
                                x => RiverLayerFilter(x),
                                x => RiverLayerFillBrush(x),
                                x => RiverLayerPen(x)));

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

    #region rivers predicates

    private Pen RiverLayerPen(IVectorFeature vectorFeature)
    {
        return new Pen(RandomBrush(), 1);
    }

    public List<IVectorFeature> lol = new List<IVectorFeature>();

    private bool RiverLayerFilter(IVectorFeature vectorFeature)
    {
        lol.Add(vectorFeature);
        return false;
        return vectorFeature.VectorStructured.Data.TryGetValue("type", out var value) ? value.ToString() != "island" : false;
    }

    private Brush RiverLayerFillBrush(IVectorFeature vectorFeature)
    {
        return Brushes.Transparent;
    }

    #endregion

    #region cells predicates

    private Pen CellsLayerPen(IVectorFeature vectorFeature)
    {
        return new Pen(Brushes.PaleGreen, 0.5);
    }

    private bool CellsLayerFilter(IVectorFeature vectorFeature)
    {
        return vectorFeature.VectorStructured.Data.TryGetValue("type", out var value) ? value.ToString() != "island" : false;
    }

    private Brush CellsLayerFillBrush(IVectorFeature vectorFeature)
    {
        return Brushes.PaleGreen;
    }

    #endregion

    private Brush RandomBrush()
    {
        int r = _random.Next(0, 256),
            g = _random.Next(0, 256),
            b = _random.Next(0, 256);
        return new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
    }

    private void UpdateLayout()
    {
        OnMapUpdated?.Invoke(this);
    }
}