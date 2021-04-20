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
                InputDataEntity.CrossProbability = Convert.ToInt32(CrossProbabilityTextBox.Text);
                InputDataEntity.MutationProbability = Convert.ToInt32(MutationProbabilityTextBox.Text);
                InputDataEntity.InversionProbability = Convert.ToInt32(InversionProbabilityTextBox.Text);
                InputDataEntity.MutationMethod = mutationMethodMapper.MapToMutationMethodEnum(MutationComboBox.SelectedIndex);
                InputDataEntity.EpochsAmount = Convert.ToInt32(EpochsAmountTextBox.Text);
                if (InputDataEntity.SelectionMethod == SelectionMethod.Best)
                {
                    InputDataEntity.BestSelectionPercentage = Convert.ToInt32(BestSelectionPercentageTextBox.Text);
                }

                OptimizationResult optimizationResult = functionOptimizerService.Optimize(InputDataEntity);
                MinimumTextBlock.Text += optimizationResult.ExtremeValue.ToString();
                X1TextBlock.Text += optimizationResult.X1.ToString();
                X2TextBlock.Text += optimizationResult.X2.ToString();
            } 
            catch(FormatException)
            {
                await ShowErrorMessage("Incorrect input", "Incorrect input value.");
            }
            System.Diagnostics.Debug.WriteLine(InputDataEntity.ToString());
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
    }
}
