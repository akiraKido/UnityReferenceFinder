using System.Collections.Generic;

namespace UnityReferenceFinder.YamlParser.Nodes
{
    public class UnityScene : YamlNode
    {
        public IReadOnlyList<YamlNode> UnityYamlFiles { get; }

        public UnityScene(IReadOnlyList<YamlNode> unityYamlFiles)
        {
            UnityYamlFiles = unityYamlFiles;
        }

        public override YamlNodeType YamlNodeType => YamlNodeType.UnityScene;

        public override YamlNode this[int index] => UnityYamlFiles[index];
    }
}