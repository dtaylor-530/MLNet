using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLNet.RegressionChart
{
    public class TransformerWrapper<TLastTransformer> where TLastTransformer : class, ITransformer
    {
        public DataViewSchema Schema;

        public Microsoft.ML.Data.TransformerChain<TLastTransformer> Transformer { get;  set; }


    }
}
