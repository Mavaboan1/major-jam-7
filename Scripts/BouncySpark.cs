using Godot;
using System;

public partial class BouncySpark : Area2D
{
    [Export] public float Speed = 200f;
    private Vector2 velocity;
    public CollisionShape2D collisionShape;

    [Export]public float Level = 1f;
    [Export] private AnimatedSprite2D animatedSprite;


    public override void _Ready()
    {
        animatedSprite.Play("default");
        collisionShape = GetNode<CollisionShape2D>("SparkRange");
        float angle = (float)GD.RandRange(0, 2 * Mathf.Pi);
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Speed;
    }

    public override void _Process(double delta)
    {
        GlobalPosition += velocity * (float)delta;

        Camera2D camera = GetViewport().GetCamera2D();
        if (camera == null) return;

        Vector2 cameraCenter = camera.GlobalPosition;
        Vector2 halfViewportSize = GetViewportRect().Size / 2;

        float margin = 5f;

        float left = cameraCenter.X - halfViewportSize.X + margin;
        float right = cameraCenter.X + halfViewportSize.X - margin;
        float top = cameraCenter.Y - halfViewportSize.Y + margin;
        float bottom = cameraCenter.Y + halfViewportSize.Y - margin;

        Vector2 pos = GlobalPosition;

        if (pos.X < left || pos.X > right)
            velocity.X *= -1;

        if (pos.Y < top || pos.Y > bottom)
            velocity.Y *= -1;
    }

    public void _on_body_entered(Node body)
    {

        if (body.HasMethod("takeDamage") != false)
        {
            if (Level == 5)
            {
                body.Call("takeDamage", 8);
            }
            else
            {
                body.Call("takeDamage", 5);

            }

        }
    }
    public void LevelUp()
    {
        Level += 1;
        Speed = 200f + (100f * Level);
        Vector2 newScale = new Vector2(0.5f+(Level/2), 0.5f+(Level/2));
        collisionShape.Scale = newScale;
        float angle = (float)GD.RandRange(0, 2 * Mathf.Pi);
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Speed;
        
    }
}
