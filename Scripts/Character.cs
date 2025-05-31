using Godot;
using System;

public partial class Character : CharacterBody2D
{
    public float moveSpeed = 150.0f;
    [Export] float Health = 100f;

    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    {
        Vector2 velocity = Velocity;

        velocity.X = 0;
        velocity.Y = 0;
        if (Input.IsKeyPressed(Key.Left))
        {
            velocity.X = -moveSpeed;
        }
        else if (Input.IsKeyPressed(Key.Right))
        {
            velocity.X = +moveSpeed;
        }

        if (Input.IsKeyPressed(Key.Down))
        {
            velocity.Y = +moveSpeed;
        }
        else if (Input.IsKeyPressed(Key.Up))
        {
            velocity.Y = -moveSpeed;
        }

        Velocity = velocity;
        MoveAndSlide();
    }
    public void PickUpItem(string itemName)
    {
         Node itemHandler = GetNodeOrNull("ItemController"); 
    if (itemHandler != null)
    {
        itemHandler.Call("PickUpItem", itemName);
    }
    else
    {
        GD.PrintErr("ItemHandler node not found on player!");
    }
    }
}
