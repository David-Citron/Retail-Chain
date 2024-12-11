using System.Collections.Generic;

public class CraftingRecipe
{

    public readonly MachineType machineType; //Required machine

    public readonly List<ItemType> inputs; //Inputs
    public readonly ItemType output; //Output - the result

    public readonly int time; //Time elapsed before the result is achieved

    public CraftingRecipe(MachineType machineType, List<ItemType> inputs, ItemType output, int time)
    {
        this.machineType = machineType;
        this.inputs = inputs;
        this.output = output;
        this.time = time;
    }
}