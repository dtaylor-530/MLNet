using System;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MLNet.RegressionChart;
using Microsoft.ML;
using MLNet.DemoApp.BikeSharing;
//using XamlBrewer.Uwp.MachineLearningSample.Models;
//using XamlBrewer.Uwp.MachineLearningSample.ViewModels;



namespace MLNet.DemoApp.Pages
{
    public sealed partial class BikeSharingPage : Page
    {
        public BikeSharingPage()
        {
            this.InitializeComponent();
            //this.DataContext = new RegressionPageViewModel();


            Initialise();
        }

        private async void Initialise()
        {
            IDataView trainDataView =await System.Threading.Tasks.Task.Run(() => new MLNet.DemoApp.BikeSharing.DataFactory().Build());
            IDataView testdataView = await System.Threading.Tasks.Task.Run(() => new MLNet.DemoApp.BikeSharing.DataFactory().BuildTest());

          

            RegressionControl.TestDataView = testdataView;
            var regressor = await BikeSharingRegressorFactory.Build(trainDataView);
            
            RegressionControl.Regressor = regressor;
        }

     
    }
}
