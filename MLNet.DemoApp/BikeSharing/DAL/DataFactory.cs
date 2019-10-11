using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLNet.DemoApp.BikeSharing
{



    public class DataFactory
    {
        private MLContext mLContext;

        public DataFactory(MLContext mLContext)
        {
            this.mLContext = mLContext;
        }

        public DataFactory()
        {
            this.mLContext = new MLContext();
        }

        public IDataView Build() => Build("../../Data/hour_test.csv");

        public IDataView BuildTest()=>Build( "../../Data/hour_train.csv");
        public IDataView Build(string path)
        {
            var readerOptions = new TextLoader.Options()
            {
                Separators = new[] { ',' },
                HasHeader = true,
                Columns = new[]
                    {
                            new TextLoader.Column("Season", DataKind.Single, 2),
                            new TextLoader.Column("Year", DataKind.Single, 3),
                            new TextLoader.Column("Month", DataKind.Single, 4),
                    new TextLoader.Column("Hour", DataKind.Single, 5),
                            new TextLoader.Column("Holiday", DataKind.Single, 6),
                            new TextLoader.Column("Weekday", DataKind.Single, 7),
                    new TextLoader.Column("Weather", DataKind.Single, 8),
                            new TextLoader.Column("Temperature", DataKind.Single, 9),
                            new TextLoader.Column("NormalizedTemperature", DataKind.Single, 10),
                    new TextLoader.Column("Humidity", DataKind.Single, 11),
                            new TextLoader.Column("Windspeed", DataKind.Single, 12),
                    new TextLoader.Column("Count", DataKind.Single, 15),
                        }
            };

            return mLContext.Data.LoadFromTextFile(path, readerOptions);
        }
    }
}

//}
//public IEnumerable<BikeSharingDemandSample> GetDataFromCsv(string dataLocation)
//{
//    return File.ReadAllLines(dataLocation)
//        .Skip(1)
//        .Select(x => x.Split(','))
//        .Select(x => new BikeSharingDemandSample()
//        {
//            Season = float.Parse(x[2]),
//            Year = float.Parse(x[3]),
//            Month = float.Parse(x[4]),
//            Hour = float.Parse(x[5]),
//            Holiday = int.Parse(x[6]) != 0,
//            Weekday = float.Parse(x[7]),
//            Weather = float.Parse(x[8]),
//            Temperature = float.Parse(x[9]),
//            NormalizedTemperature = float.Parse(x[10]),
//            Humidity = float.Parse(x[11]),
//            Windspeed = float.Parse(x[12]),
//            Count = float.Parse(x[15])
//        });
//}