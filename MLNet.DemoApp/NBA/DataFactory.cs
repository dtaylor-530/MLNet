using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegressionChart.Sample
{



    public class NBADataFactory
    {
        private MLContext mLContext;

        public NBADataFactory(MLContext mLContext)
        {
            this.mLContext = mLContext;
        }

        public NBADataFactory()
        {
            this.mLContext = new MLContext();
        }

        public IDataView Build()
        {
            string trainingDataPath = "../../Data/2017-18_NBA_salary.csv";

            var readerOptions = new TextLoader.Options()
            {
                Separators = new[] { ',' },
                HasHeader = true,
                Columns = new[]
                    {
                            new TextLoader.Column("Label", DataKind.Single, 1),
                            new TextLoader.Column("NBA_DraftNumber", DataKind.Single, 3),
                            new TextLoader.Column("Age", DataKind.Single, 4),
                            new TextLoader.Column("Ws", DataKind.Single, 22),
                            new TextLoader.Column("Bmp", DataKind.Single, 26)
                        }
            };

            return  mLContext.Data.LoadFromTextFile(trainingDataPath, readerOptions);
        }

    }


}
