using Godot;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

public partial class Character : CharacterBody2D
{

	public int Experience;
	public float moveSpeed = 150.0f;
	[Export] float Health = 100f;
	public Area2D hurtBox;
	[Export] public Resource stats;
	float damageRate;
	[Export] public float Range;
	bool Slowed;
	[Signal] public delegate void rangeSignalEventHandler(float Range);
	public int mobs = 0;
	[Export] public ProgressBar expBar;
	[Export] public Control Itemmenu;
	[Export] public ProgressBar HealthBar;
	[Export] public WildMagic wildController;
	[Export] private AnimatedSprite2D animatedSprite;
	private List<Node> Touchlist = new List<Node>();

	public bool isDead = false;



	public override void _Ready()
	{
		hurtBox = GetNode<Area2D>("%HurtBox");
		damageRate = (float)stats.Get("damage");
		HealthBar.MaxValue = Health;
		HealthBar.Value = Health;
	}

	public override void _Process(double delta)
	{
		if (isDead) return;
		expBar.Value = Experience;
		HealthBar.Value = Health;
		getExp(2);
		if (Experience >= 500)
		{
			Itemmenu.Call("Active");
			Experience -= 500;
		}

		Vector2 velocity = Velocity;

		velocity.X = 0;
		velocity.Y = 0;
		if (Input.IsActionPressed("move_left"))
		{
			playAnimation("walk");
			animatedSprite.FlipH = true;
			velocity.X = -moveSpeed;

		}
		else if (Input.IsActionPressed("move_right"))
		{
			playAnimation("walk");
			animatedSprite.FlipH = false;
			velocity.X = +moveSpeed;

		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y = +moveSpeed;
		}
		else if (Input.IsActionPressed("move_up"))
		{
			velocity.Y = -moveSpeed;
		}

		if (velocity == Vector2.Zero)
		{
			playAnimation("idle");
		}

		Velocity = velocity;
		MoveAndSlide();
		if (Touchlist.Count > 0)
		{
			Health -= damageRate * Touchlist.Count * (float)delta;
			if (!isDead)
			{
				animatedSprite.Play("take_damage");
			}
		}

		if (Health <= 0)
		{
			isDead = true;
			animatedSprite.Stop();
			die();
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
	public void RangeUp()
	{
		Range += 20f;
		EmitSignal(SignalName.rangeSignal,Range);
	}
	public void HealthUp()
	{
		HealthBar.MaxValue += 20;
		Health += 20f;
		HealthBar.Value = Health;
	}

	public void playAnimation(string newAnimation)
	{
		if (animatedSprite.Animation != newAnimation)
		{
			animatedSprite.Play(newAnimation);
		}
	}

	public void takePlayerDamage(int damageTaken)
	{
		Health -= damageTaken;
		if (!isDead)
		{
			animatedSprite.Play("take_damage");
		}
		
	}

	public void _on_hurt_box_body_entered(Node body)
	{
		if (body.HasMethod("takeDamage") != false)
		{
			Touchlist.Add(body);
		}

	}

	public async Task die()
	{
		playAnimation("death");
		animatedSprite.FrameChanged += () => onFrameChanged(animatedSprite);
	}

	private void onFrameChanged(AnimatedSprite2D sprite)
	{
		if (sprite.Frame == sprite.SpriteFrames.GetFrameCount(sprite.Animation) - 1)
		{
			QueueFree();
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
	public void getWild(int Wild)
	{
		wildController.Call("GetWild", Wild);
	}
	public void Invert()
	{
		moveSpeed *= -1;
	}
	public void Slow()
	{
		if (Slowed == false)
		{
			moveSpeed -= 100f;
			Slowed = true;
		}
		else if (Slowed == true)
		{
			moveSpeed += 100f;
			Slowed = false;
		}
	}
	

}
