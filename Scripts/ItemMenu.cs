using Godot;
using System;
using System.Collections.Generic;

public partial class ItemMenu : Control
{
    [Export] public Button Item1;
    [Export] public Button Item2;
    [Export] public Button Item3;
    [Export] public RichTextLabel DescriptionLabel;
    private Dictionary<string, string> itemDescriptions = new Dictionary<string, string>()
{
    { "Garlic", "Small Damaging Aura Around you. Leveling up increases Range" },
    { "hover_shield", "A shield that spins and blocks attacks. Leveling Up gives more Shields" },
    { "icicle", "Icicles that spin around you and damage enemies. Leveling up gives more icicles." },
    { "boots", "Gives a speed increas. Leveling up gives more speed." },
    { "bouncy_spark", "A Spark that bounces around the screen damaing enemies.Levling up increases Size and Speed." },
    { "magic_missile", "Fire homing Missiles at enemies. Leveling up increases Accuracy" },
    { "meteor_shower", "Fires loads of Random projectiles. Leveling up increases FireRate" },
    { "recovery_belt", "Stand still to regenerate health. Leveling up increases health regen" },
    { "shockwave", "fire a wave in the faced direction, knocking back enemies. Leveling up increases firerate" },
    { "tornadoes", "summon tornadoes that randomly move around the screen. Leveling up increases Firerate" },



};

    public override void _Ready()
    {
        Item1.Pressed += () => OnButtonPressed(Item1);
        Item2.Pressed += () => OnButtonPressed(Item2);
        Item3.Pressed += () => OnButtonPressed(Item3);
        Disabled();
        Item1.MouseEntered += () => OnButtonHover(Item1);
        Item2.MouseEntered += () => OnButtonHover(Item2);
        Item3.MouseEntered += () => OnButtonHover(Item3);
        Disabled();

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
                if (level >= 5f)
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

        string folderPath = "res://Scenes/Item/";
        DirAccess dir = DirAccess.Open(folderPath);
        if (dir == null)
        {
            GD.PrintErr("Folder not found: " + folderPath);
            return;
        }

        List<string> itemNames = new();

        dir.ListDirBegin();
        string file;
        while ((file = dir.GetNext()) != "")
        {
            if (!dir.CurrentIsDir() && file.EndsWith(".tscn"))
            {
                string itemName = file.GetBaseName();
                if (!excludeNames.Contains(itemName))
                {
                    itemNames.Add(itemName);
                }
            }
        }
        dir.ListDirEnd();

        for (int i = itemNames.Count - 1; i > 0; i--)
        {
            int j = (int)(GD.Randi() % (uint)(i + 1));
            (itemNames[i], itemNames[j]) = (itemNames[j], itemNames[i]);
        }

        if (itemNames.Count >= 3)
        {
            Item1.Text = itemNames[0];
            Item2.Text = itemNames[1];
            Item3.Text = itemNames[2];
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
    }

    public void Disabled()
    {
        Visible = false;
        SetProcess(false);
        SetProcessInput(false);
        SetProcessUnhandledInput(false);
        SetProcessUnhandledKeyInput(false);
        GetTree().Paused = false;
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
            ChooseItem(pressedButton.Text);
    }
    private void OnButtonHover(Button hoveredButton)
    {
        if (hoveredButton == null) return;

        string itemName = hoveredButton.Text;
        string desc = GetItemDescription(itemName);
        DescriptionLabel.Text = desc;
    }
     private string GetItemDescription(string itemName)
    {
        if (itemDescriptions.TryGetValue(itemName, out var desc))
        return desc;
    return "No description available.";
    }
}