namespace MLNet.RegressionChart
{
    public interface IScore
    {
        float Target { get; }
        float Score { get;  }
    }
}