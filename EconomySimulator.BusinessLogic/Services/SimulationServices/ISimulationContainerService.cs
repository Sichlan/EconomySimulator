using Mars.Core.Simulation;
using Mars.Core.Simulation.Entities;
using Mars.Interfaces.Model;

namespace EconomySimulator.BusinessLogic.Services.SimulationServices;

public interface ISimulationContainerService
{
    ModelDescription ModelDescription { get; }
    SimulationConfig SimulationConfiguration { get; }
    ISimulation Simulation { get; }
    SimulationWorkflowState CurrentSimulationWorkflowState { get; }
    
    /// <summary>
    /// Loads a <see cref="SimulationConfiguration"/> from a given path and executes <see cref="LoadSimulationConfiguration(SimulationConfig)"/> with it.
    /// </summary>
    /// <param name="configPath">The path to the config file.</param>
    /// <exception cref="ArgumentException">Throws an ArgumentException if the file path is invalid.</exception>
    void LoadSimulationConfiguration(string configPath);

    /// <summary>
    /// Loads a <see cref="SimulationConfiguration"/> instance. 
    /// </summary>
    /// <param name="config"></param>
    void LoadSimulationConfiguration(SimulationConfig config);

    /// <summary>
    /// Initializes the simulation and generates a <see cref="ModelDescription"/>, <see cref="ISimulation"/> and <see cref="SimulationWorkflowState"/>.
    /// </summary>
    void InitSimulation();
}