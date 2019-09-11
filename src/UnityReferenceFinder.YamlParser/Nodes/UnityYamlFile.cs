using System;
using System.Collections.Generic;

namespace UnityReferenceFinder.YamlParser.Nodes
{
    public class UnityYamlFile : YamlNode
    {
        public override YamlNodeType YamlNodeType => YamlNodeType.UnityYamlFile;

        public override YamlNode this[string key] => Block[key];

        public string FileId { get; }
        public string UnityId { get; }

        public IReadOnlyDictionary<string, YamlNode> Block { get; }

        internal UnityYamlFile(
            ReadOnlySpan<char> unityId,
            ReadOnlySpan<char> fileId,
            IReadOnlyDictionary<string, YamlNode> block)
        {
            FileId = fileId.ToString();
            UnityId = unityId.ToString();
            Block = block;
        }
    }
}