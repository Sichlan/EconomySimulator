using EconomySimulator.BusinessLogic.Services.FileServices;
using EconomySimulator.BusinessLogic.Services.SimulationServices;
using NUnit.Framework;

namespace EconomySimulator.BusinessLogicTests.Services.SimulationServices;

public class SimulationContainerServiceTests
{
    [Test]
    public async Task SaveSimulationConfiguration_CorrectPath_SaveFileTo()
    {
        //Arrange
        var outputDirectory = Directory.GetCurrentDirectory() + "\\TestOutput";
        if(Directory.Exists(outputDirectory))
            Directory.Delete(outputDirectory, true);

        var filePath = outputDirectory + "\\SaveFileConfig.json";
        
        var simulationContainerService = new SimulationContainerService(new ZipFileService());
        simulationContainerService.LoadSimulationConfiguration(SimulationContainerService.GenerateExampleConfiguration());

        //Act
        await simulationContainerService.SaveSimulationConfiguration(filePath);

        //Assert
    }
}