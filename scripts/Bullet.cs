using Godot;
using System;

public class Bullet : Area2D
{
    Vector2 direction;
    [Export] float minSpeed = 100f;
    [Export] float maxSpeed = 300f;
    float speed;
    float randomNum;
    public override void _Ready()
    {
        randomNum = (float) GD.RandRange(0, 100f);
        if(randomNum < 50) {
            LookAt(GetNode<Player>("/root/Main/Player").Position);
        }
        direction = new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation));
        speed = (float) GD.RandRange(minSpeed, maxSpeed);
    }

    public override void _PhysicsProcess(float delta)
    {
        Position += direction * speed * delta;   
    }

    void _on_Bullet_body_entered(Player body) {
        if(body.GetType() == typeof(Player)) {
            body.Die();
        }
    }

    void _on_VisibilityNotifier2D_viewport_exited(Viewport vp) {
        QueueFree();
    }
}
