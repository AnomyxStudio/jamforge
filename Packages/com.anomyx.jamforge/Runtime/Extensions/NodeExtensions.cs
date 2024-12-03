using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JamForge
{
    public static class NodeExtensions
    {
        private const string Wildcard = "*";
        private static readonly Regex SeparatorRegex = new(@"[\.\\\/]", RegexOptions.Compiled);

        public static void EnsureNodePath<TData>(this NodeData<TData> rootNode, string route, Func<string, NodeData<TData>> nodeCreation)
        {
            const int cursor = 0;
            var routes = SeparatorRegex.Split(route);

            EnsureNodePath(rootNode, cursor, routes, nodeCreation);
        }

        public static IEnumerable<NodeData<TData>> GetNodes<TData>(this NodeData<TData> rootNode, string path)
        {
            const int cursor = 0; 
            var routes = SeparatorRegex.Split(path);
            var results = new List<NodeData<TData>>();

            GetNodes(rootNode, cursor, routes, results);
            return results;
        }

        private static void GetNodes<TData>(NodeData<TData> rootNode, int cursor, IReadOnlyList<string> path, ICollection<NodeData<TData>> results)
        {
            if (cursor == path.Count)
            {
                results.Add(rootNode);
                return;
            }

            var route = path[cursor];
            var isWildcard = route.Equals(Wildcard);

            if (isWildcard)
            {
                foreach (var child in rootNode.Children.Values)
                {
                    GetNodes(child, cursor + 1, path, results);
                }
            }
            else
            {
                if (rootNode.Children.TryGetValue(route, out var child))
                {
                    GetNodes(child, cursor + 1, path, results);
                }
            }
        }
        
        private static void EnsureNodePath<TData>(NodeData<TData> rootNode, int cursor, IReadOnlyList<string> routes, Func<string, NodeData<TData>> nodeCreation)
        {
            if (cursor == routes.Count) { return; }

            var route = routes[cursor];
            var isWildcard = route.Equals(Wildcard);

            if (isWildcard)
            {
                foreach (var child in rootNode.Children.Values)
                {
                    EnsureNodePath(child, cursor + 1, routes, nodeCreation);
                }
            }
            else
            {
                if (!rootNode.Children.TryGetValue(route, out var child))
                {
                    child = nodeCreation?.Invoke(route);
                    rootNode.AddChild(child);
                }

                EnsureNodePath(child, cursor + 1, routes, nodeCreation);
            }
        }
    }
}
