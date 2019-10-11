using Microsoft.ML;
using Microsoft.ML.Data;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UtilityHelper;
using UtilityWpf.View;
using MoreLinq;
using System.Threading;

namespace MLNet.RegressionChart
{
    public class SliderItem
    {
        public string Key { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
        public double Value { get; set; }
    }

    public class RegressionControl : Control
    {
        DataGrid DataGrid;
        Button buttonSave;
        Button buttonLoad;
        private SliderItemsControl sliderItemsControl;
        private OxyPlot.Wpf.PlotView Diagram;
        private ISubject<IDataView> testDataViewChanges = new Subject<IDataView>();
        private ISubject<IDataView> trainDataViewChanges = new Subject<IDataView>();
        private ISubject<object> regressorChanges = new Subject<object>();
        private DataTable dataTable;
        private PlotModel plotModel;
        private ScatterSeries predictedSeries;
        private ScatterSeries singlePredictionSeries;
        private ScatterSeries realSeries;
        private IScore[] result;
        private DataDebuggerPreview testDataPreview;
        private int index;



        static RegressionControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RegressionControl), new FrameworkPropertyMetadata(typeof(RegressionControl)));
        }

        public RegressionControl()
        {
            if (TestDataView != null)
            {
                testDataViewChanges.OnNext(TestDataView as IDataView);
            }

            testDataViewChanges.Subscribe(async dataView =>
            {
                var xc = dataView.Preview().RowView.Select(_ => _.Values.ToDictionary(aa => aa.Key, aa => (aa.Value))).ToArray();
                dataTable = (xc as IList<Dictionary<string, object>>).ToDataTableAsDictionary();
                if (DataGrid != null)
                    DataGrid.ItemsSource = dataTable.DefaultView;

                await Init(dataView);
            });
        }


        private async Task Init(IDataView dataView)
        {
            await await Task.Run(() =>
          (from _ in dataView.Preview().ColumnView
           let Min = System.Convert.ToDouble(_.Values.Min())
           let Max = System.Convert.ToDouble(_.Values.Max())
           select new SliderItem { Key = _.Column.Name, Min = Min, Max = Max, Value = (Min + Max) / 2 }).ToArray())
           .ContinueWith(async a =>

           await this.Dispatcher.InvokeAsync(async () =>

               sliderItemsControl.Data = await a
            ), TaskScheduler.FromCurrentSynchronizationContext());
        }





        public override void OnApplyTemplate()
        {
            DataGrid = this.GetTemplateChild("DataGrid") as DataGrid;

            DataGrid.ItemsSource = dataTable?.DefaultView;
            var buttonSaveToCsv = this.GetTemplateChild("Save_To_Csv") as Button;
            buttonSave = this.GetTemplateChild("Save") as Button;
            buttonLoad = this.GetTemplateChild("Load") as Button;
            sliderItemsControl = this.GetTemplateChild("SliderItemsControl") as SliderItemsControl;

            Diagram = this.GetTemplateChild("Diagram") as OxyPlot.Wpf.PlotView;


            Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => buttonSave.Click += h, h => buttonSave.Click -= h)
                                .WithLatestFrom<EventPattern<RoutedEventArgs>, object, Action>(regressorChanges, (a, b) => (b as ISaveLoad).Save)
                                .Subscribe(_ => _.Invoke());

            Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => buttonLoad.Click += h, h => buttonLoad.Click -= h)
                  .WithLatestFrom<EventPattern<RoutedEventArgs>, object, Action>(regressorChanges, (a, b) => (b as ISaveLoad).Load)
                  .Subscribe(_ => _.Invoke());



            if (buttonSaveToCsv != null)
            {
                Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => buttonSaveToCsv.Click += h, h => buttonSaveToCsv.Click -= h)
                    .WithLatestFrom<EventPattern<RoutedEventArgs>, object, Task<bool>>(trainDataViewChanges, (a, b) => SaveToCsv(dataTable))
                    .Subscribe(async _ => await _);
            }

            var xxx = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => sliderItemsControl.ValueChanged += h, h => sliderItemsControl.ValueChanged -= h)
                     .WithLatestFrom(regressorChanges, (a, b) => new Action(() => SliderItemsControl_ValueChanged(a.EventArgs, b)))
                     .Buffer(TimeSpan.FromMilliseconds(100))
                     .ObserveOnDispatcher()
                     .Subscribe(_ => _.LastOrDefault()?.Invoke());


            regressorChanges.CombineLatest(testDataViewChanges, AddPoints).Subscribe(async _ =>
              {
                  await _;
              });


            Diagram.Model = CreatePlotModel();
            Diagram.Model.InvalidatePlot(true);


            // sliderItemsControl.ValueChanged +=(a,s SliderItemsControl_ValueChanged;
        }


        private Task AddPoints(object regressor, IDataView testDataView)
        {
            return Task.Run(() =>
            {
                var prediction = ((IPredictMany)regressor).PredictMany(testDataView);
                result = prediction.Cast<IScore>().ToArray();
                testDataPreview = testDataView.Preview(100000);
            }).ContinueWith(async a => await UpdatePlot(CancellationToken.None), TaskScheduler.FromCurrentSynchronizationContext());
        }


        private async Task<bool> SaveToCsv(DataTable dataTable)
        {
            return await Task.Run(() => dataTable.SaveToCSV("../../Data/NBACleanedData.csv")).ContinueWith(a => !a.IsFaulted);

        }

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationTokenSource cancellationTokenSource2 = new CancellationTokenSource();
        private CancellationToken cancellationToken;
        private CancellationToken cancellationToken2;
        private async void SliderItemsControl_ValueChanged(RoutedEventArgs args, object e)
        {

            (int columnIndex, double value, int rowIndex) columnValueIndex = await Task.Run(() =>
                                                                                {
                                                                                    var kvp = (args as SliderItemsControl.KeyValuePairRoutedEventArgs).KeyValuePair;
                                                                                    double value = kvp.Value;
                                                                                    int rowIndex = testDataPreview.RowView
                                                                                                        .Select((_, i) => new { val = _.Values[index].Value, i })
                                                                                                        .OrderBy(_ => _.val)
                                                                                                        .Where((_) => Convert.ToDouble(_.val) > value)
                                                                                                        .FirstOrDefault()?.i ?? 0;
                                                                                    return (testDataPreview.Schema.Single(_ => _.Name == kvp.Key).Index, value, rowIndex);
                                                                                });
            cancellationTokenSource2.Cancel();
            cancellationTokenSource2 = new CancellationTokenSource();
            cancellationToken2 = cancellationTokenSource.Token;
            await AddCrossHairAnnotation(columnValueIndex.value, result[columnValueIndex.rowIndex].Score, cancellationToken2);

            if (columnValueIndex.columnIndex != index)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource = new CancellationTokenSource();
                cancellationToken = cancellationTokenSource.Token;
                index = columnValueIndex.columnIndex;
                await UpdatePlot(cancellationToken);
            }

        }



        public object Regressor
        {
            get { return (object)GetValue(RegressorProperty); }
            set { SetValue(RegressorProperty, value); }
        }

        public static readonly DependencyProperty RegressorProperty =
            DependencyProperty.Register("Regressor", typeof(object), typeof(RegressionControl), new PropertyMetadata(null, regressorChanged));

        private static void regressorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RegressionControl).regressorChanges.OnNext(e.NewValue);
        }

        public object TestDataView
        {
            get { return (object)GetValue(TestDataViewProperty); }
            set { SetValue(TestDataViewProperty, value); }
        }


        public static readonly DependencyProperty TestDataViewProperty = DependencyProperty.Register("TestDataView", typeof(object), typeof(RegressionControl), new PropertyMetadata(null, TestDataViewChanged));

        private static void TestDataViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RegressionControl).testDataViewChanges.OnNext((IDataView)e.NewValue);
        }

        public object TrainDataView
        {
            get { return (object)GetValue(TrainDataViewProperty); }
            set { SetValue(TrainDataViewProperty, value); }
        }


        public static readonly DependencyProperty TrainDataViewProperty = DependencyProperty.Register("TrainDataView", typeof(object), typeof(RegressionControl), new PropertyMetadata(null, TrainDataViewChanged));

        private static void TrainDataViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RegressionControl).trainDataViewChanges.OnNext((IDataView)e.NewValue);
        }



        private async Task UpdatePlot(CancellationToken token)
        {
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            await Task.Run(async () =>
            {
                predictedSeries.Points.Clear();
                realSeries.Points.Clear();

                int length = (int)(result.Length / (20d * 10));

                int i = 0;
                foreach (var batch in result.Zip(testDataPreview.RowView, (a, b) => (a.Score, b.Values[0].Value, b.Values[index].Value)).Batch(20))
                {
                    i++;
                    await Task.Run(() =>
                    {
                        List<ScatterPoint> predicted = new List<ScatterPoint>();
                        List<ScatterPoint> real = new List<ScatterPoint>();
                        foreach ((float score, object value0, object value) in batch)
                        {
                            var doubleValue = Convert.ToDouble(value);
                            predicted.Add(new ScatterPoint(doubleValue, (double)score));
                            real.Add(new ScatterPoint(doubleValue, Convert.ToDouble(value0)));
                        }
                        return (predicted, real,i);
                    })
                    .ContinueWith(a => { Thread.Sleep(200); return a; })
                    .ContinueWith(async a =>
                    {
                        var b = await await a;
                        predictedSeries.Points.AddRange(b.Item1);
                        realSeries.Points.AddRange(b.Item2);
                        if (b.Item3 % length < 5 )
                            Diagram.Model.InvalidatePlot(true);
                    }, scheduler);
                   // if (i % length == 0)
                       
                }
            }, token).ContinueWith(b => Diagram.Model.InvalidatePlot(true), scheduler);
        }

        private async Task AddCrossHairAnnotation(double x, double y, CancellationToken cancellationToken)
        {
            await this.Dispatcher.InvokeAsync(() => Diagram.Model.Annotations.Clear(), System.Windows.Threading.DispatcherPriority.Background);
            await Task.Run(() =>
            {
                var vannotation = new OxyPlot.Annotations.LineAnnotation
                {
                    X = x,
                    Y = 0,
                    Type = LineAnnotationType.Vertical,
                    Color = OxyColors.Gainsboro,
                    LineStyle = LineStyle.Dash
                };


                var hannotation = new OxyPlot.Annotations.LineAnnotation
                {
                    X = 0,
                    Y = y,
                    Type = LineAnnotationType.Horizontal,
                    Color = OxyColors.Gainsboro,
                    LineStyle = LineStyle.Dash
                };


                return (vannotation, hannotation);
            }, cancellationToken).ContinueWith(async a =>
            {
                var result = await a;
                Diagram.Model.Annotations.Add(result.vannotation);
                Diagram.Model.Annotations.Add(result.hannotation);
                Diagram.Model.InvalidatePlot(true);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }





        public PlotModel CreatePlotModel()
        {

            var foreground = OxyColors.SteelBlue;
            plotModel = new PlotModel
            {
                PlotAreaBorderThickness = new OxyThickness(1, 0, 0, 1),
                PlotAreaBorderColor = foreground,
                TextColor = foreground,
                TitleColor = foreground,
                SubtitleColor = foreground,
                LegendPosition = LegendPosition.TopCenter,
                LegendOrientation = LegendOrientation.Horizontal
            };

            var axisX = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Source",
                TextColor = foreground,
                TicklineColor = foreground,
                TitleColor = foreground
            };

            plotModel.Axes.Add(axisX);

            var axisY = new LinearAxis
            {
                Title = "Target",
                TextColor = foreground,
                TicklineColor = foreground,
                TitleColor = foreground
            };
            plotModel.Axes.Add(axisY);

            // Just to put an entry in the Legend.
            singlePredictionSeries = new ScatterSeries
            {
                Title = "Single Prediction",
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerFill = OxyColors.Green
            };

            plotModel.Series.Add(singlePredictionSeries);

            realSeries = new ScatterSeries
            {
                Title = "Real",
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerFill = OxyColors.SteelBlue
            };

            plotModel.Series.Add(realSeries);

            predictedSeries = new ScatterSeries
            {
                Title = "Predicted",
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerFill = OxyColors.Firebrick
            };

            plotModel.Series.Add(predictedSeries);

            return plotModel;
        }

    }

}
