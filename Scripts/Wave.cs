using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class Wave : Area2D
{
    [Export] public float Speed = 600f;
    [Export] public float KnockbackForce = 300f;

    private Vector2 direction;
    public CollisionShape2D collisionShape;
    public bool level5active;


    public override void _Ready()
    {
        if (level5active)
        {
            collisionShape = GetNode<CollisionShape2D>("ShockRange");
            Vector2 newScale = new Vector2(0.5f, 0.5f);
            collisionShape.Scale = newScale;
        }
        

        Vector2 mousePos = GetGlobalMousePosition();
        direction = (mousePos - GlobalPosition).Normalized();

        // Rotate to face the direction of movement
        Rotation = direction.Angle();

        GetTree().CreateTimer(2f).Timeout += () => QueueFree();
    }

    public override void _Process(double delta)
    {
        GlobalPosition += direction * Speed * (float)delta;
    }
    public void _on_body_entered(Node body)
    {

        if (body.HasMethod("takeDamage") != false)
        {
            body.Call("takeDamage", 2);

        }
        if (body.HasMethod("apply_knockback"))
        {
            body.Call("apply_knockback", direction * KnockbackForce);
        }
    }
    public void level5()
    {
        GD.Print("asdasd");
        level5active = true;
    }
}
