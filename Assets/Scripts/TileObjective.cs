using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TileObjective
{
    public string Description { get; protected set; }
    public bool IsCompleted { get; protected set; }
    public int Target { get; protected set; }
    public int Progress { get; protected set; }

    public abstract void Evaluate(TileSplitManager manager);

    public virtual string GetProgressString() => $"{Progress}/{Target}";
}

public class CreateBiomeObjective : TileObjective
{
    private string biome;

    public CreateBiomeObjective(string biome, int target)
    {
        this.biome = biome;
        Target = target;
        Description = $"Create {Target} {biome}s";
    }

    public override void Evaluate(TileSplitManager manager)
    {
        Progress = manager.gridTileSplitDictionary.TryGetValue(biome, out int count) ? count : 0;
        IsCompleted = Progress >= Target;
    }
}

public class CreateComboObjective : TileObjective
{
    private string combo;

    public CreateComboObjective(string combo, int target)
    {
        this.combo = combo;
        Target = target;
        string marshException = combo == "Marshe" ? "Marsh" : combo;
        Description = $"Create {Target} {marshException}{(target > 1 ? "s" : "")}";
    }

    public override void Evaluate(TileSplitManager manager)
    {
        Progress = manager.comboTileSplitDictionary.TryGetValue(combo, out int count) ? count : 0;
        IsCompleted = Progress >= Target;
    }
}

public class ChangeTilesInOneRoll : TileObjective
{
    public ChangeTilesInOneRoll(int target)
    {
        Target = target;
        Description = $"Roll over {Target} or more tiles with the same roll";
    }

    public override void Evaluate(TileSplitManager manager)
    {
        // Example: You need to implement GetRollCounts() in TileSplitManager
        //var rollCounts = manager.GetRollCounts();
        //Progress = rollCounts.Values.Max();
        //IsCompleted = Progress >= Target;
    }
}
public class ChangeLessTilesInOneRoll : TileObjective
{
    public ChangeLessTilesInOneRoll(int target)
    {
        Target = target;
        Description = $"Roll over less than {Target} tiles with the same roll";
    }

    public override void Evaluate(TileSplitManager manager)
    {
        // Example: You need to implement GetRollCounts() in TileSplitManager
        //var rollCounts = manager.GetRollCounts();
        //Progress = rollCounts.Values.Max();
        //IsCompleted = Progress >= Target;
    }
}

public class AllFacesSameBiomeObjective : TileObjective
{
    private string biome;

    public AllFacesSameBiomeObjective(string biome)
    {
        this.biome = biome;
        Target = 1;
        Description = $"Roll a dice with all faces with {biome} elements";
    }

    public override void Evaluate(TileSplitManager manager)
    {
        // Example: You need to implement HasDiceWithAllFacesBiome in TileSplitManager
        //Progress = manager.HasDiceWithAllFacesBiome(biome) ? 1 : 0;
        //IsCompleted = Progress >= Target;
    }
}

public class ChainObjective : TileObjective
{
    private string biome;

    public ChainObjective(string biome, int target)
    {
        this.biome = biome;
        Target = target;
        Description = $"Create a chain of {Target} {biome} tiles";
    }

    public override void Evaluate(TileSplitManager manager)
    {
        // Example: You need to implement GetLongestChainOfBiome in TileSplitManager
        //Progress = manager.GetLongestChainOfBiome(biome);
        //IsCompleted = Progress >= Target;
    }
}

public static class ObjectiveFactory
{
    static Dictionary<string, string> tileTypeToString = new Dictionary<string, string>
    {
        { "forest", "Forest" },
        { "lake", "Lake" },
        { "mountain", "Mountain" },
        { "plain", "Plain" },
        { "forestForest", "Thicket" },
        { "forestLake", "Marshe" },
        { "forestMountain", "Highland" },
        { "forestPlain", "Glade" },
        { "lakeLake", "Basin" },
        { "lakeMountain", "Caldera" },
        { "lakePlain", "Bog" },
        { "mountainMountain", "Summit" },
        { "mountainPlain", "Plateau" },
        { "plainPlain", "Cropfield" }
    };

    public static TileObjective GenerateRandomObjective(TileSplitManager manager)
    {
        int type = UnityEngine.Random.Range(0, 5);
        switch (type)
        {
            case 0:
                string biome = manager.gridTileSplitDictionary.Keys.ElementAt(UnityEngine.Random.Range(0, manager.gridTileSplitDictionary.Count));
                return new CreateBiomeObjective(tileTypeToString[biome], UnityEngine.Random.Range(5, 15));
            case 1:
                string combo = manager.comboTileSplitDictionary.Keys.ElementAt(UnityEngine.Random.Range(0, manager.comboTileSplitDictionary.Count));
                return new CreateComboObjective(tileTypeToString[combo], UnityEngine.Random.Range(2, 6));
            case 2:
                return new ChangeTilesInOneRoll(UnityEngine.Random.Range(4, 10));            
            case 3:
                return new ChangeLessTilesInOneRoll(UnityEngine.Random.Range(2, 4));
            case 4:
                string allFaceBiome = manager.gridTileSplitDictionary.Keys.ElementAt(UnityEngine.Random.Range(0, manager.gridTileSplitDictionary.Count));
                return new AllFacesSameBiomeObjective(tileTypeToString[allFaceBiome]);
            //case 5:
            //    string chainBiome = manager.gridTileSplitDictionary.Keys.ElementAt(UnityEngine.Random.Range(0, manager.gridTileSplitDictionary.Count));
            //    return new ChainObjective((TileType)System.Enum.Parse(typeof(TileType), chainBiome), UnityEngine.Random.Range(3, 8));
            default:
                return null;
        }
    }
    static string ConvertToString(TileType tileType)
    {
        return tileType.ToString();
    }
}