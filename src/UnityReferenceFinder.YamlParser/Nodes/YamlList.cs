using System.Collections.Generic;

namespace UnityReferenceFinder.YamlParser.Nodes
{
    public class YamlList : YamlNode
    {
        public override YamlNodeType YamlNodeType => YamlNodeType.List;

        public override YamlNode this[int index] => Values[index];

        public override int Count() => Values.Count;

        public IReadOnlyList<YamlNode> Values { get; }

        public YamlList(IReadOnlyList<YamlNode> values)
        {
            Values = values;
        }
    }
}