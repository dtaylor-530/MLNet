
//using Microsoft.ML;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLNet.RegressionChart
{

    
    public interface IPredict<InputRow, OutputRow>
    {

      //  IEnumerable<OutputRow> PredictMany(IDataView data);


        OutputRow Predict(InputRow data);


    }

    public interface IPredictMany<InputRow, OutputRow>
    {
    
        IEnumerable<OutputRow> PredictMany(IEnumerable<InputRow> data);

        // OutputRow Predict(InputRow data);
    }

    public interface IPredictMany
    {
        IEnumerable PredictMany(Microsoft.ML.IDataView data);

    }


    //public interface IPredict
    //{
    //    IEnumerable PredictMany(IDataView data);


    //    object Predict(object data);


    //}
}
