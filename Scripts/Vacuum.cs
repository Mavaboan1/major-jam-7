using Godot;
using System;

public partial class Vacuum : Node2D
{
    [Export] public float Level = 6f;

public override void _Ready()
    {
        Node player = GetTree().Root.GetNode("Main/Character");
        player.Call("RangeUp");
    }
    public void LevelUp()
    {
        Level += 1;
        
        
            Node player = GetTree().Root.GetNode("Main/Character");
            player.Call("RangeUp");
               
    }
}
