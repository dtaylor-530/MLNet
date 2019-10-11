
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeSharingDemand.BikeSharingDemandData;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using MLNet.RegressionChart;

namespace MLNet.DemoApp.BikeSharing
{


    public class MLContext2
    {
        public static MLContext Context { get; } = new MLContext();
    }


    public class BikeSharingRegressor : Regressor<BikeSharingDemandSample, BikeSharingDemandPrediction, TransformerChain<RegressionPredictionTransformer<LinearRegressionModelParameters>>>
    {
        public BikeSharingRegressor(TransformerChain<RegressionPredictionTransformer<LinearRegressionModelParameters>> transformer, DataViewSchema schema) :
            base(
                new Path(),
                id: "id",
                mLContext: MLContext2.Context,
                transformer: transformer,
                schema: schema)
        {
        }
    }


    public class BikeSharingRegressorFactory
    {
        public static async Task<BikeSharingRegressor> Build(IDataView dataView) => new BikeSharingRegressor(await new BikeSharingDemand.Model.TransformerFactory(MLContext2.Context).Train(dataView, MLContext2.Context.Regression.Trainers.OnlineGradientDescent()), dataView.Schema);

    }

}
