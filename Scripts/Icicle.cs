using Godot;
using System;

public partial class Icicle : Node2D
{
    [Export] public float SpinSpeed = 180f;
    [Export] public float Level = 1f;

    private Node2D[] connectedNodes = new Node2D[4];

    public override void _Ready()
    {
        // Assume the 4 connected nodes are the first 4 children of this node
        for (int i = 0; i < 4; i++)
        {
            connectedNodes[i] = GetChild<Node2D>(i);
        }
        UpdateActiveNodes();
    }
    public override void _Process(double delta)
    {
        RotationDegrees += SpinSpeed * (float)delta;
        RotationDegrees %= 360f;
    }
    public void LevelUp()
    {
        Level += 1;
        if (Level == 5)
        {
            SpinSpeed += 100;
        }
        UpdateActiveNodes();
    }
    private void UpdateActiveNodes()
    {
        int activeCount = (int)Mathf.Clamp(Level, 0, 4);

        for (int i = 0; i < 4; i++)
        {
            bool shouldBeActive = i < activeCount;
            connectedNodes[i].Visible = shouldBeActive; // hides/shows node + all its children

            var collider = connectedNodes[i].GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
            if (collider != null)
            {
                collider.Disabled = !shouldBeActive;
            }

        }
    }
}

