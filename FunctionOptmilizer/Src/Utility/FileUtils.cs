using CsvHelper;
using FunctionOptimizer.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Windows.Storage;
using Windows.System;

namespace FunctionOptimizer.Utility
{
    static class FileUtils
    {
        public async static void SaveResultsToFile(List<FunctionResult> functionResults)
        {
            StorageFolder localFolder = ApplicationData.Current.TemporaryFolder;
            string path = localFolder.Path + "/results.csv";
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(functionResults);
            }
            Debug.WriteLine("Data saved to: " + path);

            var folder = await StorageFolder.GetFolderFromPathAsync(localFolder.Path);
            await Launcher.LaunchFolderAsync(folder);
        }
    }
}
