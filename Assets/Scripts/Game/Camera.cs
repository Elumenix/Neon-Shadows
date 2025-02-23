using Godot;
using System;

public partial class Camera : Camera2D
{
    public static Camera Instance { get; private set; }

    private Random _random = new Random();
    private Vector2 _originalPosition;

    private float _shakeDuration = 1f;
    private float _shakeIntensity = 10f;

    public override void _Ready()
    {
        Instance = this;
    }

    public override void _Process(double delta)
    {
        _originalPosition = GameManager.Instance.player.GlobalPosition;
        ShakeCamera(delta);
    }

    public void StartShakeCamera(float duration, float intensity)
    {
        _shakeDuration = duration;
        _shakeIntensity = intensity;
        DragHorizontalEnabled = false;
        DragVerticalEnabled = false;
    }

    public void ShakeCamera(double delta) {
        // check if the shake time is over, if not shake the camera
        if (_shakeDuration > 0)
        {
            float offsetX = (float)(_random.NextDouble() * 2 - 1) * _shakeIntensity;
            float offsetY = (float)(_random.NextDouble() * 2 - 1) * _shakeIntensity;

            GlobalPosition = _originalPosition + new Vector2(offsetX, offsetY);

            _shakeDuration -= (float)delta;


            // Reset the camera location when shake is over
            if (_shakeDuration <= 0)
            {
                GlobalPosition = _originalPosition;
                DragHorizontalEnabled = true;
                DragVerticalEnabled = true;
            }
        }
    }
}
