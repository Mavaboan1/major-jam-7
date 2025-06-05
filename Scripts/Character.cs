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
	[Export] public Control DeathScreen;
	[Export] public Control WinScreen;
	[Export] public Control PauseScreen;

	private List<Node> Touchlist = new List<Node>();

	public bool isDead = false;



	public override void _Ready()
	{
		hurtBox = GetNode<Area2D>("%HurtBox");
		damageRate = (float)stats.Get("damage");
		HealthBar.MaxValue = Health;
		HealthBar.Value = Health;
		var music = GetNode<AudioStreamPlayer>("/root/MenuMusic/AudioStreamPlayer2D2");
		music.Stop();
		Range = 60f;
	}

	public override void _Process(double delta)
	{
		EmitSignal(SignalName.rangeSignal,Range);

		if (isDead) return;
		expBar.Value = Experience;
		HealthBar.Value = Health;
		if (Experience >= 100)
		{
			Itemmenu.Call("Active");
			Experience -= 100;
		}

		Vector2 input = Vector2.Zero;

		if (Input.IsActionPressed("Pause"))
		{
			PauseScreen.Call("Activate");
}
if (Input.IsActionPressed("move_left"))
		{
			input.X -= 1;
			playAnimation("walk");
			animatedSprite.FlipH = true;
		}
		else if (Input.IsActionPressed("move_right"))
		{
			input.X += 1;
			playAnimation("walk");
			animatedSprite.FlipH = false;
		}

if (Input.IsActionPressed("move_down"))
{
    input.Y += 1;
}
else if (Input.IsActionPressed("move_up"))
{
    input.Y -= 1;
}

if (input == Vector2.Zero)
{
    playAnimation("idle");
}
else
{
    input = input.Normalized();
}

Velocity = input * moveSpeed;
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
		moveSpeed += 10f;
	}
	public void RangeUp()
	{
		Range += 30f;
	}
	public void HealthUp()
	{
		HealthBar.MaxValue += 10;
		Health += 10f;
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
			DeathScreen.Call("Activate");
			
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
	public void Heal(int healing)
	{
		Health += healing;
		
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
			moveSpeed -= 50f;
			Slowed = true;
		}
		else if (Slowed == true)
		{
			moveSpeed += 50f;
			Slowed = false;
		}
	}
	

}
