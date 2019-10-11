using Microsoft.ML.Data;

namespace RegressionChart.Sample
{
    public class InputRow
    {
        [LoadColumn(1), ColumnName("Label")]
        public float Salary;

        [LoadColumn(3)]
        public float NBA_DraftNumber;

        [LoadColumn(4)]
        public float Age;

        [LoadColumn(22)]
        public float Ws;

        [LoadColumn(26)]
        public float Bmp;
    }
}
