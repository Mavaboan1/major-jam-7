using Godot;
using System;

public partial class Ice_Shard : Area2D
{
    public void _on_body_entered(Node body)
    {
                    GD.Print("Testsadsdasd");

        if (body.HasMethod("takeDamage") != false)
        {
            body.Call("takeDamage", 5);

        }
    }
    
}
