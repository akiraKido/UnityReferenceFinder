using System;

namespace UnityReferenceFinder.YamlParser.Nodes
{
    public class YamlScalar : YamlNode
    {
        public override YamlNodeType YamlNodeType => YamlNodeType.Scalar;

        public override YamlNode this[string key] => throw new NotSupportedException();

        public override string AsString()
        {
            return Value;
        }

        public override int AsInt() => int.Parse(Value);

        public override bool TryAsInt(out int result) => int.TryParse(Value, out result);

        public string Value { get; }

        public YamlScalar(ReadOnlySpan<char> value)
        {
            Value = value.ToString();
        }
    }
}