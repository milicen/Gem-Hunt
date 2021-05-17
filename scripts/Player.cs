using Godot;
using System;

public class Player : KinematicBody2D
{
    Vector2 velocity = Vector2.Zero;
    [Export] float movementSpeed = 20000f;

    public override void _Ready()
    {

    }

    public override void _Process(float delta) {
        GetNode<CollisionShape2D>("CollisionShape2D").LookAt(GetGlobalMousePosition());
        
    }

    public override void _PhysicsProcess(float delta)
    {
        MoveInputHandler(delta);
    }

    void MoveInputHandler(float delta) {
        velocity = new Vector2(
                Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"), 
                Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
            ) * movementSpeed;

        velocity = MoveAndSlide(velocity * delta);
    }
}
