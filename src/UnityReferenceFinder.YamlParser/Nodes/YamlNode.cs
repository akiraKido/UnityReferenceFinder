using System;

namespace UnityReferenceFinder.YamlParser.Nodes
{
    public abstract class YamlNode
    {
        public abstract YamlNodeType YamlNodeType { get; }

        public virtual YamlNode this[string key] => throw new NotSupportedException();
        public virtual YamlNode this[int index] => throw new NotSupportedException();

        public virtual int Count() => throw new NotSupportedException();

        public virtual string AsString() => throw new NotSupportedException();

        public virtual int AsInt() => throw new NotSupportedException();
        public virtual bool TryAsInt(out int result) => throw new NotSupportedException();

        public T As<T>() where T : YamlNode => (T) this;
    }
}