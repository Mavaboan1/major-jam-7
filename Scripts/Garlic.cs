using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public partial class Garlic : Area2D
{
    [Export]public float Level;
    [Export] public Timer timer;
    [Export] private AnimatedSprite2D animatedSprite;
    public CollisionShape2D collisionShape;
    
    private List<Node> Garliclist = new List<Node>();

    public override void _Ready()
    {
        collisionShape = GetNode<CollisionShape2D>("GarlicRange");

        Level = 1f;
        Vector2 newScale = new Vector2(Level, Level);
        collisionShape.Scale = newScale;
        animatedSprite.Play("default");
    }
    public void _on_body_entered(Node body)
    {

        if (body.HasMethod("takeDamage") != false)
        {
            Garliclist.Add(body);
        }
    }
    public void _on_body_exited(Node boody)
    {
        if (boody.HasMethod("takeDamage") != false)
        {
            Garliclist.Remove(boody);
        }
    }
    public void _on_timer_timeout()
    {
        for (int i = 0; i < Garliclist.Count; i++)
        {
            if (Level == 5)
            {
                Garliclist[i].Call("takeDamage", 4);

            }
            else
            {
                Garliclist[i].Call("takeDamage", 2);

            }
        }
    }

    public override void _Process(double delta)
    {

        

    }
    public void LevelUp()
    {
        Level += 1;
        Vector2 newScale = new Vector2(Level, Level);
        collisionShape.Scale = newScale;
    }
}
