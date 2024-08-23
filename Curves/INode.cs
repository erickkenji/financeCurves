using System;
using System.Collections;
using System.Collections.Generic;

namespace Curves
{
    public interface INode
    {
        DateTime Date { get; }
        double Rate { get; }
    }

    public class Node : INode
    {
        public DateTime Date { get; private set; }
        public double Rate { get; private set; }

        public Node(DateTime date, double rate)
        {
            Date = date;
            Rate = rate;
        }
    }

    public class NodeCollection : IReadOnlyCollection<INode>
    {
        private readonly INode[] ascendingNodes;
        private readonly List<INode> unorderedNodes;

        public NodeCollection(List<INode> nodes)
        {
            this.unorderedNodes = nodes;
            this.ascendingNodes = unorderedNodes
                                    .OrderBy(node => node.Date)
                                    .ToArray();
        }

        #region IReadonlyCollection members

        public int Count
        {
            get { return ascendingNodes.Length; }
        }

        public IEnumerator<INode> GetEnumerator()
        {
            return ((IEnumerable<INode>)this.ascendingNodes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
