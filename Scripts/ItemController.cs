using Godot;
using System;
using System.Collections.Generic;

public partial class ItemController : Node2D
{
private Dictionary<string, Node> itemInstances = new Dictionary<string, Node>();

    [Export] public string ItemScenePath = "res://Scenes/Item/";

    public void PickUpItem(string itemName)
    {

        if (itemInstances.ContainsKey(itemName))
        {
            Node item = itemInstances[itemName];
            item.Call("LevelUp");
        }
        else
        {
            string fullPath = ItemScenePath + itemName + ".tscn";
            PackedScene scene = ResourceLoader.Load<PackedScene>(fullPath);

            if (scene != null)
            {
                Node itemInstance = scene.Instantiate();
                itemInstance.Name = itemName;  
                AddChild(itemInstance);
                itemInstances[itemName] = itemInstance;
            }
            else
            {
                GD.PrintErr($"Item scene not found at: {fullPath}");
            }
        }
    }
}
