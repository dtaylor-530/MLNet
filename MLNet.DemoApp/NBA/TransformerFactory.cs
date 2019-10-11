using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using MLNet.RegressionChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionChart.Sample
{



    public class TransformerFactory : TransformerFactory<Microsoft.ML.Data.RegressionPredictionTransformer<LinearRegressionModelParameters>>
    {

        public TransformerFactory(MLContext mLContext) : base(mLContext)
        {
        }
        //mLContext.Regression.Trainers.OnlineGradientDescent(


        
    protected override EstimatorChain<RegressionPredictionTransformer<LinearRegressionModelParameters>> GetPipeLine(ITrainerEstimator<RegressionPredictionTransformer<LinearRegressionModelParameters>, LinearRegressionModelParameters> linearTrainer) 
            => mLContext.Transforms.ReplaceMissingValues("Age", "Age", MissingValueReplacingEstimator.ReplacementMode.Mean)
                    .Append(mLContext.Transforms.ReplaceMissingValues("Ws", "Ws", MissingValueReplacingEstimator.ReplacementMode.Mean))
                    .Append(mLContext.Transforms.ReplaceMissingValues("Bmp", "Bmp", MissingValueReplacingEstimator.ReplacementMode.Mean))
                    .Append(mLContext.Transforms.ReplaceMissingValues("NBA_DraftNumber", "NBA_DraftNumber", MissingValueReplacingEstimator.ReplacementMode.Mean))
                    .Append(mLContext.Transforms.NormalizeBinning("NBA_DraftNumber", "NBA_DraftNumber"))
                    .Append(mLContext.Transforms.NormalizeMinMax("Age", "Age"))
                    .Append(mLContext.Transforms.NormalizeMeanVariance("Ws", "Ws"))
                    .Append(mLContext.Transforms.NormalizeMeanVariance("Bmp", "Bmp"))
                    .Append(mLContext.Transforms.Concatenate(
                        "Features",
                        new[] { "NBA_DraftNumber", "Age", "Ws", "Bmp" }))
                     // .Append(mLContext.Regression.Trainers.FastTree()); // PlatformNotSupportedException
                     // .Append(mLContext.Regression.Trainers.OnlineGradientDescent(new OnlineGradientDescentTrainer.Options { })); // InvalidOperationException if you don't normalize.
                     // .Append(mLContext.Regression.Trainers.StochasticDualCoordinateAscent());       
                     .Append(linearTrainer);
    }
    
}
