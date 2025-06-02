using Godot;
using System;

public partial class WildMagic : Node2D
{
    [Export] public ProgressBar WildMagicBar;

    public override void _Ready()
    {
        WildMagicBar.MaxValue = 100f;
        WildMagicBar.Value = 0;
    }

    public override void _Process(double delta)
    {
        GetWild(1);
        if (WildMagicBar.Value == WildMagicBar.MaxValue)
        {
            TriggerWildMagic();
            WildMagicBar.Value = 0;
        }
    }
    public void GetWild(int wild)
    {
        WildMagicBar.Value += wild;
    }
    public void TriggerWildMagic()
    {
        GD.Randomize();
        int Randomnr = (int)GD.Randi() % 10 + 1;
        if (Randomnr == 1)
        {

        }
        else if (Randomnr == 2)
        {
            
        }
    }
}
