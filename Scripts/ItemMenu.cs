using Godot;
using System;
using System.Collections.Generic;


public partial class ItemMenu : Control
{

    [Export] public Button Item1;
    [Export] public Button Item2;
    [Export] public Button Item3;

    public override void _Ready()
    {
        Item1.Pressed += () => OnButtonPressed(Item1);
    Item2.Pressed += () => OnButtonPressed(Item2);
    Item3.Pressed += () => OnButtonPressed(Item3);
        Disabled();
    }
    public void RollItems()
    {
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
                itemNames.Add(file.GetBaseName());
        }
        dir.ListDirEnd();

        for (int i = itemNames.Count - 1; i > 0; i--)
        {
            int j = (int)(GD.Randi() % (uint)(i + 1));
            (itemNames[i], itemNames[j]) = (itemNames[j], itemNames[i]);
        }

        Item1.Text = itemNames[0];
        Item2.Text = itemNames[1];
        Item3.Text = itemNames[2];





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
            Disabled(); // Hide menu after choosing
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
}
