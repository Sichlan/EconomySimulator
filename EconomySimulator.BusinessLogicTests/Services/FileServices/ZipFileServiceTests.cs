using EconomySimulator.BusinessLogic.Services.FileServices;
using NUnit.Framework;

namespace EconomySimulator.BusinessLogicTests.Services.FileServices;

public class ZipFileServiceTests
{
    [Test]
    public async Task CreateTestZipFile()
    {
        var outputDirectory = Directory.GetCurrentDirectory() + "\\TestOutput";
        if(Directory.Exists(outputDirectory))
            Directory.Delete(outputDirectory, true);

        var filePath = outputDirectory + "\\SaveFileConfig.zip";
        
        var zipFileService = new ZipFileService();

        await zipFileService.CreateSimulationConfigurationArchive(filePath,
            "test",
            "F:\\_Artworks\\Touhou\\Tarot\\The Sun.png",
            "F:\\_Artworks\\Touhou\\Tarot\\The Youkai of Boundaries.png",
            "F:\\_Artworks\\Touhou\\Tarot\\The Servant.png",
            "F:\\_Artworks\\Touhou\\Tarot\\The Puppet Master.png");
    }
}