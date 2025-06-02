using Godot;
using System;

public partial class Tornado : Area2D
{
    [Export] public float Speed = 200f;
    [Export] public float DirectionChangeInterval = 0.25f;
    [Export] public float Lifetime = 3f;

    private Vector2 direction = Vector2.Zero;
    private float directionTimer = 0f;
    private float lifetimeTimer = 0f;

    public override void _Ready()
    {
        ChooseNewDirection();
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;

        GlobalPosition += direction * Speed * dt;

        directionTimer += dt;
        if (directionTimer >= DirectionChangeInterval)
        {
            ChooseNewDirection();
            directionTimer = 0f;
        }

        lifetimeTimer += dt;
        if (lifetimeTimer >= Lifetime)
        {
            QueueFree();
        }
    }

    private void ChooseNewDirection()
    {
        float angle = (float)GD.RandRange(0, Mathf.Tau); // 0 to 2π
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).Normalized();
    }
    public void _on_body_entered(Node body)
    {

        if (body.HasMethod("takeDamage") != false)
        {
            body.Call("takeDamage", 5);

        }
    }
}
