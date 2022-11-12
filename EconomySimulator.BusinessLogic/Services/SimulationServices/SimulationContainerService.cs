using EconomySimulator.BusinessLogic.Models.Simulation.Agents;
using EconomySimulator.BusinessLogic.Models.Simulation.Layers;
using Mars.Components.Starter;
using Mars.Core.Simulation;
using Mars.Core.Simulation.Entities;
using Mars.Interfaces.Model;

namespace EconomySimulator.BusinessLogic.Services.SimulationServices;

public class SimulationContainerService : ISimulationContainerService
{
    public ModelDescription ModelDescription { get; private set; }
    public SimulationConfig SimulationConfiguration { get; private set; }
    public ISimulation Simulation { get; private set; }
    public SimulationWorkflowState CurrentSimulationWorkflowState { get; private set; }
    
    /// <inheritdoc />
    public void LoadSimulationConfiguration(string configPath)
    {
        if (!File.Exists(configPath))
            throw new ArgumentException("Path does not exist!");

        var fileContent = File.ReadAllText(configPath);
        var simulationConfig = SimulationConfig.Deserialize(fileContent);
        
        LoadSimulationConfiguration(simulationConfig);
    }

    /// <inheritdoc />
    public void LoadSimulationConfiguration(SimulationConfig config)
    {
        SimulationConfiguration = config;
    }

    /// <inheritdoc />
    public void InitSimulation()
    {
        if (SimulationConfiguration == null)
            throw new NullReferenceException($"{nameof(SimulationConfiguration)} may not be null!");

        ModelDescription ??= InitModelDescription();

        Simulation = SimulationStarter.Build(ModelDescription, SimulationConfiguration);
        CurrentSimulationWorkflowState = Simulation.PrepareSimulation(ModelDescription, SimulationConfiguration);
    }

    private ModelDescription InitModelDescription()
    {
        var description = new ModelDescription()
        {
            SimulationConfig = SimulationConfiguration
        };

        description.AddLayer<GisCellsLayer>();
        description.AddLayer<GisRiverLayer>();
        description.AddLayer<GisRoutesLayer>();
        description.AddLayer<GisMarkersLayer>();
        description.AddLayer<AgentLayer>();

        description.AddAgent<ManualAgent, AgentLayer>();

        return description;
    }
}