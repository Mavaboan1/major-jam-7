using Godot;
using System;
using System.Collections.Generic;

public partial class TreeSpawner : Node2D
{
    [Export] public PackedScene TreeScene;
    [Export] public PackedScene RockScene;

    [Export] public int NumberOfTrees = 2000;
    [Export] public Rect2 SpawnArea = new Rect2(-5000, -5000, 10000, 10000); // example area

    public override void _Ready()
    {
        SpawnTrees();
    }

   public void SpawnTrees()
{
    if (TreeScene == null || RockScene == null)
    {
        GD.PrintErr("TreeScene or RockScene is not assigned!");
        return;
    }

    const float MinDistance = 100f;
    const int MaxAttempts = 10;

    // Load all rock sprites once
    string rockSpriteFolder = "res://Sprites/Rocks/";
    List<Texture2D> rockTextures = new List<Texture2D>();
    DirAccess dir = DirAccess.Open(rockSpriteFolder);
    if (dir != null)
    {
        dir.ListDirBegin();
        string file;
        while ((file = dir.GetNext()) != "")
        {
            if (!dir.CurrentIsDir() && file.EndsWith(".png"))
            {
                Texture2D tex = GD.Load<Texture2D>(rockSpriteFolder + file);
                if (tex != null)
                    rockTextures.Add(tex);
            }
        }
        dir.ListDirEnd();
    }
    else
    {
        GD.PrintErr("Failed to open rock sprite folder.");
    }

    // Spawn Trees
    for (int i = 0; i < NumberOfTrees; i++)
    {
        Vector2 randomPosition;
        int attempts = 0;

        do
        {
            randomPosition = new Vector2(
                (float)GD.RandRange(SpawnArea.Position.X, SpawnArea.Position.X + SpawnArea.Size.X),
                (float)GD.RandRange(SpawnArea.Position.Y, SpawnArea.Position.Y + SpawnArea.Size.Y)
            );
            attempts++;
        } while (randomPosition.Length() < MinDistance && attempts < MaxAttempts);

        if (randomPosition.Length() < MinDistance)
            continue;

        Node2D treeInstance = TreeScene.Instantiate<Node2D>();
        treeInstance.Position = randomPosition;
        AddChild(treeInstance);
    }

    // Spawn Rocks with random sprite
    for (int i = 0; i < NumberOfTrees+4000; i++)
    {
        Vector2 randomPosition;
        int attempts = 0;

        do
        {
            randomPosition = new Vector2(
                (float)GD.RandRange(SpawnArea.Position.X, SpawnArea.Position.X + SpawnArea.Size.X),
                (float)GD.RandRange(SpawnArea.Position.Y, SpawnArea.Position.Y + SpawnArea.Size.Y)
            );
            attempts++;
        } while (randomPosition.Length() < MinDistance && attempts < MaxAttempts);

        if (randomPosition.Length() < MinDistance)
            continue;

        Node2D rockInstance = RockScene.Instantiate<Node2D>();
        rockInstance.Position = randomPosition;

        // Assign a random texture
        if (rockTextures.Count > 0)
{
    int randomIndex = (int)(GD.Randi() % (uint)rockTextures.Count);
    Texture2D chosenTexture = rockTextures[randomIndex];
    Sprite2D spriteNode = rockInstance.GetNodeOrNull<Sprite2D>("Sprite2D");
    if (spriteNode != null)
        spriteNode.Texture = chosenTexture;
    else
        GD.PrintErr("RockScene does not contain a Sprite2D named 'Sprite2D'");
}

        AddChild(rockInstance);
    }
}

}
