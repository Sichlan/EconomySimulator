using EconomySimulator.BusinessLogic.Models.Simulation.Agents;
using EconomySimulator.BusinessLogic.Models.Simulation.Layers;
using EconomySimulator.BusinessLogic.Services.FileServices;
using Mars.Components.Starter;
using Mars.Core.Simulation;
using Mars.Core.Simulation.Entities;
using Mars.Interfaces.Model;
using Mars.Interfaces.Model.Options;
using Newtonsoft.Json;

namespace EconomySimulator.BusinessLogic.Services.SimulationServices;

public class SimulationContainerService : ISimulationContainerService
{
    private readonly IZipFileService _zipFileService;
    private SimulationConfig? _simulationConfiguration;
    private readonly string _outputDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EconomySimulator\\Configurations\\Active";
    private readonly JsonSerializerSettings? _jsonSerializerSettings = new JsonSerializerSettings
    {
        PreserveReferencesHandling = PreserveReferencesHandling.Objects, 
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Formatting = Formatting.Indented
    };

    public SimulationContainerService(IZipFileService zipFileService)
    {
        _zipFileService = zipFileService;
    }

    public event ISimulationContainerService.SimulationInitiatedEventHandler? OnSimulationInitiated; 
    public event ISimulationContainerService.SimulationStepEventHandler? OnSimulationStep;
    public event ISimulationContainerService.SimulationConfigChangedHandler? OnSimulationConfigChanged;

    #region Properties

    public ModelDescription? ModelDescription { get; private set; }

    public SimulationConfig? SimulationConfiguration
    {
        get
        {
            return _simulationConfiguration;
        }
        private set
        {
            _simulationConfiguration = value;
            OnSimulationConfigChanged?.Invoke(this);
        }
    }

    public ISimulation? Simulation { get; private set; }
    public SimulationWorkflowState? CurrentSimulationWorkflowState { get; private set; }

    #endregion

    /// <inheritdoc />
    public async Task LoadSimulationConfiguration(string filePath)
    {
        if (!File.Exists(filePath))
            throw new ArgumentException("Path does not exist!");
        
        var configPath = _outputDirectory + "\\config.json";

        await _zipFileService.ReadSimulationConfigurationArchive(filePath, _outputDirectory);

        if (!File.Exists(configPath))
            throw new FileNotFoundException("Can't find config file in archive");

        var fileContent = string.Join('\n',(await File.ReadAllLinesAsync(configPath)).Where(x => !x.Contains("outputKind") && !x.Contains("encoding")));
        var simulationConfig = JsonConvert.DeserializeObject<SimulationConfig>(fileContent, _jsonSerializerSettings);

        if (simulationConfig == null)
            throw new NullReferenceException("Deserialized config was null!");
        
        await LoadSimulationConfiguration(simulationConfig);
        ModelDescription = null;
    }

    /// <inheritdoc />
    public async Task LoadSimulationConfiguration(SimulationConfig config)
    {
        AnnihilateSimulation();
        
        LayerMapping cellsLayer, riversLayer, routesLayer, markersLayer;

        if ((cellsLayer = config.LayerMappings.First(x => x.Name == nameof(GisCellsLayer))) == null)
            throw new ArgumentException("No cells layer found!");
        if ((riversLayer = config.LayerMappings.First(x => x.Name == nameof(GisRiverLayer))) == null)
            throw new ArgumentException("No rivers layer found!");
        if ((routesLayer = config.LayerMappings.First(x => x.Name == nameof(GisRoutesLayer))) == null)
            throw new ArgumentException("No routes layer found!");
        if ((markersLayer = config.LayerMappings.First(x => x.Name == nameof(GisMarkersLayer))) == null)
            throw new ArgumentException("No markers layer found!");

        cellsLayer.File = _outputDirectory + "\\cells\\cells.geojson";
        riversLayer.File = _outputDirectory + "\\rivers\\rivers.geojson";
        routesLayer.File = _outputDirectory + "\\routes\\routes.geojson";
        markersLayer.File = _outputDirectory + "\\markers\\markers.geojson";
        
        Directory.CreateDirectory(Path.GetDirectoryName(cellsLayer.File)!);
        Directory.CreateDirectory(Path.GetDirectoryName(riversLayer.File)!);
        Directory.CreateDirectory(Path.GetDirectoryName(routesLayer.File)!);
        Directory.CreateDirectory(Path.GetDirectoryName(markersLayer.File)!);
        
        SimulationConfiguration = config;
    }

    /// <inheritdoc />
    public async Task SaveSimulationConfiguration(string path)
    {
        if (SimulationConfiguration == null)
            throw new NullReferenceException(nameof(SimulationConfiguration) + " may not be null!");

        LayerMapping cellsLayer, riversLayer, routesLayer, markersLayer;

        if ((cellsLayer = SimulationConfiguration.LayerMappings.First(x => x.Name == nameof(GisCellsLayer))) == null)
            throw new ArgumentException("No cells layer found!");
        if ((riversLayer = SimulationConfiguration.LayerMappings.First(x => x.Name == nameof(GisRiverLayer))) == null)
            throw new ArgumentException("No rivers layer found!");
        if ((routesLayer = SimulationConfiguration.LayerMappings.First(x => x.Name == nameof(GisRoutesLayer))) == null)
            throw new ArgumentException("No routes layer found!");
        if ((markersLayer = SimulationConfiguration.LayerMappings.First(x => x.Name == nameof(GisMarkersLayer))) == null)
            throw new ArgumentException("No markers layer found!");

        var directoryPath = Path.GetDirectoryName(path);

        if (string.IsNullOrEmpty(directoryPath))
            throw new DirectoryNotFoundException();

        string cellsPath = cellsLayer.File,
            riversPath = riversLayer.File,
            routesPath = routesLayer.File,
            markersPath = markersLayer.File;

        // var config = SimulationConfiguration.Serialize();
        var config = JsonConvert.SerializeObject(SimulationConfiguration, _jsonSerializerSettings); 
            
        Directory.CreateDirectory(directoryPath);
        
        await _zipFileService.CreateSimulationConfigurationArchive(path,
            config,
            cellsPath,
            riversPath,
            routesPath,
            markersPath);
    }

    /// <inheritdoc />
    public void InitSimulation()
    {
        if (SimulationConfiguration == null)
            throw new NullReferenceException($"{nameof(SimulationConfiguration)} may not be null!");

        ModelDescription ??= InitModelDescription();

        Simulation = SimulationStarter.Build(ModelDescription, SimulationConfiguration);
        CurrentSimulationWorkflowState = Simulation.PrepareSimulation(ModelDescription, SimulationConfiguration);
        
        OnSimulationInitiated?.Invoke(this);
    }

    public void AnnihilateSimulation()
    {
        SimulationConfiguration = null;
        Simulation = null;
        ModelDescription = null;
        CurrentSimulationWorkflowState = null;
    }

    private ModelDescription InitModelDescription()
    {
        var description = new ModelDescription
        {
            SimulationConfig = SimulationConfiguration
        };

        description.AddLayer<GisCellsLayer>();
        description.AddLayer<GisRiverLayer>();
        description.AddLayer<GisRoutesLayer>();
        description.AddLayer<GisMarkersLayer>();
        // description.AddLayer<AgentLayer>();
        
        // description.AddAgent<ManualAgent, AgentLayer>();

        return description;
    }
    
    public static SimulationConfig GenerateExampleConfiguration()
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
                }
            },
            LayerMappings = new List<LayerMapping>
            {
                new()
                {
                    Name = nameof(GisCellsLayer),
                    File = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\EconomySimulator\\Configurations\\Active\\cells\\cells.geojson"
                }
            },
            AgentMappings = new List<AgentMapping>
            {
                new()
                {
                    Name = nameof(ManualAgent),
                    InstanceCount = 1
                }
            },
            Execution = new Execution
            {
                NodeNumber = 0
            }
        };
    }
}