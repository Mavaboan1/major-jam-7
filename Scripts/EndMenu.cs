using Godot;
using System;

public partial class EndMenu : Control
{
    [Export] public Button back;

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
}
