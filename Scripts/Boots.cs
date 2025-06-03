using Godot;
using System;

public partial class Boots : Node2D
{
    [Export] public float Level = 1f;

public override void _Ready()
    {
        Node player = GetTree().Root.GetNode("Main/Character");
        player.Call("SpeedUp");
    }
    public void LevelUp()
    {
        Level += 1;
        if (Level == 5)
        {
            Node player = GetTree().Root.GetNode("Main/Character");
            player.Call("SpeedUp");
            player.Call("SpeedUp");
            player.Call("SpeedUp");

        }
        else
        {
            Node player = GetTree().Root.GetNode("Main/Character");
            player.Call("SpeedUp");
        }
        
    }
}
