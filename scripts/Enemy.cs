using Godot;
using System;

public class Enemy : KinematicBody2D
{
    Vector2 direction;
    float speed = 0f;

    float hp = 10f;

    Timer timer;
    bool isJustSpawned = true;

    public override void _Ready()
    {
        Rotation = (float) GD.RandRange(0, 2 * Mathf.Pi);
        var angle = Rotation;
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        timer = new Timer();
        timer.WaitTime = 1.4f;
        AddChild(timer);
        timer.Start();
    }

    public override async void  _Process(float delta)
    {
        if(isJustSpawned) {
            await ToSignal(timer, "timeout");
            isJustSpawned = false;
        }
        else {
            Position += speed * direction * delta;
        }
    }

    public void SetSpeed(float newSpeed) {
        speed = newSpeed;
    }
}
