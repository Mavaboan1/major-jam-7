using Godot;
using System;

public partial class Ice_Shard : Area2D
{
    public void _on_body_entered(Node body)
    {

        if (body.HasMethod("takeDamage") != false)
        {
            body.Call("takeDamage", 4);

        }
    }
    
}
