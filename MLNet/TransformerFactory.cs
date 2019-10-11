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

namespace MLNet.RegressionChart
{

    public abstract class TransformerFactory<TLastTransformer> where TLastTransformer : class, ITransformer
    {
        protected MLContext mLContext;

        public TransformerFactory(MLContext mLContext)
        {
            this.mLContext = mLContext;
        }
        public TransformerFactory()
        {
        }

        public async Task<Microsoft.ML.Data.TransformerChain<TLastTransformer>> Train(IDataView dataView, ITrainerEstimator<RegressionPredictionTransformer<LinearRegressionModelParameters>, LinearRegressionModelParameters> linearTrainer) => 
            await System.Threading.Tasks.Task.Run(() => GetPipeLine(linearTrainer).Fit(dataView));

       
        protected abstract Microsoft.ML.Data.EstimatorChain<TLastTransformer> GetPipeLine(ITrainerEstimator<RegressionPredictionTransformer<LinearRegressionModelParameters>, LinearRegressionModelParameters> linearTrainer);
    }
}
