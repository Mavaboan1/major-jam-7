using Godot;
using System;
using System.Collections.Generic;

public partial class ItemMenu : Control
{
    [Export] public Button Item1;
    [Export] public Button Item2;
    [Export] public Button Item3;
    [Export] public Button Visbility;
    [Export] public RichTextLabel DescriptionLabel;
    [Export] public Sprite2D ItemPreviewSprite;
    [Export] public AudioStreamPlayer2D audio;
    private bool isTransparent = false;
    private Camera2D playerCamera;
    private Vector2 originalZoom = Vector2.One;
    private bool zoomedOut = false;

    private Dictionary<string, string> itemDescriptions = new Dictionary<string, string>()
    {
        { "spell_circle", "Small Damaging Aura Around you. Leveling up increases Range" },
        { "hover_shield", "A shield that spins and blocks attacks. Leveling Up gives more Shields" },
        { "icicle", "Icicles that spin around you and damage enemies. Leveling up gives more icicles." },
        { "boots", "Gives a speed increase. Leveling up gives more speed." },
        { "bouncy_spark", "A Spark that bounces around the screen damaing enemies.Levling up increases Size and Speed." },
        { "magic_missile", "Fire homing Missiles at enemies. Leveling up increases Accuracy" },
        { "meteor_shower", "Fires loads of Random projectiles. Leveling up increases FireRate" },
        { "recovery_belt", "Stand still to regenerate health. Leveling up increases health regen" },
        { "shockwave", "fire a wave in the faced direction, knocking back enemies. Leveling up increases firerate" },
        { "tornadoes", "summon tornadoes that randomly move around the screen. Leveling up increases Firerate" },
        { "vacuum", "Increases Pickup Range. Leveling up increases Range" },
        { "mage_armor", "Increases Max Health. Leveling up increases Max Health" },
    };

    public override void _Ready()
    {
        Visbility.Pressed += OnToggleTransparencyPressed;
        Item1.Pressed += () => OnButtonPressed(Item1);
        Item2.Pressed += () => OnButtonPressed(Item2);
        Item3.Pressed += () => OnButtonPressed(Item3);
        Item1.MouseEntered += () => OnButtonHover(Item1);
        Item2.MouseEntered += () => OnButtonHover(Item2);
        Item3.MouseEntered += () => OnButtonHover(Item3);
        Disabled();

        var character = GetTree().Root.GetNodeOrNull("Main/Character");
        playerCamera = character?.GetNodeOrNull<Camera2D>("Camera");
    }

    public void RollItems()
    {
        List<string> excludeNames = new();
        Node itemController = GetTree().Root.GetNodeOrNull("Main/Character/ItemController");
        foreach (Node item in itemController.GetChildren())
        {
            var levelVariant = item.Get("Level");
            try
            {
                float level = levelVariant.As<float>();
                if (level == 5f)
                {
                    string itemName = item.Name;
                    excludeNames.Add(itemName);
                }
            }
            catch
            {
                GD.Print($"Item {item.Name} does not have a valid float Level property");
            }
        }

        // ✅ Hardcoded list of item names
        List<string> itemNames = new()
        {
            "spell_circle",
            "hover_shield",
            "icicle",
            "boots",
            "bouncy_spark",
            "magic_missile",
            "meteor_shower",
            "recovery_belt",
            "shockwave",
            "tornadoes",
            "vacuum",
            "mage_armor"
        };

        itemNames.RemoveAll(name => excludeNames.Contains(name));

        // Shuffle
        for (int i = itemNames.Count - 1; i > 0; i--)
        {
            int j = (int)(GD.Randi() % (uint)(i + 1));
            (itemNames[i], itemNames[j]) = (itemNames[j], itemNames[i]);
        }

        if (itemNames.Count >= 3)
        {
            Item1.Text = itemNames[0].Replace('_',' ');
            Item2.Text = itemNames[1].Replace('_',' ');
            Item3.Text = itemNames[2].Replace('_',' ');
        }
        else
        {
            GD.PrintErr("Not enough items to choose from after filtering level 5.");
        }
    }

    public void Active()
    {
        Visible = true;
        SetProcess(true);
        SetProcessInput(true);
        SetProcessUnhandledInput(true);
        SetProcessUnhandledKeyInput(true);
        RollItems();
        GetTree().Paused = true;
        audio.Play();
        if (playerCamera.Zoom == new Vector2(2f,2f))
        {
            playerCamera.Zoom = new Vector2(1f, 1f);
            zoomedOut = true;
        }
    }

    public void Disabled()
    {
        Visible = false;
        SetProcess(false);
        SetProcessInput(false);
        SetProcessUnhandledInput(false);
        SetProcessUnhandledKeyInput(false);
        GetTree().Paused = false;
        if (zoomedOut)
        {
            playerCamera.Zoom = new Vector2(2f,2f);
            zoomedOut = false;
        }
    }

    public void ChooseItem(string itemName)
    {
        Node player = GetTree().Root.GetNode("Main/Character");
        if (player != null)
        {
            player.Call("PickUpItem", itemName);
            Disabled();
        }
        else
        {
            GD.PrintErr("Player node not found!");
        }
    }

    private void OnButtonPressed(Button pressedButton)
    {
        if (pressedButton != null)
            ChooseItem(pressedButton.Text.Replace(' ','_'));
    }

    private void OnButtonHover(Button hoveredButton)
    {
        if (hoveredButton == null) return;

        string itemName = hoveredButton.Text.Replace(' ','_');
        string desc = GetItemDescription(itemName);
        DescriptionLabel.Text = desc;

        string imagePath = $"res://Sprites/Items/Used/{itemName}.png";

        if (ResourceLoader.Exists(imagePath))
        {
            Texture2D texture = GD.Load<Texture2D>(imagePath);
            if (texture != null && ItemPreviewSprite != null)
            {
                ItemPreviewSprite.Texture = texture;
                ItemPreviewSprite.Scale = new Vector2(1f, 1f);
            }
        }
        else
        {
            GD.PrintErr($"Image not found: {imagePath}");
        }
    }

    private string GetItemDescription(string itemName)
    {
        if (itemDescriptions.TryGetValue(itemName, out var desc))
            return desc;
        return "No description available.";
    }

    private void OnToggleTransparencyPressed()
    {
        isTransparent = !isTransparent;

        Modulate = isTransparent 
            ? new Color(1, 1, 1, 0.3f) 
            : new Color(1, 1, 1, 1f);  
    }
}
