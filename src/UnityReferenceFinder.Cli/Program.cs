using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityReferenceFinder.YamlParser;
using UnityReferenceFinder.YamlParser.Nodes;

namespace UnityReferenceFinder.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var randomScenePath = @"C:\Users\dohi\Documents\repos\OnAirV2\Assets\Scenes\TestSite.unity";

            var text = File.ReadAllText(randomScenePath);
            
            var yaml = new UnityYamlTree().Parse(text);
            Console.WriteLine(yaml.As<UnityScene>().UnityYamlFiles.Count);
            
            
            return;
            
            var assetsPath = @"C:\Users\dohi\Documents\repos\OnAirV2\Assets";
            var targetFile = @"C:\Users\dohi\Documents\repos\OnAirV2\Assets\Textures\Common\common_ui.png";
            var targetImageName = "btn_common_01";

            var meta = targetFile + ".meta";
            var guid = GetGuid(meta);
            var fileIdToRecycle = GetFileIdToRecycle(meta, targetImageName);

            var directoryInfo = new DirectoryInfo(assetsPath);
            var scenes = FindFilesWithExtension(directoryInfo, ".unity", true);

            foreach (var scene in scenes)
            {
                var usingGameObjects = UsingGameObjects(scene.FullName, guid, fileIdToRecycle);
                if (usingGameObjects.Any() == false) continue;

                Console.WriteLine(scene.Name + ":");
                foreach (var gameObject in usingGameObjects)
                {
                    Console.WriteLine($"  {gameObject}");
                }
            }
        }

        private static string GetFileIdToRecycle(string metaFilePath, string targetImageName)
        {
            foreach (var line in File.ReadLines(metaFilePath))
            {
                if (line.Contains(targetImageName))
                {
                    return line.Substring(0, line.IndexOf(':')).Trim();
                }
            }

            throw new Exception($"Target image name not found: {targetImageName}");
        }

        private static IEnumerable<FileInfo> FindFilesWithExtension(DirectoryInfo directoryInfo, string extension,
            bool recursive = false)
        {
            foreach (var file in directoryInfo.EnumerateFiles())
            {
                if (file.Extension == extension)
                {
                    yield return file;
                }
            }

            if (recursive == false) yield break;

            foreach (var directory in directoryInfo.EnumerateDirectories())
            {
                foreach (var fileInfo in FindFilesWithExtension(directory, extension, true))
                {
                    yield return fileInfo;
                }
            }
        }

        private static string GetGuid(string metaFilePath)
        {
            var fileInfo = new FileInfo(metaFilePath);
            if (fileInfo.Extension != ".meta") throw new Exception("file path must be of extension .meta");

            foreach (var line in File.ReadLines(metaFilePath))
            {
                if (line.StartsWith("guid: "))
                {
                    return line.Substring("guid: ".Length);
                }
            }

            throw new Exception($"No guid found for file: {metaFilePath}");
        }

        private static List<string> UsingGameObjects(string scenePath, string fileGuid, string fileIdToRecycle)
        {
            var fileInfo = new FileInfo(scenePath);
            if (fileInfo.Extension != ".unity") throw new Exception($"Path must be scene (*.unity): {scenePath}");

            var result = new List<string>();
            var lines = File.ReadAllLines(scenePath);

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.Contains(fileGuid) && line.Contains(fileIdToRecycle))
                {
                    var ii = i;
                    var name = string.Empty;
                    while (true)
                    {
                        ii -= 1;
                        line = lines[ii];
                        if (line.Contains("m_Name"))
                        {
                            name = line.Substring(line.IndexOf(':') + 1).Trim();
                            continue;
                        }

                        if (line.StartsWith("GameObject:"))
                        {
                            break;
                        }
                    }
                    result.Add(name);
                }
            }

            return result;
        }
    }
}