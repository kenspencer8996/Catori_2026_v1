using System.Windows;
using System.Windows.Media.Animation;

namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC : CatoriInterfaces.IAnimationPlaybackControl
{
    private readonly Dictionary<(DependencyObject Target, DependencyProperty Property), AnimationClock> _playbackClocks = new();
    private bool _animationsPaused;

    private void ApplyPlaybackAnimation(DependencyObject target, DependencyProperty property, AnimationTimeline? animation,
        HandoffBehavior handoff = HandoffBehavior.SnapshotAndReplace)
    {
        var key = (target, property);
        AnimationClock? clock = animation is null ? null : (AnimationClock)animation.CreateClock(true);
        if (_playbackClocks.Remove(key, out var previous))
        {
            previous.Controller?.Remove();
        }
        if (target is Animatable animatable)
        {
            animatable.ApplyAnimationClock(property, clock, handoff);
        }
        else if (target is UIElement element)
        {
            element.ApplyAnimationClock(property, clock, handoff);
        }
        if (clock is not null)
        {
            _playbackClocks[key] = clock;
            if (_animationsPaused)
            {
                clock.Controller!.Pause();
                clock.Controller.SeekAlignedToLastTick(TimeSpan.Zero, TimeSeekOrigin.BeginTime);
            }
        }
    }

    public void PauseAnimations()
    {
        Dispatcher.VerifyAccess();
        _animationsPaused = true;
        foreach (var clock in _playbackClocks.Values.ToArray())
        {
            clock.Controller!.Pause();
        }
    }

    public void ResumeAnimations()
    {
        Dispatcher.VerifyAccess();
        _animationsPaused = false;
        foreach (var clock in _playbackClocks.Values.ToArray())
        {
            clock.Controller!.Resume();
        }
    }

    public void AdvanceAnimations(TimeSpan elapsed)
    {
        Dispatcher.VerifyAccess();
        if (!_animationsPaused || elapsed < TimeSpan.Zero)
        {
            throw new InvalidOperationException("Animation stepping requires a paused control and a nonnegative interval.");
        }
        foreach (var clock in _playbackClocks.Values.ToArray())
        {
            clock.Controller!.SeekAlignedToLastTick((clock.CurrentTime ?? TimeSpan.Zero) + elapsed, TimeSeekOrigin.BeginTime);
        }
    }
}
