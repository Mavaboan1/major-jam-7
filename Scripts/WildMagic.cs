using Godot;
using System;

public partial class WildMagic : Node2D
{
    [Export] public ProgressBar WildMagicBar;
    public Node wildSingleton;

    public override void _Ready()
    {
        WildMagicBar.MaxValue = 300f;
        WildMagicBar.Value = 0;
        wildSingleton = GetNode<Node>("/root/WildMagicSingle");
    }

    public override void _Process(double delta)
    {
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
        int Randomnr = (int)GD.Randi() % 9 + 1;
        if (Randomnr == 2)
        {
            invert();
        }
        else if (Randomnr == 3)
        {
            Slow();
        }
        else if (Randomnr == 4)
        {
            Zoom();
        }
        else if (Randomnr == 5)
        {
            Flip();
        }
        else if (Randomnr == 6)
        {
            Darkness();
        }
        else if (Randomnr == 7)
        {
            SpeedTime();
        }
        else if (Randomnr == 8)
        {
            wildSingleton.Call("monster_speed");
        }
        else if (Randomnr == 9)
        {
            wildSingleton.Call("monster_size");
        }
        else if (Randomnr == 1)
        {
            wildSingleton.Call("exp_explosion");
        }
    }
    public void invert()
    {
        Node player = GetTree().Root.GetNode("Main/Character");
        player.Call("Invert");
        Timer timer = new Timer();
        timer.WaitTime = 15f;
        timer.OneShot = true;
        AddChild(timer);

        timer.Timeout += () =>
        {
            player.Call("Invert");
            timer.QueueFree();
        };

        timer.Start();
    }
    public void Slow()
    {
        Node player = GetTree().Root.GetNode("Main/Character");
        player.Call("Slow");
        Timer timer = new Timer();
        timer.WaitTime = 15f;
        timer.OneShot = true;
        AddChild(timer);

        timer.Timeout += () =>
        {
            player.Call("Slow");
            timer.QueueFree();
        };
        timer.Start();

    }
    public void Zoom()
    {
        Camera2D camera = GetTree().Root.GetNode<Camera2D>("Main/Character/Camera");
        camera.Zoom = new Vector2(2f, 2f);
        Timer timer = new Timer();
        timer.WaitTime = 15f;
        timer.OneShot = true;
        AddChild(timer);

        timer.Timeout += () =>
        {
            camera.Zoom = new Vector2(1f, 1f);
            timer.QueueFree();
        };
        timer.Start();
    }
    public void Flip()
    {
        Node2D main = GetTree().Root.GetNode<Node2D>("Main");
        main.Rotation = MathF.PI;
        Timer timer = new Timer();
        timer.WaitTime = 15f;
        timer.OneShot = true;
        AddChild(timer);

        timer.Timeout += () =>
        {
            main.Rotation = MathF.PI * 2;
            timer.QueueFree();
        };
        timer.Start();

    }
    public void Darkness()
    {
        ColorRect DarknessOverlay = GetTree().Root.GetNode<ColorRect>("Main/Character/Camera/Darkness");
        DarknessOverlay.Visible = true;
        Timer timer = new Timer();
        timer.WaitTime = 1f;
        timer.OneShot = true;
        AddChild(timer);

        timer.Timeout += () =>
        {
            DarknessOverlay.Visible = false;
            timer.QueueFree();
        };
        timer.Start();
    }
    public void SpeedTime()
    {
        Engine.TimeScale = 2f;
        Timer timer = new Timer();
        timer.WaitTime = 15f;
        timer.OneShot = true;
        AddChild(timer);

        timer.Timeout += () =>
        {
            Engine.TimeScale = 1f;
            timer.QueueFree();
        };
        timer.Start();
    }
}
