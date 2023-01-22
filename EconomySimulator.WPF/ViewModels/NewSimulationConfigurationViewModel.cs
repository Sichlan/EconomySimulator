using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EconomySimulator.BusinessLogic.Models.Simulation.Layers;
using EconomySimulator.WPF.Resources.Enums;
using Mars.Interfaces.Model;
using Mars.Interfaces.Model.Options;
using Microsoft.Win32;

namespace EconomySimulator.WPF.ViewModels;

public partial class NewSimulationConfigurationViewModel : ObservableObject
{
    [ObservableProperty] private string? _gisCellsLayerFilePath;
    [ObservableProperty] private string? _gisRiversLayerFilePath;
    [ObservableProperty] private string? _gisRoutesLayerFilePath;
    [ObservableProperty] private string? _gisMarkersLayerFilePath;

    public RelayCommand<GisLayerTypeEnum> SelectFilePathCommand { get; }

    public NewSimulationConfigurationViewModel()
    {
        SelectFilePathCommand = new RelayCommand<GisLayerTypeEnum>(ExecuteSelectFilePathCommand);
    }

    private void ExecuteSelectFilePathCommand(GisLayerTypeEnum gisLayerTypeEnum)
    {
        var openFileDialog = new OpenFileDialog
        {
            RestoreDirectory = true,
            Title = Resources.Localization.Resources.OpenDialogTitle,
            CheckFileExists = true,
            Filter = "GeoJSON|*.geojson|All files|*.*"
        };

        if (openFileDialog.ShowDialog() != true)
            return;
        
        var path = openFileDialog.FileName;

        switch (gisLayerTypeEnum)
        {
            case GisLayerTypeEnum.Cells:
                GisCellsLayerFilePath = path;
                break;
            case GisLayerTypeEnum.Rivers:
                GisRiversLayerFilePath = path;
                break;
            case GisLayerTypeEnum.Routes:
                GisRoutesLayerFilePath = path;
                break;
            case GisLayerTypeEnum.Markers:
                GisMarkersLayerFilePath = path;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gisLayerTypeEnum), gisLayerTypeEnum, null);
        }
    }

    public SimulationConfig ToSimulationConfig()
    {
        return new SimulationConfig
        {
            Globals = new Globals
            {
                Steps = long.MaxValue,
                OutputTarget = OutputTargetType.Csv,
                CsvOptions = new CsvOptions
                {
                    Delimiter = ";",
                    NumberFormat = "en-EN"
                },
                MongoOptions = null
            },
            LayerMappings = new List<LayerMapping>
            {
                new()
                {
                    Name = nameof(GisCellsLayer),
                    File = GisCellsLayerFilePath
                },
                new()
                {
                    Name = nameof(GisRiverLayer),
                    File = GisRiversLayerFilePath
                },
                new()
                {
                    Name = nameof(GisRoutesLayer),
                    File = GisRoutesLayerFilePath
                },
                new()
                {
                    Name = nameof(GisMarkersLayer),
                    File = GisMarkersLayerFilePath
                },
            },
            AgentMappings = new List<AgentMapping>(),
            Execution = new Execution
            {
                NodeNumber = 0
            }
        };
    }
}