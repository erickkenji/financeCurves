using System;

namespace Curves
{
    public class ForwardCurve : ICurve
    {
        private List<INode> nodes;

        public ForwardCurve(List<INode> nodes)
        {
            this.nodes = nodes;
            LinkNodes();
        }

        private void LinkNodes()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i > 0)
                {
                    nodes[i].PreviousNode = nodes[i - 1];
                }
                if (i < nodes.Count - 1)
                {
                    nodes[i].NextNode = nodes[i + 1];
                }
            }
        }

        public double Rate(DateTime time)
        {
            // Retorna a taxa forward para o tempo fornecido
            INode node = GetNode(time);
            return node?.Rate ?? InterpolateRate(time);
        }

        public double Factor(DateTime time)
        {
            // Retorna o fator forward a partir da taxa forward
            double timeInYears = (time - DateTime.Today).TotalDays / 365.0;
            return Math.Exp(-Rate(time) * timeInYears);
        }

        public SpotCurve ToSpotCurve()
        {
            List<INode> spotNodes = new List<INode>();

            foreach (var node in nodes)
            {
                spotNodes.Add(new Node(node.Time, ConvertForwardToSpot(node)));
            }

            return new SpotCurve(spotNodes);
        }

        private double InterpolateRate(DateTime time)
        {
            // Implementar lógica de interpolação entre nodes
            return 0.0;
        }

        private double ConvertForwardToSpot(INode node)
        {
            // Lógica para converter a taxa forward em spot
            return node.Rate;
        }

        public INode GetNode(DateTime time)
        {
            return nodes.Find(n => n.Time == time);
        }
    }
}
