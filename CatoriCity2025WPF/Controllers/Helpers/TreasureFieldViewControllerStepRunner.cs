using CatoriCity2025WPF.Objects.Arguments;
using CatoriCity2025WPF.Views.Controls.Treasure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Animation;
using CatoriCity2025WPF.Objects;
namespace CatoriCity2025WPF.Controllers.Helpers
{
    internal class TreasureFieldViewControllerStepRunner
    {
        PersonControl person;
        TreasureFieldViewController _controller;
        double viewMainheight => _controller._view.MainLayoutField.ActualHeight;
        double viewMainWidth => _controller._view.MainLayoutField.ActualWidth;
        internal TreasureFieldViewControllerStepRunner(
            PersonControl personControl, TreasureFieldViewController controller )
        {
            this.person = personControl;
            this._controller = controller;
        }
        private bool _isRunning;
        internal async Task RunRecordedStepsAsync(LearnedStepRunArgument stepsModel)
        {
            if (_isRunning) return; // prevent multiple simultaneous runs
            _isRunning = true;
            int i = 0;
            var spots = GetAllTreasureSpotControls();
            cLogger.Log($"------  start {spots.Count} ------");

            foreach (var item in spots)
            {
                if (stepsModel.NumberOfSpotsToRun > 0 && i > stepsModel.NumberOfSpotsToRun)
                    break;

                cLogger.Log($"------  Loop i: {i} ------");

                _controller.ShowRunUpdate($" {i + 1} of {spots.Count}");
  
                await RunStepsAsync(item, stepsModel, spots.Count);

                // move person back to bottom AFTER steps complete
                double toploc = viewMainheight - person.ActualHeight - 50;
                double personLeft = Canvas.GetLeft(person);
                double persontop = Canvas.GetTop(person);

                await AnimateLeftTopAsync(100, toploc, personLeft, persontop, 2, RunAfterAnimation.None);

                i++;
            }

            _isRunning = false;
        }
        private async Task RunStepsAsync(
            TreasureSpotDetailModel model,LearnedStepRunArgument runArgs,
            int spotscount)
        {
            int i = 0;

            foreach (var step in runArgs.LearnedSteps)
            {
                var thisstep = step.TreasureStep;

                if (runArgs.RunType == "All")
                    step.IsChecked = true;

                if (!step.IsChecked)
                    continue;

                Point centeredPoint = UIUtility.GetCenteredCanvasPoint(person);

                double left = centeredPoint.X;
                double top = centeredPoint.Y;

                cLogger.Log($"==== Step {thisstep} i {i} left {left} top {top} ====");

                switch (thisstep)
                {
                    case TreasureStepEnum.WalkToTreasureSpot:
                        {
                            double spotLeft = Canvas.GetLeft(model.Treasurespot);
                            double spotTop = Canvas.GetTop(model.Treasurespot) + 20;

                            cLogger.Log($"WalkToTreasureSpot -> {spotLeft},{spotTop}");

                            await AnimateLeftTopAsync(spotLeft, spotTop, left, top, 2, RunAfterAnimation.Digging);
                            break;
                        }

                    case TreasureStepEnum.WalkToWorkbench:
                        {
                            Point centeredPointWorkbench =
                                UIUtility.GetCenteredCanvasPoint(_controller._view.workbench);

                            double workbenchleft = centeredPointWorkbench.X;
                            double workbenchtop = centeredPointWorkbench.Y;

                            cLogger.Log($"WalkToWorkbench -> {workbenchleft},{workbenchtop}");

                            await AnimateLeftTopAsync(workbenchleft, workbenchtop, left, top, 2,RunAfterAnimation.OpenChest);
                            break;
                        }

                    case TreasureStepEnum.Bank:
                        {
                            cLogger.Log("Bank deposit");
                            AddTreasureAmountToPersonFunds(model.TreasureValue);
                            break;
                        }
                }

                i++;
            }
        }
        internal List<TreasureSpotDetailModel> GetAllTreasureSpotControls()
        {
            List<TreasureSpotDetailModel> treasureSpotControls;
            try
            {
                treasureSpotControls = new List<TreasureSpotDetailModel>();

                foreach (UIElement child in _controller._view.MainLayoutField.Children)
                {

                    if (child is TreasureSpotControl treasureSpot)
                    {
                        TreasureSpotDetailModel model = new TreasureSpotDetailModel();
                        model.Treasurespot = (TreasureSpotControl)child;
                        treasureSpotControls.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                cLogger.Log("ex " + ex.Message);
                throw;
            }

            return treasureSpotControls;
        }

        private void StartDigging()
        {
            person.StartDiggingAsync();
        }
        private async Task OpenChestAsync()
        {
            _controller._view.workbench.OpenChest();
        }
        private Task AnimateLeftTopAsync(
            double targetLeft,
            double targetTop,
            double startLeft,
            double startTop,
            int speedseconds, RunAfterAnimation afterAnimation)
        {
            if (speedseconds == 0)
                speedseconds = 1;   
            var tcs = new TaskCompletionSource();

            Storyboard storyboard = new Storyboard();

            DoubleAnimation leftAnimation = new DoubleAnimation
            {
                From = startLeft,
                To = targetLeft,
                Duration = TimeSpan.FromSeconds(speedseconds)
            };

            DoubleAnimation topAnimation = new DoubleAnimation
            {
                From = startTop,
                To = targetTop,
                Duration = TimeSpan.FromSeconds(speedseconds)
            };

            Storyboard.SetTarget(leftAnimation, person);
            Storyboard.SetTarget(topAnimation, person);

            Storyboard.SetTargetProperty(leftAnimation, new PropertyPath("(Canvas.Left)"));
            Storyboard.SetTargetProperty(topAnimation, new PropertyPath("(Canvas.Top)"));

            storyboard.Children.Add(leftAnimation);
            storyboard.Children.Add(topAnimation);

            storyboard.Completed += async (s, e) =>
            {
                switch (afterAnimation)
                {
                    case RunAfterAnimation.None:
                        break;
                    case RunAfterAnimation.Digging:
                        await person.StartDiggingAsync();
                        break;
                    case RunAfterAnimation.OpenChest:
                        await OpenChestAsync();
                        break;
                    default:
                        break;
                }
                tcs.SetResult();
            };

            storyboard.Begin();

            return tcs.Task;
        }
        private void AddTreasureAmountToPersonFunds(decimal funds)
        {
            GlobalAllApps.CurrentPerson.Funds += funds;
        }
    }

    public enum RunAfterAnimation
    {
        None,
        Digging,
        OpenChest
    }
}
