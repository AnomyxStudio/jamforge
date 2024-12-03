using System;
using System.Collections.Generic;

namespace JamForge
{
    public abstract class NodeData<TData>
    {
        public string Path { get; }

        public TData Data { get; }

        public NodeData<TData> Parent { get; private set; }

        public Dictionary<string, NodeData<TData>> Children { get; } = new();

        public NodeData(string path, TData data)
        {
            Path = path;
            Data = data;
        }

        private void SetParent(NodeData<TData> node)
        {
            Parent = node;
        }

        public void AddChild(NodeData<TData> node)
        {
            if (node == null) { throw new NullReferenceException(); }

            if (Children.TryAdd(node.Path, node))
            {
                node.SetParent(this);
            }
        }
    }
}
