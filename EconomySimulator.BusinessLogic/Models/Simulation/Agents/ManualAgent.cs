using EconomySimulator.BusinessLogic.Models.Simulation.Layers;
using Mars.Interfaces.Agents;

namespace EconomySimulator.BusinessLogic.Models.Simulation.Agents;

public class ManualAgent : IAgent<AgentLayer>
{
    public AgentLayer AgentLayer { get; set; }

    public Guid ID { get; set; }
    
    public void Init(AgentLayer layer)
    {
        AgentLayer = layer;
    }

    public void Tick()
    {
        //TODO: Implement this!
    }
}