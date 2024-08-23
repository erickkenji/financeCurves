namespace Curves
{
    public interface ICurve
    {
        double Rate(DateTime time);
        double Factor(DateTime startDate, DateTime endDate);
        double DiscountFactor(DateTime startDate, DateTime endDate);
        INode GetNode(DateTime time);
    }
}
