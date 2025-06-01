using Godot;
using System;

public partial class Missile : RigidBody2D
{
    [Export] public float Speed = 600f;
    [Export] public float TurnSpeed = 5f; 

    private Node2D target;

    public override void _Ready()
    {
        target = GetTree().Root.GetNodeOrNull<Node2D>("Main/Enemy"); 
    
      
        LinearVelocity = Transform.X * Speed;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (target == null || !IsInstanceValid(target))
            return;

    
        Vector2 toTarget = (target.GlobalPosition - GlobalPosition).Normalized();
        Vector2 currentDirection = Transform.X;

        float angleToTarget = currentDirection.AngleTo(toTarget);
        float maxTurn = TurnSpeed * (float)delta;

     
        angleToTarget = Mathf.Clamp(angleToTarget, -maxTurn, maxTurn);

        Rotation += angleToTarget;

     
        LinearVelocity = Transform.X * Speed;
    }
}
