using Godot;
using System;

public partial class BulletPoint : Node2D
{
     [Export] PackedScene bullet_scn;

    [Export] float bullet_speed = 600f;
    [Export] float bps = 1f;
    [Export] float bullet_damage = 5f;
    float fire_rate;
    float time_until_fire = 0f;
    public override void _Ready()
    {
        fire_rate = 1 / bps;
    }
    public override void _Process(double delta)
    {
        if (time_until_fire > fire_rate)
        {
            Area2D bullet = bullet_scn.Instantiate<Area2D>();
            bullet.Rotation = GlobalRotation;
            bullet.GlobalPosition = GlobalPosition;

            GetTree().Root.AddChild(bullet);
            time_until_fire = 0f;
        }
        else
        {
            time_until_fire += (float)delta;
        }
    }
}


