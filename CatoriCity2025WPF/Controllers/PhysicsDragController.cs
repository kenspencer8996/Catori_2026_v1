using System;
using System.Windows;
using System.Windows.Media;

public class PhysicsDragController
{
    private readonly Action<Point> _setPosition;

    private Point _currentPos;
    private Point _targetPos;
    private Vector _velocity;

    private bool _isRunning;

    private const double Stiffness = 0.20;
    private const double Damping = 0.80;

    public PhysicsDragController(Action<Point> setPosition)
    {
        _setPosition = setPosition;
    }

    public void Start(Point initialPos)
    {
        _currentPos = initialPos;
        _targetPos = initialPos;
        _velocity = new Vector(0, 0);

        if (!_isRunning)
        {
            CompositionTarget.Rendering += OnRendering;
            _isRunning = true;
        }
    }

    public void SetTarget(Point target)
    {
        _targetPos = target;
    }

    public void Stop()
    {
        if (_isRunning)
        {
            CompositionTarget.Rendering -= OnRendering;
            _isRunning = false;
        }
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        var toTarget = _targetPos - _currentPos;
        var force = new Vector(toTarget.X * Stiffness, toTarget.Y * Stiffness);

        _velocity += force;
        _velocity *= Damping;

        _currentPos += _velocity;

        _setPosition(_currentPos);
    }
}
