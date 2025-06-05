using Godot;
using System;

public partial class Testgiver : Node2D
{
    public override void _Process(double delta)
    {
        // Send Garlic on Space press
        if (Input.IsActionJustPressed("ui_accept")) 
        {
            Node player = GetTree().Root.GetNode("Main/Character");

            if (player != null)
            {
                player.Call("PickUpItem", "vacuum");
                GD.Print("Sent Garlic to Player");
            }
            else
            {
                GD.PrintErr("Player node not found!");
            }
        }

        // Send Icicle on J press
        if (Input.IsActionJustPressed("icicle"))
        {
            Node player = GetTree().Root.GetNode("Main/Character");

            if (player != null)
            {
                player.Call("PickUpItem", "bouncy_spark");
                GD.Print("Sent Icicle to Player");
            }
            else
            {
                GD.PrintErr("Player node not found!");
            }
        }
    }
}
