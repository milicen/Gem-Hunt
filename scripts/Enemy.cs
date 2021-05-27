using Godot;
using System;

public class Enemy : KinematicBody2D
{
    public TextureProgress hpBar;
    public CollisionPolygon2D body2D;

    public Vector2 velocity;
    public float speed = 0f;

    public int hp;
    [Export] public int maxHp;

    public Timer timer;
    public bool isJustSpawned = true;

    public override void _Ready()
    {
        hpBar = GetNode<TextureProgress>("TextureProgress");
        body2D = GetNode<CollisionPolygon2D>("CollisionPolygon2D"); 

        hp = maxHp;

        hpBar.MaxValue = maxHp;
        hpBar.Value = hpBar.MaxValue;

        var angle = (float) GD.RandRange(0, 2 * Mathf.Pi);
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        // body2D.GlobalRotation = velocity.Angle();

        GetNode<RayCast2D>("RayCast2D").CastTo = velocity.Normalized() * 50;

        timer = new Timer();
        timer.WaitTime = 1.4f;
        AddChild(timer);
        timer.Start();

    }

    public override async void  _PhysicsProcess(float delta)
    {
        if(isJustSpawned) {
            await ToSignal(timer, "timeout");
            isJustSpawned = false;
        }
        else {            
            var collision = MoveAndCollide(velocity * speed * delta);
            if(collision != null) {
                if(collision.Collider.HasMethod("Die")) {
                    collision.Collider.Call("Die");
                }
                velocity = velocity.Bounce(collision.Normal);
            }

        }

        if(hp <= 0) {
            Die();
        }

    }

    public void SetSpeed(float newSpeed) {
        speed = newSpeed;
    }

    public void TakeDamage() {
        hp--;
        hpBar.Value = hp;
    }

    public void Die() {
        QueueFree();
    }
}
