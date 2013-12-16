using System;
using System.Windows.Media.Animation;
using Microsoft.Phone.Controls;
using System.Windows;

namespace Roma_Tre_WiFi.Views
{
    public class FadeTransition : TransitionElement
    {
        double from;
        double to;
        int span;
        int delay;

        public FadeTransition(double from, double to, int span, int delay)
        {
            this.from = from;
            this.to = to;
            this.span = span;
            this.delay = delay;
        }

        public override ITransition GetTransition(UIElement element)
        {
            Storyboard FadingStoryBoard = CreateStoryboard();
            Storyboard.SetTarget(FadingStoryBoard, element);
            return new CustomTransition(FadingStoryBoard);
        }

        protected Storyboard CreateStoryboard()
        {
            Storyboard result = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = from;
            animation.To = to;
            animation.Duration = new Duration(new TimeSpan(0, 0, 0, 0, span));
            animation.BeginTime = new TimeSpan(0, 0, 0, 0, delay);

            Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));
            result.Children.Add(animation);
            return result;
        }
    }

    public class FadeInTransition : FadeTransition
    {
        public FadeInTransition()
            : base(0.0, 1.0, 400, 0)
        {
        }

        public FadeInTransition(int span)
            : base(0.0, 1.0, span, 0)
        {
        }

        public FadeInTransition(int span, int delay)
            : base(0.0, 1.0, span, delay)
        {
        }
    }

    public class FadeOutTransition : FadeTransition
    {
        public FadeOutTransition()
            : base(1.0, 0.0, 400, 0)
        {
        }

        public FadeOutTransition(int span)
            : base(1.0, 0.0, span, 0)
        {
        }

        public FadeOutTransition(int span, int delay)
            : base(1.0, 0.0, span, delay)
        {
        }
    }

    public class CustomTransition : ITransition
    {
        Storyboard storyboard;

        public CustomTransition(Storyboard sb)
        {
            storyboard = sb;
        }

        // these are used by the transition

        public void Begin()
        {
            storyboard.Begin();
        }

        public event EventHandler Completed
        {
            add
            {
                storyboard.Completed += value;
            }
            remove
            {
                storyboard.Completed -= value;
            }
        }

        // not used for SplashScreen, but needed to create the whole animation
        // if you don´t add these, you will not be able to build your project

        public void Stop()
        {
            storyboard.Stop();
        }

        public void Pause()
        {
            storyboard.Pause();
        }

        public void Resume()
        {
            storyboard.Resume();
        }

        public void SkipToFill()
        {
            storyboard.SkipToFill();
        }

        public void Seek(TimeSpan offset)
        {
            storyboard.SeekAlignedToLastTick(offset);
        }

        public void SeekAlignedToLastTick(TimeSpan offset)
        {
            storyboard.SeekAlignedToLastTick(offset);
        }

        public ClockState GetCurrentState()
        {
            return storyboard.GetCurrentState();
        }

        public TimeSpan GetCurrentTime()
        {
            return storyboard.GetCurrentTime();
        }
    }
}