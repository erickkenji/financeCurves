using System;
using System.Collections;
using System.Collections.Generic;

namespace Curves
{
    public class SpotCurve
    {
        private NodeCollection nodes;

        public SpotCurve(NodeCollection nodes)
        {
            this.nodes = nodes;
        }

        //public double Rate(DateTime time)
        //{
            //// Retorna a taxa spot para o tempo fornecido
            //INode node = GetNode(time);
            //return node?.Rate ?? InterpolateRate(time);
        //}

        //public double Factor(DateTime time)
        //{
        //    // Retorna o fator de desconto a partir da taxa spot
        //    double timeInYears = (time - DateTime.Today).TotalDays / 365.0;
        //    return Math.Exp(-Rate(time) * timeInYears);
        //}

        //public ForwardCurve ToForwardCurve()
        //{
        //    List<INode> forwardNodes = new List<INode>();

        //    foreach (var node in nodes)
        //    {
        //        forwardNodes.Add(new Node(node.Time, ConvertSpotToForward(node)));
        //    }

        //    return new ForwardCurve(forwardNodes);
        //}

        private double InterpolateRate(DateTime time)
        {
            // Implementar lógica de interpolação entre nodes
            return 0.0;
        }

        private double ConvertSpotToForward(INode node)
        {
            // Lógica para converter a taxa spot em forward
            return node.Rate;
        }

        //public INode GetNode(DateTime time)
        //{
        //    return nodes.Find(n => n.Time == time);
        //}
    }
   
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        // Exemplo de criação de buckets
    //        List<INode> spotBuckets = new List<INode>
    //    {
    //        new Bucket(new DateTime(2025, 8, 23), 0.02),
    //        new Bucket(new DateTime(2026, 8, 23), 0.025),
    //        new Bucket(new DateTime(2027, 8, 23), 0.03)
    //    };

    //        SpotCurve spotCurve = new SpotCurve(spotBuckets);
    //        ForwardCurve forwardCurve = spotCurve.ToForwardCurve();

    //        DateTime testDate = new DateTime(2026, 8, 23);
    //        Console.WriteLine($"Spot Rate for {testDate.ToShortDateString()}: {spotCurve.Rate(testDate)}");
    //        Console.WriteLine($"Forward Rate for {testDate.ToShortDateString()}: {forwardCurve.Rate(testDate)}");
    //    }
    //}
}
