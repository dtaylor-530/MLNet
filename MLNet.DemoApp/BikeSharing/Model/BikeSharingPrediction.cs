using Microsoft.ML.Data;
using MLNet.RegressionChart;

namespace BikeSharingDemand.BikeSharingDemandData
{
    public class BikeSharingDemandPrediction:IScore
    {
        [ColumnName("Label")]
        public float Label;

        public float Target => Label;


        [ColumnName("Score")]
        public float Score { get; set; }
        //Predicted Count
    }
}
