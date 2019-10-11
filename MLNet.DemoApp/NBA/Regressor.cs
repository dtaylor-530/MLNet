
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using MLNet.Contract;
using MLNet.RegressionChart;

namespace RegressionChart.Sample
{


    public class MLContext2
    {
        public static MLContext Context { get; } = new MLContext();
    }



    public class NBARegressor : Regressor<InputRow, OutputRow, TransformerChain<RegressionPredictionTransformer<LinearRegressionModelParameters>>>
    {
        public NBARegressor(TransformerChain<RegressionPredictionTransformer<LinearRegressionModelParameters>> transformer, DataViewSchema schema) :
            base(
                new MLNet.DemoApp.Path(),
                id: "id",
                mLContext: MLContext2.Context,
                transformer: transformer,
                schema: schema)
        {
        }
    }


    public class NBARegressorFactory
    {
        public static async Task<NBARegressor> Build(IDataView dataView) =>
                                                                        new NBARegressor(await new TransformerFactory(MLContext2.Context)
                                                                            .Train(
                                                                            dataView,
                                                                            MLContext2.Context.Regression.Trainers.OnlineGradientDescent()),
                                                                            dataView.Schema);
    }
}
