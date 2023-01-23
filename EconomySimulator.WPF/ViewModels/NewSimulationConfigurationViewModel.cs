using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
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
    
    private readonly string _outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EconomySimulator\\Configurations\\Active";

    public RelayCommand<GisLayerTypeEnum> SelectFilePathCommand { get; }

    public NewSimulationConfigurationViewModel()
    {
        SelectFilePathCommand = new RelayCommand<GisLayerTypeEnum>(ExecuteSelectFilePathCommand);
    }

    [Localizable(false)]
    public void PrepareFiles()
    {
        if (Directory.Exists(_outputDirectory))
            Directory.Delete(_outputDirectory, true);
        
        string cellsFilePath = _outputDirectory + "\\cells\\cells.geojson",
            riversFilePath = _outputDirectory + "\\rivers\\rivers.geojson",
            routesFilePath = _outputDirectory + "\\routes\\routes.geojson",
            markersFilePath = _outputDirectory + "\\markers\\markers.geojson";
        
        Directory.CreateDirectory(Path.GetDirectoryName(cellsFilePath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(riversFilePath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(routesFilePath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(markersFilePath)!);

        if (string.IsNullOrWhiteSpace(GisCellsLayerFilePath)
            || string.IsNullOrWhiteSpace(GisRiversLayerFilePath)
            || string.IsNullOrWhiteSpace(GisRoutesLayerFilePath)
            || string.IsNullOrWhiteSpace(GisMarkersLayerFilePath))
            throw new FileNotFoundException($"A layer's path was empty or null!" +
                                            $"\n cells: {GisCellsLayerFilePath}" +
                                            $"\n rivers: {GisRiversLayerFilePath}" +
                                            $"\n routes: {GisRoutesLayerFilePath}" +
                                            $"\n markers: {GisMarkersLayerFilePath}");

        if (!File.Exists(GisCellsLayerFilePath)
            || !File.Exists(GisRiversLayerFilePath)
            || !File.Exists(GisRoutesLayerFilePath)
            || !File.Exists(GisMarkersLayerFilePath))
            throw new FileNotFoundException($"A layer's file could not be found!" +
                                            $"\n cells: {GisCellsLayerFilePath}" +
                                            $"\n rivers: {GisRiversLayerFilePath}" +
                                            $"\n routes: {GisRoutesLayerFilePath}" +
                                            $"\n markers: {GisMarkersLayerFilePath}");
        
        File.Copy(GisCellsLayerFilePath, cellsFilePath);
        File.Copy(GisRiversLayerFilePath, riversFilePath);
        File.Copy(GisRoutesLayerFilePath, routesFilePath);
        File.Copy(GisMarkersLayerFilePath, markersFilePath);
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

    [Localizable(false)]
    public SimulationConfig ToSimulationConfig()
    {
        PrepareFiles();
        
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
                    File = _outputDirectory + "\\cells\\cells.geojson"
                },
                new()
                {
                    Name = nameof(GisRiverLayer),
                    File = _outputDirectory + "\\rivers\\rivers.geojson"
                },
                new()
                {
                    Name = nameof(GisRoutesLayer),
                    File = _outputDirectory + "\\routes\\routes.geojson"
                },
                new()
                {
                    Name = nameof(GisMarkersLayer),
                    File = _outputDirectory + "\\markers\\markers.geojson"
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