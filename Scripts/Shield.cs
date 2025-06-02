using Godot;
using System;

public partial class Shield : Area2D
{
    public void _on_area_entered(Node body)
    {
            GD.Print("TEST");

        if (body.IsInGroup("Projectiles"))
        {
            GD.Print("TEST");
            body.QueueFree();

        }
    }
}
