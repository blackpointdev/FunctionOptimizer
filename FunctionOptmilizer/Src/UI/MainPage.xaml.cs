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
using FunctionOptimizer.Utility;

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
                InputDataEntity.Maximize = MaximizeCheckBox.IsChecked.GetValueOrDefault(false);
                InputDataEntity.MutationMethod = mutationMethodMapper.MapToMutationMethodEnum(MutationComboBox.SelectedIndex);
                InputDataEntity.EpochsAmount = Convert.ToInt32(EpochsAmountTextBox.Text);
                if (InputDataEntity.SelectionMethod == SelectionMethod.Best || InputDataEntity.SelectionMethod == SelectionMethod.Roulette)
                {
                    InputDataEntity.BestSelectionPercentage = Convert.ToInt32(BestSelectionPercentageTextBox.Text);
                } 
                else if (InputDataEntity.SelectionMethod == SelectionMethod.Tournament)
                {
                    InputDataEntity.TournamentAmount = Convert.ToInt32(TournamentAmountTextBox.Text);
                }

                OptimizationResult optimizationResult = functionOptimizerService.Optimize(InputDataEntity);

                FunctionValuesPlotView.Model = GetEpochsFunctionValue(optimizationResult);
                MeanPlotView.Model = GetMeanPlotModel(optimizationResult);
                StandardDeviationPlotView.Model = GetStandardDeviationPlotModel(optimizationResult);

                MinimumValueTextBlock.Text = optimizationResult.ExtremeValue.ToString();
                X1ValueTextBlock.Text = optimizationResult.X1.ToString();
                X2ValueTextBlock.Text = optimizationResult.X2.ToString();
                TimeElapsedTextBlock.Text = (optimizationResult.TimeElapsed / 1000.0).ToString() + "s";
                if (SaveToFileCheckBox.IsChecked.GetValueOrDefault(true))
                    FileUtils.SaveResultsToFile(optimizationResult.BestFromPreviousEpochs);
            } 
            catch(FormatException)
            {
                await ShowErrorMessage("Incorrect input", "Incorrect input value.");
            }
        }

        private PlotModel GetMeanPlotModel(OptimizationResult optimizationResult)
        {
            var plotModel = GetDefaultPlotModel();
            var series = new LineSeries
            {
                MarkerType = MarkerType.None
            };
            plotModel.Title = "Mean function value for each epoch";
            var dataPoint = GetDataPointForMeanPlot(optimizationResult);
            series.Points.AddRange(dataPoint);
            series.InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline;
            plotModel.Series.Add(series);
            return plotModel;
        }

        private PlotModel GetStandardDeviationPlotModel(OptimizationResult optimizationResult)
        {
            var plotModel = GetDefaultPlotModel();
            var series = new LineSeries
            {
                MarkerType = MarkerType.None
            };
            plotModel.Title = "Standard deviation value for each epoch";
            var dataPoint = GetDataPointsForStandardDeviationPlot(optimizationResult);
            series.Points.AddRange(dataPoint);
            series.InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline;
            plotModel.Series.Add(series);
            return plotModel;
        }

        private PlotModel GetEpochsFunctionValue(OptimizationResult optimizationResult)
        {
            var plotModel = GetDefaultPlotModel();
            var series = new LineSeries
            {
                MarkerType = MarkerType.None
            };
            plotModel.Title = "Function value for each epoch";
            var dataPoint = GetDataPointsFromOptimizationResult(optimizationResult);
            series.Points.AddRange(dataPoint);
            series.InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline;
            plotModel.Series.Add(series);
            return plotModel;
        }

        private PlotModel GetDefaultPlotModel()
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
                Position = AxisPosition.Left
            });

            return plotModel;
        }

        private List<DataPoint> GetDataPointForMeanPlot(OptimizationResult optimizationResult)
        {
            var dataPoints = new List<DataPoint>();
            var means = optimizationResult.MeanFromPreviousEpochs;
            for (int i = 0; i < means.Count; i++)
                dataPoints.Add(new DataPoint(i, means[i]));
            return dataPoints;
        }

        private List<DataPoint> GetDataPointsFromOptimizationResult(OptimizationResult optimizationResults)
        {
            var dataPoints = new List<DataPoint>();
            var bestOfEachEpoch = optimizationResults.BestFromPreviousEpochs;
            for (int i = 0; i < bestOfEachEpoch.Count; i++) 
                dataPoints.Add(new DataPoint(i, bestOfEachEpoch[i].FunctionValue));
            return dataPoints;
        }

        private List<DataPoint> GetDataPointsForStandardDeviationPlot(OptimizationResult optimizationResult)
        {
            var dataPoints = new List<DataPoint>();
            var standardDeviations = optimizationResult.StandardDeviation;
            for (int i = 0; i < standardDeviations.Count; i++)
                dataPoints.Add(new DataPoint(i, standardDeviations[i]));
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
            EpochsAmountTextBox.Text = "100";
            PopulationAmountTextBox.Text = "100";

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
