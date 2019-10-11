using Microsoft.ML.Data;

namespace RegressionChart.Sample
{
    public class OutputRow: MLNet.RegressionChart.IScore
    {
        [ColumnName("Label")]
        public float Salary;


        public float Target => Salary;


        [ColumnName("Score")]
        public float Score { get; set; }
    }
}
