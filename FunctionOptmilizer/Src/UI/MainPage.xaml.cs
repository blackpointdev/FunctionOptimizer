using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using FunctionOptimizer.Model;
using Windows.UI.Popups;
using FunctionOptimizer.Core;
using FunctionOptimizer.Selection.Mapper;
using FunctionOptimizer.Selection;
using FunctionOptimizer.Operations.Mutation.Mapper;
using FunctionOptimizer.Operations.Crossing.Mapper;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace FunctionOptimizer
{
    public sealed partial class MainPage : Page
    {
        private InputDataEntity InputDataEntity;
        private readonly FunctionOptimizerService functionOptimizerService;
        private readonly SelectionMethodMapper selectionMethodMapper;
        private readonly CrossMethodMapper crossMethodMapper;
        private readonly MutationMethodMapper mutationMethodMapper;
        public MainPage()
        {
            InitializeComponent();
            InputDataEntity = new InputDataEntity();
            functionOptimizerService = new FunctionOptimizerService();
            selectionMethodMapper = new SelectionMethodMapper();
            crossMethodMapper = new CrossMethodMapper();
            mutationMethodMapper = new MutationMethodMapper();
        }

        public async void OptimizeButtonClick(Object sender, RoutedEventArgs e)
        {
            try
            {
                InputDataEntity.DataRange = new Range(Convert.ToInt32(StartRangeTextBox.Text), Convert.ToInt32(EndRangeTextBox.Text));
                InputDataEntity.Accuracy = Convert.ToUInt16(AccuracyTextBox.Text);
                InputDataEntity.PopulationAmount = Convert.ToInt32(PopulationAmountTextBox.Text);
                InputDataEntity.SelectionMethod = selectionMethodMapper.MapToSelectionMethodEnum(SelectionComboBox.SelectedIndex);
                InputDataEntity.CrossMethod = crossMethodMapper.MapToCrossMethodEnum(CrossComboBox.SelectedIndex);
                InputDataEntity.EliteStrategyAmount = Convert.ToInt32(EliteStrategyTextBox.Text);
                InputDataEntity.CrossProbability = Convert.ToInt32(CrossProbabilityTextBox.Text);
                InputDataEntity.MutationProbability = Convert.ToInt32(MutationProbabilityTextBox.Text);
                InputDataEntity.InversionProbability = Convert.ToInt32(InversionProbabilityTextBox.Text);
                InputDataEntity.MutationMethod = mutationMethodMapper.MapToMutationMethodEnum(MutationComboBox.SelectedIndex);
                InputDataEntity.EpochsAmount = Convert.ToInt32(EpochsAmountTextBox.Text);
                InputDataEntity.TournamentAmount = Convert.ToInt32(TournamentAmountTextBox.Text);
                if (InputDataEntity.SelectionMethod == SelectionMethod.Best)
                {
                    InputDataEntity.BestSelectionPercentage = Convert.ToInt32(BestSelectionPercentageTextBox.Text);
                }
                
                OptimizationResult optimizationResult = functionOptimizerService.Optimize(InputDataEntity);

                FunctionValuesPlotView.Model = GetEpochsFunctionValue(optimizationResult);
                MinimumValueTextBlock.Text = optimizationResult.ExtremeValue.ToString();
                X1ValueTextBlock.Text = optimizationResult.X1.ToString();
                X2ValueTextBlock.Text = optimizationResult.X2.ToString();
            } 
            catch(FormatException)
            {
                await ShowErrorMessage("Incorrect input", "Incorrect input value.");
            }
        }

        private PlotModel GetEpochsFunctionValue(OptimizationResult optimizationResult)
        {
            var plotModel = new PlotModel();
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = InputDataEntity.EpochsAmount
            });
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = -30,
                Maximum = 0
            });
            var series = new LineSeries
            {
                MarkerType = MarkerType.None
                //Color = OxyColors.White
            };
            plotModel.Title = "Function value for each epoch";
            //plotModel.Background = OxyColors.Black;
            var dataPoint = GetDataPointsFromOptimizationResult(optimizationResult);
            series.Points.AddRange(dataPoint);
            series.InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline;
            plotModel.Series.Add(series);
            return plotModel;
        }

        private List<DataPoint> GetDataPointsFromOptimizationResult(OptimizationResult optimizationResults)
        {
            var dataPoints = new List<DataPoint>();
            var bestOfEachEpoch = optimizationResults.BestFromPreviousEpochs;
            for (int i = 0; i < bestOfEachEpoch.Count; i++) 
                dataPoints.Add(new DataPoint(i, bestOfEachEpoch[i].FunctionValue));
            return dataPoints;
        }

        private IAsyncOperation<ContentDialogResult> ShowErrorMessage(string title, string message)
        {
            var contentDialog = new ContentDialog()
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK"
            };
            return contentDialog.ShowAsync();
        }

        private void MockButtonClick(object sender, RoutedEventArgs e)
        {
            StartRangeTextBox.Text = "-10";
            EndRangeTextBox.Text = "10";

            AccuracyTextBox.Text = "8";
            EpochsAmountTextBox.Text = "1000";
            PopulationAmountTextBox.Text = "1000";

            BestSelectionPercentageTextBox.Text = "80";
            //TODO here
            CrossProbabilityTextBox.Text = "60";
            MutationProbabilityTextBox.Text = "40";
            InversionProbabilityTextBox.Text = "10";
            EliteStrategyTextBox.Text = "1";

            SelectionComboBox.SelectedIndex = 0;
            MutationComboBox.SelectedIndex = 0;
            CrossComboBox.SelectedIndex = 0;
        }
    }
}
