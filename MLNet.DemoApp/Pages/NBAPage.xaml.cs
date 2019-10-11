using System;

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MLNet.RegressionChart;
using Microsoft.ML;
using RegressionChart.Sample;
//using XamlBrewer.Uwp.MachineLearningSample.Models;
//using XamlBrewer.Uwp.MachineLearningSample.ViewModels;



namespace MLNet.DemoApp.Pages
{
    public sealed partial class NBAPage : Page
    {
        public NBAPage()
        {
            this.InitializeComponent();
            //this.DataContext = new RegressionPageViewModel();


            Initialise();
        }

        private async void Initialise()
        {
            IDataView dataView =await System.Threading.Tasks.Task.Run(()=>new NBADataFactory().Build());
            RegressionControl.TestDataView = dataView;
            RegressionControl.Regressor = await NBARegressorFactory.Build(dataView);
        }

    }
}
