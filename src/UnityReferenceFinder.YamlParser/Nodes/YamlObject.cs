using System.Collections.Generic;

namespace UnityReferenceFinder.YamlParser.Nodes
{
    public class YamlObject : YamlNode
    {
        public override YamlNodeType YamlNodeType => YamlNodeType.Object;
        public override YamlNode this[string key] => Values[key];

        public override int Count() => Values.Count;

        public IReadOnlyDictionary<string, YamlNode> Values { get; }

        public YamlObject(IReadOnlyDictionary<string, YamlNode> values)
        {
            Values = values;
        }
    }
}