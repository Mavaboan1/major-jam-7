using Godot;
using System;

public partial class RecoveryBelt : Node2D
{
    private Node player;
    public float Level = 1f;
    private Vector2 lastPosition;
    private float idleTimer = 0f;
    private float callTimer = 0f;

    private const float IdleTimeRequired = 2f;
    private const float CallInterval = 1f;

    private bool isIdle = false;

    public override void _Ready()
    {
        player = GetTree().Root.GetNode("Main/Character");
        if (player == null)
            GD.PushError("Player node not found at Main/Character.");

        lastPosition = ((Node2D)player).GlobalPosition;
    }

    public override void _Process(double delta)
    {
        if (player == null)
            return;

        Vector2 currentPos = ((Node2D)player).GlobalPosition;
        if (currentPos.DistanceTo(lastPosition) < 0.1f)
        {
            idleTimer += (float)delta;

            if (!isIdle && idleTimer >= IdleTimeRequired)
            {
                isIdle = true;
                callTimer = 0f;  // reset call timer to immediately call
            }

            if (isIdle)
            {
                callTimer += (float)delta;
                if (callTimer >= CallInterval)
                {
                    callTimer = 0f;
                    player.Call("takePlayerDamage", -Level); // Change to the method you want to call
                }
            }
        }
        else
        {
            isIdle = false;
            idleTimer = 0f;
            callTimer = 0f;
        }

        lastPosition = currentPos;
    }
    public void LevelUp()
    {
        Level += 1;
        

    }
}
