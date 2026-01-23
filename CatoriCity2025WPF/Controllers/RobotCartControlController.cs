using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Animation;

namespace CatoriCity2025WPF.Controllers
{
    internal class RobotCartControlController
    {
        public double _originalLeft = 350;
        public double _originalTop = 520;

        RobotCartControl _view;
        internal RobotCartControlController(RobotCartControl view)
        {
            _view = view;
        }

        internal void MoveCartFromTimer()
        {
            //show side view
            _view.SetSideView();
            int seconds = 2;
            Storyboard sb = new Storyboard();
            DoubleAnimation daleft = AnimationHelper.GetDoubleAnimation(_originalLeft, 500, seconds * 1000);
            daleft.EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            };
            Storyboard.SetTarget(daleft, _view);
            Storyboard.SetTargetProperty(daleft, new PropertyPath("(Canvas.Left)"));
            sb.Children.Add(daleft);

            DoubleAnimation dtop = AnimationHelper.GetDoubleAnimation(_originalTop, 550, seconds * 1000);
            Storyboard.SetTarget(dtop, _view);
            Storyboard.SetTargetProperty(dtop, new PropertyPath("(Canvas.Top)"));
            dtop.EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            };
            sb.Children.Add(dtop);

            DoubleAnimation daleft2 = AnimationHelper.GetDoubleAnimation(1000, 1220, seconds * 1000);
            Storyboard.SetTarget(daleft2, _view);
            Storyboard.SetTargetProperty(daleft2, new PropertyPath("(Canvas.Left)"));
            sb.Children.Add(daleft2);

            DoubleAnimation dtop2 = AnimationHelper.GetDoubleAnimation(550, 400, seconds * 1000);
            Storyboard.SetTarget(dtop2, _view);
            Storyboard.SetTargetProperty(dtop2, new PropertyPath("(Canvas.Top)"));
            sb.Children.Add(dtop2);

            sb.Completed += (s, e) =>
            {
                //show rear view
                Task.Delay(500).ContinueWith(t =>
                {
                    _view.Dispatcher.Invoke(() =>
                    {
                        //show front view
                        _view.SetFrontiew();
                    });
                });
            };
            sb.Begin();
        }


        internal void MoveCartToLoadOut()
        {
            _view.SetFrontiew();
            int seconds = 2;
            Storyboard sb = new Storyboard();
            DoubleAnimation daleft = AnimationHelper.GetDoubleAnimation(550, 650, seconds * 1000);
            daleft.EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            };
            Storyboard.SetTarget(daleft, _view);
            Storyboard.SetTargetProperty(daleft, new PropertyPath("(Canvas.Top)"));
            sb.Children.Add(daleft);


            sb.Completed += (s, e) =>
            {
                //show rear view
                //_view.SetreadView();
                Task.Delay(500).ContinueWith(t =>
                {
                    _view.Dispatcher.Invoke(() =>
                    {
                        //show front view

                        _view.SetSideView();
                        MoveLeft();

                    });
                });
            };
            sb.Begin();
        }
        private double stopbetweencabinets = 780;
        private void MoveLeft()
        {
            //show side view
            _view.SetSideView();
            int seconds = 2;
            Storyboard sb = new Storyboard();
            DoubleAnimation daleft = AnimationHelper.GetDoubleAnimation(1220, stopbetweencabinets, seconds * 1000);
            daleft.EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            };
            Storyboard.SetTarget(daleft, _view);
            Storyboard.SetTargetProperty(daleft, new PropertyPath("(Canvas.Left)"));
            sb.Children.Add(daleft);
            sb.Completed += (s, e) =>
            {
                //show rear view
                Task.Delay(500).ContinueWith(t =>
                {
                    _view.Dispatcher.Invoke(() =>
                    {
                        //show front view
                        _view.SetRearView();
                       MoveAtAngleBehindCounter();
                    });
                });
            };
            sb.Begin();
        }
        double stopatShelves = 400;
        double robotwidthorig = 350;
        double robotheightorig = 350;
        double robotwidthfinal = 200;
        double robotheightfinal = 200;

        private void MoveAtAngleBehindCounter()
        {
            //show side view
            _view.SetRearView();
            int seconds = 2;
            Storyboard sb = new Storyboard();
            DoubleAnimation daWidth = AnimationHelper.GetDoubleAnimation(robotwidthorig, robotwidthfinal, seconds * 1000);
            daWidth.EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            };
            Storyboard.SetTarget(daWidth, _view);
            Storyboard.SetTargetProperty(daWidth, new PropertyPath("(Canvas.Width)"));
            sb.Children.Add(daWidth);

            DoubleAnimation daHeight = AnimationHelper.GetDoubleAnimation(robotwidthorig, robotwidthfinal, seconds * 1000);
            daHeight.EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            };
            Storyboard.SetTarget(daHeight, _view);
            Storyboard.SetTargetProperty(daHeight, new PropertyPath("(Canvas.Height)"));
            sb.Children.Add(daHeight);

            DoubleAnimation daleft = AnimationHelper.GetDoubleAnimation(robotwidthorig, robotwidthfinal, seconds * 1000);
            daleft.EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            };
            Storyboard.SetTarget(daleft, _view); DoubleAnimation dtop = AnimationHelper.GetDoubleAnimation(_originalTop, stopatShelves, seconds * 1000);
            Storyboard.SetTarget(dtop, _view);
            Storyboard.SetTargetProperty(dtop, new PropertyPath("(Canvas.Top)"));
            dtop.EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 5
            };
            sb.Children.Add(dtop);
            sb.Completed += (s, e) =>
            {
                //show rear view
                Task.Delay(500).ContinueWith(t =>
                {
                    _view.Dispatcher.Invoke(() =>
                    {
                        //show front view
                        _view.SetFrontiew();
                        _view.RobotAllDone();
                    });
                });
            };
            sb.Begin();
        }
    }
}
