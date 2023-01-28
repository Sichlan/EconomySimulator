using EconomySimulator.BusinessLogic.Services.FileServices;
using NUnit.Framework;
using Serilog;

namespace EconomySimulator.BusinessLogicTests.Services.FileServices;

public class ZipFileServiceTests
{
    [Test]
    public async Task CreateTestZipFile()
    {
        // TODO: fix this. Logger is missing from zipFileService
        // var outputDirectory = Directory.GetCurrentDirectory() + "\\TestOutput";
        // if(Directory.Exists(outputDirectory))
        //     Directory.Delete(outputDirectory, true);
        //
        // var filePath = outputDirectory + "\\SaveFileConfig.zip";
        //
        // var zipFileService = new ZipFileService();
        //
        // await zipFileService.CreateSimulationConfigurationArchive(filePath,
        //     "test",
        //     "F:\\_Artworks\\Touhou\\Tarot\\The Sun.png",
        //     "F:\\_Artworks\\Touhou\\Tarot\\The Youkai of Boundaries.png",
        //     "F:\\_Artworks\\Touhou\\Tarot\\The Servant.png",
        //     "F:\\_Artworks\\Touhou\\Tarot\\The Puppet Master.png");
    }
}