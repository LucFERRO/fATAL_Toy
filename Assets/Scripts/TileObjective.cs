using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TileObjective
{
    public string Description { get; protected set; }
    public bool IsCompleted { get; protected set; }
    public int Target { get; protected set; }
    public string Biome { get; protected set; } 
    public int Progress { get; protected set; }

    public abstract void Evaluate(TileSplitManager manager);
    public virtual void ForceComplete()
    {
        Progress = Target;
        IsCompleted = true;
    }
    public virtual void Reset()
    {
        Progress = 0;
        IsCompleted = false;
    }
}

public class CreateBiomeObjective : TileObjective
{
    public CreateBiomeObjective(string biome, int target)
    {
        Biome = biome;
        Target = target;
        Description = $"Create TARGET BIOME";
    }

    public override void Evaluate(TileSplitManager manager)
    {
        Progress = manager.biomeTilesCreatedSinceReset(Biome);
        IsCompleted = Progress >= Target;
    }
}

public class CreateComboObjective : TileObjective
{
    public CreateComboObjective(string combo, int target)
    {
        Biome = combo;
        Target = target;
        Description = $"Create TARGET BIOME";
    }

    public override void Evaluate(TileSplitManager manager)
    {
        Progress = manager.comboTilesCreatedSinceReset(Biome);
        IsCompleted = Progress >= Target;
    }
}

public class ChangeTilesInOneRoll : TileObjective
{
    public ChangeTilesInOneRoll(int target)
    {
        Biome = "";
        Target = target;
        Description = $"Modify directly TARGET or more tiles with the same roll";
    }

    public override void Evaluate(TileSplitManager manager)
    {
        //var rollCounts = manager.GetRollCounts();
        //IsCompleted = Progress >= Target;
        int currentMaxRoll = manager.WhatIsMaxRoll();
        Progress = currentMaxRoll;
        //Progress = 99;
        IsCompleted = currentMaxRoll >= Target;
    }
}
public class ChangeLessTilesInOneRoll : TileObjective
{
    public ChangeLessTilesInOneRoll(int target)
    {
        Biome = "";
        Target = target;
        Description = $"Modify directly TARGET or less tiles with the same roll";
    }

    public override void Evaluate(TileSplitManager manager)
    {
        //var rollCounts = manager.GetRollCounts();
        //IsCompleted = Progress >= Target;
        int currentMinRoll = manager.WhatIsMinRoll();
        Progress = currentMinRoll;
        //Progress = 99;
        IsCompleted = currentMinRoll <= Target;
    }
}

public class AllFacesSameBiomeObjective : TileObjective
{
    public AllFacesSameBiomeObjective(string biome)
    {
        Biome = biome;
        Target = 1;
        Description = $"Roll a dice with all faces containing BIOME elements";
    }

    public override void Evaluate(TileSplitManager manager)
    {
        Progress = manager.IsDiceMadeOfOneBiome(Biome) ? 1 : 0;
        //IsCompleted = Progress >= Target;
        //Progress = 99;
        IsCompleted = Progress == 1;
    }
}

public class ChainObjective : TileObjective
{
    public ChainObjective(string biome, int target)
    {
        Biome = biome;
        Target = target;
        Description = $"Create a chain of TARGET BIOME tiles";
    }

    public override void Evaluate(TileSplitManager manager)
    {
        //Progress = manager.GetLongestChainOfBiome(biome);
        //IsCompleted = Progress >= Target;
    }
}

public static class ObjectiveFactory
{
    public static TileObjective GenerateRandomObjective(int objectiveType, TileSplitManager manager, string forcedBiome = null)
    {
        switch (objectiveType)
        {
            case 0:
                {
                    string biome = manager.gridTileSplitDictionary.Keys.ElementAt(UnityEngine.Random.Range(1, manager.gridTileSplitDictionary.Count));
                    return new CreateBiomeObjective(biome, UnityEngine.Random.Range(5, 15));
                }
            case 1:
                {
                    string combo = forcedBiome != null ? TileTypeUtils.GetRandomComboFromBaseBiome(forcedBiome) : manager.comboTileSplitDictionary.Keys.ElementAt(UnityEngine.Random.Range(0, manager.comboTileSplitDictionary.Count));
                    return new CreateComboObjective(combo, UnityEngine.Random.Range(2, 6));
                }
            case 2:
                return new ChangeTilesInOneRoll(UnityEngine.Random.Range(4, 10));
            case 3:
                return new ChangeLessTilesInOneRoll(UnityEngine.Random.Range(2, 4));
            case 4:
                {
                    string allFaceBiome = forcedBiome ?? manager.gridTileSplitDictionary.Keys.ElementAt(UnityEngine.Random.Range(1, manager.gridTileSplitDictionary.Count));
                    return new AllFacesSameBiomeObjective(allFaceBiome);
                }
            //case 5:
            //    string chainBiome = manager.gridTileSplitDictionary.Keys.ElementAt(UnityEngine.Random.Range(0, manager.gridTileSplitDictionary.Count));
            //    return new ChainObjective((TileType)System.Enum.Parse(typeof(TileType), chainBiome), UnityEngine.Random.Range(3, 8));
            default:
                return null;
        }
    }
}