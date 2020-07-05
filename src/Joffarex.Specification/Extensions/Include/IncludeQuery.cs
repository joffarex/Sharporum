using System.Collections.Generic;

namespace Joffarex.Specification.Extensions.Include
{
    public class IncludeQuery<TEntity, TPreviousProperty> : IIncludeQuery<TEntity, TPreviousProperty>
    {
        public IncludeQuery(Dictionary<IIncludeQuery, string> pathMap)
        {
            PathMap = pathMap;
        }

        public Dictionary<IIncludeQuery, string> PathMap { get; } = new Dictionary<IIncludeQuery, string>();
        public IncludeVisitor Visitor { get; } = new IncludeVisitor();

        // ToHashSet is only available if netstandard2.1 is used.
        public HashSet<string> Paths
        {
            get
            {
                var pathSet = new HashSet<string>();

                foreach (var kvp in PathMap)
                {
                    pathSet.Add(kvp.Value);
                }

                return pathSet;
            }
        }
    }
}