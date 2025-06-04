using Godot;
using System;

public partial class OptionsMenu : Control
{
    public void _on_back_pressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/menu.tscn");

    }
    public void _on_volume_value_changed(double Value)
    {
        float db = Mathf.LinearToDb((float)Value);
        db = Mathf.Clamp(db, -80, 0);

        AudioServer.SetBusVolumeDb(0, db);
    }
}
