using Godot;
using System;

public partial class Missile : RigidBody2D
{
    [Export] public float Speed = 600f;
    [Export] public float TurnSpeed = 5f;

    private Node2D target;

    public override void _Ready()
    {
        LinearVelocity = Transform.X * Speed;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (target == null || !IsInstanceValid(target))
        target = FindClosestEnemy();

    if (target == null)
        return;


        Vector2 toTarget = (target.GlobalPosition - GlobalPosition).Normalized();
        Vector2 currentDirection = Transform.X;

        float angleToTarget = currentDirection.AngleTo(toTarget);
        float maxTurn = TurnSpeed * (float)delta;


        angleToTarget = Mathf.Clamp(angleToTarget, -maxTurn, maxTurn);

        Rotation += angleToTarget;


        LinearVelocity = Transform.X * Speed;
    }
    private Node2D FindClosestEnemy()
{
    Node2D closestEnemy = null;
    float closestDistance = float.MaxValue;

    // Change this to the parent node holding all enemies
    Node enemiesParent = GetTree().Root.GetNodeOrNull<Node>("Main/Enemy");

    if (enemiesParent == null)
        return null;

    foreach (Node enemyNode in enemiesParent.GetChildren())
    {
        if (enemyNode is Node2D enemy)
        {
            float dist = GlobalPosition.DistanceTo(enemy.GlobalPosition);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestEnemy = enemy;
            }
        }
    }

    return closestEnemy;
}
}
