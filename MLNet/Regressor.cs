using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using MLNet.Contract;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using Windows.Storage;

namespace MLNet.RegressionChart
{

    public class Regressor <InputRow,OutputRow,TLastTransformer>:ISaveLoad, IPredictMany<InputRow, OutputRow>, IPredict<InputRow, OutputRow>, IPredictMany
        where InputRow:class
        where OutputRow : class, new()
        where TLastTransformer:class,ITransformer
    {
        private MLContext mLContext;
        private PredictionEngine<InputRow, OutputRow> predictionEngine;
        private DataViewSchema schema;
        private ITransformer transformer;
        private readonly IPath path;
        private string id;
      

        public Regressor(IPath path, string id,MLContext mLContext,ITransformer transformer,DataViewSchema schema):this(path,id, mLContext,transformer)
        {
            this.schema= schema;
  
        }


        public Regressor(IPath path, string id, MLContext mLContext, ITransformer transformer)
        {
            this.path = path;
            this.id = id;
            this.mLContext = mLContext;
            this.transformer = transformer;
        }

        public void Save()
        {
            if (schema != null)
            {
                mLContext.Model.Save(transformer, schema, path.Find(id));
            }
        }

        public void Load()
        {
            if (schema != null)
            {
                transformer = mLContext.Model.Load(path.Find(id), out schema);
            }
        }


        public IEnumerable PredictMany(IDataView data)
        {
            return mLContext.Data.CreateEnumerable<OutputRow>(transformer.Transform(data), reuseRowObject: false);
        }

        public IEnumerable<OutputRow> PredictMany(IEnumerable<InputRow> data)
        {
            predictionEngine = predictionEngine ?? mLContext.Model.CreatePredictionEngine<InputRow, OutputRow>(transformer);
            return data.Select(d=>predictionEngine.Predict(d));
        }

        public OutputRow Predict(InputRow data)
        {
            predictionEngine = predictionEngine ?? mLContext.Model.CreatePredictionEngine<InputRow, OutputRow>(transformer);
            return predictionEngine.Predict(data);
        }
    }
}
