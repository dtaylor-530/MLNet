//using Microsoft.ML;
//using Microsoft.ML.Data;
//using Microsoft.ML.Transforms;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MLNet.RegressionChart
//{
//    public class DataFactory
//    {
//        private MLContext mLContext;

//        public DataFactory(MLContext mLContext)
//        {
//            this.mLContext = mLContext;
//        }

//        public IDataView Build()
//        {
//            string trainingDataPath = DataGen.Singleton.Singleton<ApplicationData>.Instance.Data.FullName;
//            var readerOptions = new TextLoader.Options()
//            {
//                Separators = new[] { ',' },
//                HasHeader = true,
//                Columns = new[]
//                    {
//                            new TextLoader.Column("Label", DataKind.Single, 1),
//                            new TextLoader.Column("NBA_DraftNumber", DataKind.Single, 3),
//                            new TextLoader.Column("Age", DataKind.Single, 4),
//                            new TextLoader.Column("Ws", DataKind.Single, 22),
//                            new TextLoader.Column("Bmp", DataKind.Single, 26)
//                        }
//            };

//            var pipeline = mLContext.Transforms.ReplaceMissingValues("Age", "Age", MissingValueReplacingEstimator.ReplacementMode.Mean)
//              .Append(mLContext.Transforms.ReplaceMissingValues("Ws", "Ws", MissingValueReplacingEstimator.ReplacementMode.Mean))
//              .Append(mLContext.Transforms.ReplaceMissingValues("Bmp", "Bmp", MissingValueReplacingEstimator.ReplacementMode.Mean))
//              .Append(mLContext.Transforms.ReplaceMissingValues("NBA_DraftNumber", "NBA_DraftNumber", MissingValueReplacingEstimator.ReplacementMode.Mean))
//              .Append(mLContext.Transforms.NormalizeBinning("NBA_DraftNumber", "NBA_DraftNumber"))
//              .Append(mLContext.Transforms.NormalizeMinMax("Age", "Age"))
//              .Append(mLContext.Transforms.NormalizeMeanVariance("Ws", "Ws"))
//              .Append(mLContext.Transforms.NormalizeMeanVariance("Bmp", "Bmp"))
//              .Append(mLContext.Transforms.Concatenate(
//                  "Features",
//                  new[] { "NBA_DraftNumber", "Age", "Ws", "Bmp" }))
//               // .Append(_MLContext.Regression.Trainers.FastTree()); // PlatformNotSupportedException
//               // .Append(_MLContext.Regression.Trainers.OnlineGradientDescent(new OnlineGradientDescentTrainer.Options { })); // InvalidOperationException if you don't normalize.
//               // .Append(_MLContext.Regression.Trainers.StochasticDualCoordinateAscent());       
//               .Append(mLContext.Regression.Trainers.OnlineGradientDescent());
//            //.Append(_MLContext.Regression.Trainers.FastForest());


//            return mLContext.Data.LoadFromTextFile(trainingDataPath, readerOptions);


//            //return mLContext.Data.CreateEnumerable<Data>(trainingData, reuseRowObject: false);

//        }
//    }
//}
