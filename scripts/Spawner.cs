using Godot;
using System;

public class Spawner : PathFollow2D
{
    Timer timer;
    [Export] PackedScene enemyScene;
    [Export] PackedScene bulletScene;

    [Export] float minTimer = .2f;
    [Export] float maxTimer = 1f;

    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        RandomizeWaitTime();
    }

    public override void _Process(float delta) {
        UnitOffset += delta;
    }

    void _on_Timer_timeout() {
        // spawn bullet

        // instance bullet
        var bullet = (Bullet) bulletScene.Instance();

        // set bullet position to spawner position
        bullet.Position = Position;

        // rotate 90 degrees
        RotationDegrees += 90;

        // set random angle from -60 till 60 deg
        var randAngle = (float) GD.RandRange(-30f, 30f);

        // add random angle to current rotation and assign to bullet
        RotationDegrees += randAngle;
        bullet.RotationDegrees = RotationDegrees;
        // add bullet as Main's child
        GetNode<Node>("/root/Main").AddChild(bullet);
        RandomizeWaitTime();
    }

    void RandomizeWaitTime() {
        timer.WaitTime = (float) GD.RandRange(minTimer, maxTimer);
    }

    public void StartTimer() {
        timer.Start();
        GD.Print("spawner timer started");
    }

    public void StopTimer() {
        timer.Stop();
        RandomizeWaitTime();
    }
}
