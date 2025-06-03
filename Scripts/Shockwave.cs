using Godot;
using System;

public partial class Shockwave : Node2D
{
    [Export] PackedScene bullet_scn;
    [Export]public float Level = 1;

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
            var bullet = bullet_scn.Instantiate<Wave>();
            bullet.Rotation = GlobalRotation;
            bullet.GlobalPosition = GlobalPosition;
            if (Level == 5)
            {
                bullet.Call("level5");
            }
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
