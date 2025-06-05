using Godot;
using System;

public partial class EndMenu : Control
{
    [Export] public Button back;
    [Export] public Button Resume;


    public override void _Ready()
    {
        back.Pressed += () => OnButtonPressed(back);

    }
    public void Activate()
    {
        Visible = true;
        SetProcess(true);
        SetProcessInput(true);
        SetProcessUnhandledInput(true);
        SetProcessUnhandledKeyInput(true);
        GetTree().Paused = true;

    }
    private void OnButtonPressed(Button pressedButton)
    {
        Visible = false;
        SetProcess(false);
        SetProcessInput(false);
        SetProcessUnhandledInput(false);
        SetProcessUnhandledKeyInput(false);
        GetTree().Paused = false;

        GetTree().ChangeSceneToFile("res://Scenes/menu.tscn");
    }
    public void _on_button_2_pressed()
    {
        Visible = false;
        SetProcess(false);
        SetProcessInput(false);
        SetProcessUnhandledInput(false);
        SetProcessUnhandledKeyInput(false);
        GetTree().Paused = false;
    }
    public void _on_volume_value_changed(double Value)
    {
        float db = Mathf.LinearToDb((float)Value);
        db = Mathf.Clamp(db, -80, 0);

        AudioServer.SetBusVolumeDb(0, db);
    }
}
