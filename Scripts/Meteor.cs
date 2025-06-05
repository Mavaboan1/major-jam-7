using Godot;
using System;

public partial class Meteor : Area2D
{
    private Vector2 dir;
    public void _on_body_entered(Node body)
    {

        QueueFree();
        if (body.HasMethod("takeDamage") != false)
        {
            body.Call("takeDamage", 4);

        }
    }
    public override void _Ready() {
        dir = Transform.X.Normalized();
    }


    public override void _Process(double delta) {
        GlobalPosition += dir * 600f * (float)delta;
    }
}
