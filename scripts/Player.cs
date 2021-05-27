using Godot;
using System;

public class Player : KinematicBody2D
{
    [Signal] delegate void PlayerDie();

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

    public void Die() {
        Hide();
        EmitSignal("PlayerDie");
    }

    public void CenterPosition(Vector2 position) {
        Position = position;
    }

    void _on_Area2D_area_entered(Area2D area) {
        if(area.IsInGroup("gem")) {
            GetNode<Main>("/root/Main").SetPoint();
            area.QueueFree();
        }
    }
}
