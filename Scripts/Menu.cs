using Godot;
using System;

public partial class Menu : Control
{
    public void _on_play_pressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
    }
    public void _on_endless_pressed()
    {

    }
    public void _on_options_pressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/OptionsMenu.tscn");

    }
    public void _on_exit_pressed()
    {
        GetTree().Quit();
    }
}
