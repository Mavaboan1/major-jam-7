using Godot;
using System;

public partial class Garlic : Area2D
{
    public float Level;
    public CollisionShape2D collisionShape;

    public override void _Ready()
    {
        collisionShape = GetNode<CollisionShape2D>("GarlicRange");

        Level = 1f;
        Vector2 newScale = new Vector2(Level, Level);
        collisionShape.Scale = newScale;
    }

    public override void _Process(double delta)
    {
        
    }
    public void LevelUp()
    {
        Level += 1;
        Vector2 newScale = new Vector2(Level, Level);
        collisionShape.Scale = newScale;
    }
}
