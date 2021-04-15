using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace ExportTfsChangsetFiles
{
    namespace GetTfsChangeSet
    {
        class Program
        {
            static void Main(string[] args)
            {
                //note: this is for tfs 2017
                //note: need to install old versions of tfs nuget package
                //Install-Package Microsoft.TeamFoundationServer.ExtendedClient -Version 15.112.1

                string teamProjectCollectionUrl = @"";
                List<int> changesetIds = new List<int>() { };
                string outputDir = @"C:\Temp";

                TfsTeamProjectCollection teamProjectCollection = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(teamProjectCollectionUrl));
                string downloadPath = "";

                foreach (var changesetId in changesetIds.OrderBy(c => c))
                {
                    downloadPath = Path.Combine(outputDir, $"{changesetId.ToString()}");
                    var files = GetFiles(teamProjectCollection, changesetId, downloadPath);
                    Console.WriteLine($"ChangeSet {changesetId} done");
                }

                Console.WriteLine("Done.");
                Console.ReadKey();
            }
            private static List<string> GetFiles(TfsTeamProjectCollection teamProjectCollection, int changesetId, string downloadPath)
            {
                List<string> files = new List<string>();
                VersionControlServer versionControlServer = teamProjectCollection.GetService(typeof(VersionControlServer)) as VersionControlServer;
                Changeset changeset = versionControlServer.GetChangeset(changesetId);

                if (changeset.Changes != null)
                {
                    foreach (var changedItem in changeset.Changes)
                    {
                        var item = changedItem.Item;
                        if (item.ItemType != ItemType.File || item.DeletionId != 0)
                            continue;

                        var outFilename = Path.Combine(downloadPath, item.ServerItem.Replace("$/", "").Replace("/", @"\"));

                        item.DownloadFile(outFilename);
                        files.Add(outFilename);
                    }
                }
                return files;
            }
        }
    }
}