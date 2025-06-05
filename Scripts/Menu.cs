using Godot;
using System;

public partial class Menu : Control
{
    [Export] public Node signal_bus;

    

    public void _on_play_pressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
    }
    public void _on_endless_pressed()
    {
        signal_bus = GetTree().Root.GetNode("WildMagicSingle");
        signal_bus.Call("signal_bus");
        GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
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
