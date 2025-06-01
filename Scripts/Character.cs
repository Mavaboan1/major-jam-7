using Godot;
using System;
using System.Collections.Generic;

public partial class Character : CharacterBody2D
{

	public int Experience;
	public float moveSpeed = 150.0f;
	[Export] float Health = 100f;
	public Area2D hurtBox;
	[Export] public Resource stats;
	float damageRate;
	public int mobs = 0;
	[Export] public ProgressBar expBar;
	[Export] public Control Itemmenu;
	[Export] public ProgressBar HealthBar;
	private List<Node> Touchlist = new List<Node>();



	public override void _Ready()
	{
		hurtBox = GetNode<Area2D>("%HurtBox");
		damageRate = (float)stats.Get("damage");
		GD.Print(damageRate);
		HealthBar.MaxValue = Health;
		HealthBar.Value = Health;
	}

	public override void _Process(double delta)
	{
		expBar.Value = Experience;
		HealthBar.Value = Health;

		if (Experience >= 500)
		{
			Itemmenu.Call("Active");
			Experience -= 500;
		}
		getExp(1);

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
		if (Touchlist.Count > 0)
		{
			Health -= damageRate * Touchlist.Count * (float)delta;
		}

		if (Health <= 0)
		{
			QueueFree();
		}
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
	public void SpeedUp()
	{
		moveSpeed += 20f;
	}

	public void takePlayerDamage(int damageTaken)
	{
		Health -= damageTaken;
	}

	public void _on_hurt_box_body_entered(Node body)
	{
		if (body.HasMethod("takeDamage") != false)
		{
			Touchlist.Add(body);
		}

	}

	public void _on_hurt_box_body_exited(Node body)
	{
		if (body.HasMethod("takeDamage") != false)
		{
			Touchlist.Remove(body);
		}
	}
    public void getExp(int Exp)
	{
		Experience += Exp;
	}

}
