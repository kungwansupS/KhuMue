using System.Reflection;

namespace KhuMue.Core;

/// <summary>How many of an upgrade the current character has used. Null = could not be read.</summary>
internal readonly record struct UpgradeState(int? Used, int Max)
{
    public bool Complete => Used >= Max;
}

/// <summary>
/// Reads permanent upgrades. Vanilla values come from <see cref="Player"/> (tModLoader 1.4.4.9);
/// Calamity values from its public "GetPermanentPowerup" call (2.2.4, ModCalls.HasPermanentPowerup);
/// Calamity Hunt Auric Souls from AuricSoulPlayer fields (1.2.3).
/// </summary>
internal static class UpgradeService
{
    private static readonly Dictionary<string, Func<Player, int?>> Readers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["LifeCrystal"] = p => p.ConsumedLifeCrystals,
        ["LifeFruit"] = p => p.ConsumedLifeFruit,
        ["ManaCrystal"] = p => p.ConsumedManaCrystals,
        ["DemonHeart"] = p => p.extraAccessory ? 1 : 0,
        ["AegisCrystal"] = p => p.usedAegisCrystal ? 1 : 0,
        ["AegisFruit"] = p => p.usedAegisFruit ? 1 : 0,
        ["ArcaneCrystal"] = p => p.usedArcaneCrystal ? 1 : 0,
        ["GalaxyPearl"] = p => p.usedGalaxyPearl ? 1 : 0,
        ["GummyWorm"] = p => p.usedGummyWorm ? 1 : 0,
        ["Ambrosia"] = p => p.usedAmbrosia ? 1 : 0,
        ["ArtisanLoaf"] = p => p.ateArtisanBread ? 1 : 0,
        ["TorchGodsFavor"] = p => p.unlockedBiomeTorches ? 1 : 0,
        ["MinecartUpgradeKit"] = p => p.unlockedSuperCart ? 1 : 0,
        // World upgrade, shared by everyone in the world.
        ["PeddlersSatchel"] = _ => NPC.peddlersSatchelWasUsed ? 1 : 0,
    };

    /// <summary>Upgrade ids Calamity's "GetPermanentPowerup" call understands (2.2.4).</summary>
    private static readonly HashSet<string> CalamityPowerups = new(StringComparer.OrdinalIgnoreCase)
    {
        "SanguineTangerine", "MiracleFruit", "TaintedCloudberry", "SacredStrawberry",
        "CometShard", "EtherealCore", "PhantomHeart", "CelestialOnion",
        "MushroomPlasmaRoot", "InfernalBlood", "RedLightningContainer",
        "ElectrolyteGelPack", "StarlightFuelCell", "Ectoheart",
    };

    /// <summary>Guide upgrade id to CalamityHunt AuricSoulPlayer bool field (1.2.3).</summary>
    private static readonly (string Id, string Field)[] HuntSouls =
    {
        ("GoozmaSoul", "goozmaSoul"),
        ("YharonSoul", "yharonSoul"),
        ("RottenSoul", "olddukeSoul"),
    };

    private static bool _huntResolved;
    private static ModPlayer? _huntTemplate;
    private static readonly Dictionary<string, FieldInfo> HuntFieldInfos = new(StringComparer.OrdinalIgnoreCase);

    internal static void Unload()
    {
        _huntResolved = false;
        _huntTemplate = null;
        HuntFieldInfos.Clear();
    }

    internal static UpgradeState Read(UpgradeDefinition definition, Player player)
    {
        if (Readers.TryGetValue(definition.Id, out Func<Player, int?>? reader))
            return new UpgradeState(reader(player), definition.Max);

        if (CalamityPowerups.Contains(definition.Id))
            return new UpgradeState(ReadCalamity(definition.Id, player), definition.Max);

        ResolveHunt();
        if (_huntTemplate is not null && HuntFieldInfos.TryGetValue(definition.Id, out FieldInfo? field))
        {
            try
            {
                return new UpgradeState(field.GetValue(player.GetModPlayer(_huntTemplate)) is true ? 1 : 0, definition.Max);
            }
            catch
            {
                return new UpgradeState(null, definition.Max);
            }
        }
        return new UpgradeState(null, definition.Max);
    }

    private static int? ReadCalamity(string id, Player player)
    {
        if (!ModLoader.TryGetMod(SupportedMods.Calamity, out Mod calamity))
            return null;
        try
        {
            if (calamity.Call("GetPermanentPowerup", player, id) is not bool used)
                return null;
            // In Master Mode the onion grants the Demon Heart slot instead, so Calamity treats an eaten
            // Demon Heart as "already consumed" (CelestialOnion.HasConsumedBefore).
            if (id == "CelestialOnion" && Main.masterMode && player.extraAccessory)
                used = true;
            return used ? 1 : 0;
        }
        catch
        {
            return null;
        }
    }

    private static void ResolveHunt()
    {
        if (_huntResolved)
            return;
        _huntResolved = true;
        if (!ModLoader.TryGetMod(SupportedMods.Hunt, out Mod hunt) || !hunt.TryFind("AuricSoulPlayer", out ModPlayer template))
            return;
        _huntTemplate = template;
        Type type = template.GetType();
        foreach ((string id, string fieldName) in HuntSouls)
        {
            FieldInfo? field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field is not null && field.FieldType == typeof(bool))
                HuntFieldInfos[id] = field;
            else
                KhuMue.Instance.Logger.Warn($"AuricSoulPlayer.{fieldName} not found; upgrade '{id}' will show as unknown.");
        }
    }
}
