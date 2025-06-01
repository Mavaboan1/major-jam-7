using Godot;
using System;

public partial class MagicMissile : Node2D
{
    [Export] public float Level = 1f;
    [Export] PackedScene bullet_scn;
    [Export] float bullet_speed = 600f;
    [Export] float bps = 1f; // bullets per second
    [Export] float bullet_damage = 5f;

    float fire_rate;
    float time_until_fire = 0f;

    public override void _Ready()
    {
        fire_rate = 1 / bps;
    }

    public override void _Process(double delta)
    {
        time_until_fire += (float)delta;

        if (time_until_fire >= fire_rate)
        {
            foreach (Node child in GetChildren())
            {
                if (child is Node2D spawnPoint)
                {
                    RigidBody2D bullet = bullet_scn.Instantiate<RigidBody2D>();
                    bullet.Rotation = (float)GD.RandRange(0, Mathf.Tau);
                    bullet.GlobalPosition = spawnPoint.GlobalPosition;
                    bullet.LinearVelocity = bullet.Transform.X * bullet_speed;
                    GetTree().Root.AddChild(bullet);
                }
            }

            time_until_fire = 0f;
        }
    }

    public void LevelUp()
    {
        Level += 1;
        bps += 1;
        fire_rate = 1 / bps;
    }
}
