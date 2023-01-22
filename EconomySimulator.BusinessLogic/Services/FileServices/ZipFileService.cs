using System.Diagnostics;
using System.IO.Compression;

namespace EconomySimulator.BusinessLogic.Services.FileServices;

public class ZipFileService : IZipFileService
{
    /// <inheritdoc/>
    public void CreateZipFile(string fileName, IEnumerable<string> files)
    {
        // Create and open a new ZIP file
        var zip = ZipFile.Open(fileName, ZipArchiveMode.Create);
        foreach (var file in files)
        {
            // Add the entry for each file
            zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
        }
        // Dispose of the object when we are done
        zip.Dispose();
    }

    public async Task CreateSimulationConfigurationArchive(string fileName, string configJson, string cellsLayer, string riversLayer, string routesLayer, string markersLayer)
    {
        var outputDirectory = Path.GetDirectoryName(fileName);

        if (string.IsNullOrEmpty(outputDirectory)
            || !File.Exists(cellsLayer)
            || !File.Exists(riversLayer)
            || !File.Exists(routesLayer)
            || !File.Exists(markersLayer))
            throw new ArgumentException();
        
        var basePath = Path.GetTempPath() + $"EconomySimulator\\{Guid.NewGuid()}";

        try
        {
            var cellsPath = basePath + "\\cells";
            var riversPath = basePath + "\\rivers";
            var routesPath = basePath + "\\routes";
            var markersPath = basePath + "\\markers";

            Directory.CreateDirectory(cellsPath);
            Directory.CreateDirectory(riversPath);
            Directory.CreateDirectory(routesPath);
            Directory.CreateDirectory(markersPath);

            var configPath = basePath + "\\config.json";
            await File.WriteAllTextAsync(configPath, configJson);

            File.Copy(cellsLayer, cellsPath + "\\cells.geojson");
            File.Copy(routesLayer, riversPath + "\\rivers.geojson");
            File.Copy(riversLayer, routesPath + "\\routes.geojson");
            File.Copy(markersLayer, markersPath + "\\markers.geojson");

            Directory.CreateDirectory(outputDirectory);
            ZipFile.CreateFromDirectory(basePath, fileName);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
        finally
        {
            Directory.Delete(basePath, true);
        }
    }

    public async Task ReadSimulationConfigurationArchive(string fileName, string outputPath)
    {
        if (!File.Exists(fileName))
            throw new ArgumentException();

        try
        {
            Directory.Delete(outputPath, true);
            Directory.CreateDirectory(outputPath);
            ZipFile.ExtractToDirectory(fileName, outputPath);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }
}