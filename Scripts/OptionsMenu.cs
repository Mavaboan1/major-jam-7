using Godot;
using System;

public partial class OptionsMenu : Control
{
    public void _on_back_pressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/menu.tscn");

    }
}
