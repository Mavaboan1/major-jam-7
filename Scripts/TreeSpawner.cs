using Godot;
using System;

public partial class TreeSpawner : Node2D
{
    [Export] public PackedScene TreeScene;
    [Export] public int NumberOfTrees = 2000;
    [Export] public Rect2 SpawnArea = new Rect2(-5000, -5000, 10000, 10000); // example area

    public override void _Ready()
    {
        SpawnTrees();
    }

    public void SpawnTrees()
    {
        if (TreeScene == null)
        {
            GD.PrintErr("TreeScene is not assigned!");
            return;
        }

        for (int i = 0; i < NumberOfTrees; i++)
        {
            Vector2 randomPosition = new Vector2(
                (float)GD.RandRange(SpawnArea.Position.X, SpawnArea.Position.X + SpawnArea.Size.X),
                (float)GD.RandRange(SpawnArea.Position.Y, SpawnArea.Position.Y + SpawnArea.Size.Y)
            );

            Node2D treeInstance = TreeScene.Instantiate<Node2D>();
            treeInstance.Position = randomPosition;
            AddChild(treeInstance);
        }
    }
}
