using MLNet.Contract;
using MLNet.RegressionChart;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLNet.DemoApp
{
    public class Path : IPath
    {
        public string Find(string id) => System.IO.Path.Combine(DataGen.Singleton.Singleton<ApplicationData>.Instance.Data.FullName, id);
        //string modelPath() => Path.Combine(DataGen.Singleton.Singleton<ApplicationData>.Instance.Data.FullName, id);
    }

    public class ApplicationData
    {
        public FileInfo Data { get; } = new FileInfo("../../Data");

    }
}
