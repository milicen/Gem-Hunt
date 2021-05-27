using Godot;
using System;

public class Boomerang : Area2D
{
    [Export] PackedScene gemScene;

    Player player;
    Vector2 direction;
    Vector2 startPoint;
    float maxDistance = 250f;
    float setDistance;
    float boomerangSpeed = 400f;
    float angle;
    bool isFlying = false;
    int clickCount = 0;

    public override void _Ready()
    {
        player = GetNode<Player>("/root/Main/Player");
        player.Connect("PlayerDie", this, "_on_PlayerDie");
    }

    public override void _Process(float delta)
    {
        if(!isFlying) {
            GlobalPosition = player.GlobalPosition;
            GetNode<CollisionShape2D>("CollisionShape2D").RotationDegrees = 0;
            LookAt(GetGlobalMousePosition());
        }
        else {
            GetNode<CollisionShape2D>("CollisionShape2D").RotationDegrees += 2000f * delta;

            var currentDistance = (GlobalPosition - startPoint).Length();

            if(currentDistance < setDistance) {
                GlobalPosition += direction.Normalized() * boomerangSpeed * delta;
            }
            else {
                setDistance = 0;

                LookAt(player.Position);
                angle = Rotation;

                direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                currentDistance = (player.GlobalPosition - GlobalPosition).Length();
                
                GlobalPosition += direction.Normalized() * boomerangSpeed * delta;

                if(currentDistance <= 3f) {
                    isFlying = false;
                    clickCount = 0;
                }

            }
        }

        if(Input.IsActionJustPressed("shoot") && clickCount < 1) {
            isFlying = true;
            clickCount++;
            setDistance = maxDistance;

            startPoint = player.GlobalPosition;

            var target = GetGlobalMousePosition();

            LookAt(target);

            angle = Rotation;
            direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
        
    }

    void _on_Boomerang_body_entered(Enemy body) {
        if(body.IsInGroup("enemy")) {
            // body.QueueFree();
            // var enemyBody = (Enemy) body.GetParent();
            body.TakeDamage();
            SpawnGem(body.GlobalPosition);
        }
    }

    void _on_PlayerDie() {
        Hide();
    }

    void SpawnGem(Vector2 spawnPos) {
        // instance a gem
        var gem = (Area2D) gemScene.Instance();
        // set position to current enemy position
        gem.Position = spawnPos;
        // add as Main's child
        GetParent().AddChild(gem);
    }

}
