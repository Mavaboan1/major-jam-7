using Godot;
using System;

public partial class Character : CharacterBody2D
{
	public float moveSpeed = 150.0f;
	[Export] float Health = 100f;
	public Area2D hurtBox;
	[Export] public Resource stats;
	float damageRate;
	public int mobs = 0;

	public override void _Ready()
	{
		hurtBox = GetNode<Area2D>("%HurtBox");
		damageRate = (float)stats.Get("damage");
		GD.Print(damageRate);
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
		if (mobs >= 0)
		{
			Health -= damageRate * mobs * (float)delta;
		}
		GD.Print(Health);
	}

	public void _on_hurt_box_body_entered(Node body)
	{
		mobs += 1;
	}
	

	
}
