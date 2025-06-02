using Godot;
using System;

public partial class Tornadoes : Node2D
{
    [Export] public float Level = 1f;

    [Export] PackedScene bullet_scn;

    [Export] float bps = 1f;
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
            bullet.GlobalPosition = GlobalPosition;

            GetTree().Root.AddChild(bullet);
            time_until_fire = 0f;
        }
        else
        {
            time_until_fire += (float)delta;
        }
    }
    public void LevelUp()
    {
        Level += 1;
        bps += 0.5f;
        fire_rate = 1 / bps;

    }
}
