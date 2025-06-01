using Godot;
using System;

public partial class Missile : Area2D
{
    private Vector2 dir;

    [Export] public float Speed = 600f;
    [Export] public float TurnSpeed = 5f;

    private Node2D target;

    public override void _Ready()
    {
        dir = Transform.X.Normalized();

    }

    public override void _PhysicsProcess(double delta)
    {
        if (target == null || !IsInstanceValid(target))
        target = FindClosestEnemy();

    if (target == null)
        return;
        GlobalPosition += dir * 600f * (float)delta;



        Vector2 toTarget = (target.GlobalPosition - GlobalPosition).Normalized();

        float angleToTarget = dir.AngleTo(toTarget);
        float maxTurn = TurnSpeed * (float)delta;


        angleToTarget = Mathf.Clamp(angleToTarget, -maxTurn, maxTurn);
        dir = dir.Rotated(angleToTarget).Normalized();

        GlobalPosition += dir * Speed * (float)delta;
        Rotation += angleToTarget;


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
public void _on_body_entered(Node body)
    {

        QueueFree();
        if (body.HasMethod("takeDamage") != false)
        {
            body.Call("takeDamage", 5);

        }
    }
}
