using System;
using System.Collections.Generic;
using System.Linq;

public static class TileTypeUtils
{
    public static readonly Dictionary<string, string> TileTypeToString = new Dictionary<string, string>
    {
        { "forest", "Forest" },
        { "lake", "Lake" },
        { "mountain", "Mountain" },
        { "plain", "Plain" },
        { "forestForest", "Thicket" },
        { "forestLake", "Marsh" },
        { "forestMountain", "Pine Hill" },
        { "forestPlain", "Glade" },
        { "lakeLake", "Basin" },
        { "lakeMountain", "Caldera" },
        { "lakePlain", "Bog" },
        { "mountainMountain", "Summit" },
        { "mountainPlain", "Highland" },
        { "plainPlain", "Cropfield" }
    };

    public static string GetRandomComboFromBaseBiome(string baseBiome)
    {
        if (string.IsNullOrEmpty(baseBiome))
        {
            throw new ArgumentException("baseBiome cannot be null or empty.");
        }

        var allTypes = Enum.GetNames(typeof(TileType));

        var combos = allTypes
            .Where(t => t.Length > baseBiome.Length && t.IndexOf(baseBiome, StringComparison.OrdinalIgnoreCase) >= 0)
            .ToList();

        if (combos.Count == 0)
        {
            throw new InvalidOperationException($"No combo biome found for base biome '{baseBiome}'.");
        }

        int idx = UnityEngine.Random.Range(0, combos.Count);
        return combos[idx];
    }
}