using Godot;
using System;

public partial class Camera : Camera2D
{
    private Random _random = new Random();
    private Vector2 _originalPosition;

    private float _shakeDuration = 1f;
    private float _shakeIntensity = 10f;

    public override void _Ready()
    {
        _originalPosition = Position;
    }

    public override void _Process(double delta)
    {
        ShakeCamera(delta);
    }

    public void StartShakeCamera(float duration, float intensity)
    {
        _shakeDuration = duration;
        _shakeIntensity = intensity;
    }

    public void ShakeCamera(double delta) {
        // check if the shake time is over, if not shake the camera
        if (_shakeDuration > 0)
        {
            float offsetX = (float)(_random.NextDouble() * 2 - 1) * _shakeIntensity;
            float offsetY = (float)(_random.NextDouble() * 2 - 1) * _shakeIntensity;

            Position = _originalPosition + new Vector2(offsetX, offsetY);

            _shakeDuration -= (float)delta;


            // Reset the camera location when shake is over
            if (_shakeDuration <= 0)
            {
                Position = _originalPosition;
            }
        }
    }
}
