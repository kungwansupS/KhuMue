using System.Collections;
using System.Reflection;
using Terraria.GameContent.Events;

namespace KhuMue.Core;

/// <summary>
/// Named, read-only world/player facts used by the guide data ("v.Plantera", "cal.Yharon", ...).
/// A flag returns <c>null</c> when it cannot be known (mod missing, or its internals changed):
/// the guide then says "unknown" instead of guessing.
/// Every non-vanilla source below was checked against the decompiled mod build listed in
/// <see cref="SupportedMods"/>.
/// </summary>
internal static class Flags
{
    private static readonly Dictionary<string, Func<bool?>> Registry = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Flags read through Boss Checklist (any mod that registers with it), keyed "bc.Mod Name".</summary>
    private static Dictionary<string, Func<bool>>? _bossChecklistDowned;

    internal static IReadOnlyCollection<string> Keys => Registry.Keys;

    internal static bool? Get(string key)
    {
        if (Registry.TryGetValue(key, out Func<bool?>? getter))
        {
            try
            {
                return getter();
            }
            catch
            {
                return null;
            }
        }

        if (key.StartsWith("bc.", StringComparison.OrdinalIgnoreCase))
            return BossChecklistDowned(key[3..]);

        return null;
    }

    internal static bool Has(string key) => Registry.ContainsKey(key) || key.StartsWith("bc.", StringComparison.OrdinalIgnoreCase);

    internal static void Initialize()
    {
        Registry.Clear();
        _bossChecklistDowned = null;
        RegisterVanilla();
        // A changed mod must not stop the guide from loading; its flags then read as unknown.
        Guard(RegisterCalamity, "Calamity");
        Guard(RegisterAddons, "add-on");
    }

    private static void Guard(Action register, string what)
    {
        try
        {
            register();
        }
        catch (Exception e)
        {
            KhuMue.Instance.Logger.Warn($"Reading {what} progress flags failed; they will show as unknown.", e);
        }
    }

    internal static void Unload()
    {
        Registry.Clear();
        _bossChecklistDowned = null;
    }

    private static void Add(string key, Func<bool?> getter) => Registry[key] = getter;

    private static void Add(string key, Func<bool> getter) => Registry[key] = () => getter();

    // ------------------------------------------------------------------ vanilla

    private static void RegisterVanilla()
    {
        Add("always", () => true);
        Add("never", () => false);

        Add("v.KingSlime", () => NPC.downedSlimeKing);
        Add("v.EyeOfCthulhu", () => NPC.downedBoss1);
        Add("v.EvilBoss", () => NPC.downedBoss2);
        Add("v.QueenBee", () => NPC.downedQueenBee);
        Add("v.Skeletron", () => NPC.downedBoss3);
        Add("v.Deerclops", () => NPC.downedDeerclops);
        Add("v.Hardmode", () => Main.hardMode);
        Add("v.QueenSlime", () => NPC.downedQueenSlime);
        Add("v.Destroyer", () => NPC.downedMechBoss1);
        Add("v.Twins", () => NPC.downedMechBoss2);
        Add("v.SkeletronPrime", () => NPC.downedMechBoss3);
        Add("v.MechAny", () => NPC.downedMechBossAny);
        Add("v.MechTwo", () => MechCount() >= 2);
        Add("v.MechAll", () => MechCount() >= 3);
        Add("v.Plantera", () => NPC.downedPlantBoss);
        Add("v.Golem", () => NPC.downedGolemBoss);
        Add("v.DukeFishron", () => NPC.downedFishron);
        Add("v.EmpressOfLight", () => NPC.downedEmpressOfLight);
        Add("v.Cultist", () => NPC.downedAncientCultist);
        Add("v.PillarSolar", () => NPC.downedTowerSolar);
        Add("v.PillarVortex", () => NPC.downedTowerVortex);
        Add("v.PillarNebula", () => NPC.downedTowerNebula);
        Add("v.PillarStardust", () => NPC.downedTowerStardust);
        Add("v.Pillars", () => NPC.downedTowers);
        Add("v.MoonLord", () => NPC.downedMoonlord);

        Add("v.GoblinArmy", () => NPC.downedGoblins);
        Add("v.FrostLegion", () => NPC.downedFrost);
        Add("v.Pirates", () => NPC.downedPirates);
        Add("v.Martians", () => NPC.downedMartians);
        Add("v.MourningWood", () => NPC.downedHalloweenTree);
        Add("v.Pumpking", () => NPC.downedHalloweenKing);
        Add("v.Everscream", () => NPC.downedChristmasTree);
        Add("v.SantaNK1", () => NPC.downedChristmasSantank);
        Add("v.IceQueen", () => NPC.downedChristmasIceQueen);
        Add("v.OldOnesArmy1", () => DD2Event.DownedInvasionT1);
        Add("v.OldOnesArmy2", () => DD2Event.DownedInvasionT2);
        Add("v.OldOnesArmy3", () => DD2Event.DownedInvasionT3);
        Add("v.Clown", () => NPC.downedClown);

        // Torch God's Favor is a per-character unlock, not a world flag.
        Add("p.TorchGod", () => Main.gameMenu ? null : Main.LocalPlayer.unlockedBiomeTorches);

        Add("w.Crimson", () => WorldGen.crimson);
        Add("w.Corruption", () => !WorldGen.crimson);
        Add("w.Drunk", () => Main.drunkWorld);
        Add("w.Expert", () => Main.expertMode);
        Add("w.Master", () => Main.masterMode);
        Add("w.Journey", () => Main.GameModeInfo.IsJourneyMode);
        Add("w.Small", () => Main.maxTilesX <= 4200);
        Add("w.Multiplayer", () => Main.netMode == NetmodeID.MultiplayerClient);
        Add("w.BloodMoon", () => Main.bloodMoon);
        Add("w.Eclipse", () => Main.eclipse);
        Add("w.Night", () => !Main.dayTime);
    }

    private static int MechCount() => (NPC.downedMechBoss1 ? 1 : 0) + (NPC.downedMechBoss2 ? 1 : 0) + (NPC.downedMechBoss3 ? 1 : 0);

    // ------------------------------------------------------------------ Calamity

    /// <summary>Guide flag name to the name Calamity's public "GetBossDowned" call accepts (2.2.4).</summary>
    private static readonly (string Flag, string CallName)[] CalamityBosses =
    {
        ("DesertScourge", "desertscourge"),
        ("GiantClam", "giantclam"),
        ("GiantClamHardmode", "hmgiantclam"),
        ("AcidRain1", "acidrain1"),
        ("Crabulon", "crabulon"),
        ("HiveMind", "hivemind"),
        ("Perforators", "perforators"),
        ("SlimeGod", "slimegod"),
        ("Cryogen", "cryogen"),
        ("AcidRain2", "acidrain2"),
        ("AquaticScourge", "aquaticscourge"),
        ("CragmawMire", "cragmawmire"),
        ("BrimstoneElemental", "brimstoneelemental"),
        ("CalamitasClone", "calamitasclone"),
        ("GreatSandShark", "greatsandshark"),
        ("Leviathan", "leviathan"),
        ("AstrumAureus", "astrumaureus"),
        ("PlaguebringerGoliath", "plaguebringergoliath"),
        ("Ravager", "ravager"),
        ("AstrumDeus", "astrumdeus"),
        ("ProfanedGuardians", "profanedguardians"),
        ("Dragonfolly", "dragonfolly"),
        ("Providence", "providence"),
        ("CeaselessVoid", "ceaselessvoid"),
        ("StormWeaver", "stormweaver"),
        ("Signus", "signus"),
        ("Polterghast", "polterghast"),
        ("Mauler", "mauler"),
        ("NuclearTerror", "nuclearterror"),
        ("OldDuke", "oldduke"),
        ("DevourerOfGods", "devourerofgods"),
        ("Yharon", "yharon"),
        ("ExoMechs", "exomechs"),
        ("Thanatos", "thanatos"),
        ("Ares", "ares"),
        ("ExoTwins", "exotwins"),
        ("SupremeCalamitas", "supremecalamitas"),
        ("PrimordialWyrm", "primordialwyrm"),
        ("BossRush", "bossrush"),
    };

    private static void RegisterCalamity()
    {
        if (!ModLoader.TryGetMod(SupportedMods.Calamity, out Mod calamity))
            return;

        foreach ((string flag, string callName) in CalamityBosses)
        {
            string name = callName;
            Add("cal." + flag, () => calamity.Call("GetBossDowned", name) as bool?);
        }

        Add("cal.Sentinels", () => AllOf("cal.CeaselessVoid", "cal.StormWeaver", "cal.Signus"));
        Add("cal.FinalBosses", () => AllOf("cal.ExoMechs", "cal.SupremeCalamitas"));
        Add("cal.Revengeance", () => calamity.Call("GetDifficultyActive", "revengeance") as bool?);
        Add("cal.Death", () => calamity.Call("GetDifficultyActive", "death") as bool?);
        Add("cal.BossRushActive", () => calamity.Call("GetDifficultyActive", "bossrush") as bool?);

        // Not exposed through Mod.Call; public static field in CalamityMod.DownedBossSystem (2.2.4).
        Add("cal.Dreadnautilus", StaticMember(calamity, "CalamityMod.DownedBossSystem", "downedDreadnautilus"));
    }

    private static bool? AllOf(params string[] keys)
    {
        bool unknown = false;
        foreach (string key in keys)
        {
            bool? value = Get(key);
            if (value == false)
                return false;
            if (value is null)
                unknown = true;
        }
        return unknown ? null : true;
    }

    // ------------------------------------------------------------------ add-ons

    private static void RegisterAddons()
    {
        if (ModLoader.TryGetMod(SupportedMods.Catalyst, out Mod catalyst))
            Add("catalyst.Astrageldon", StaticMember(catalyst, "CatalystMod.WorldDefeats", "downedAstrageldon"));

        if (ModLoader.TryGetMod(SupportedMods.Hunt, out Mod hunt))
            Add("hunt.Goozma", SystemMember(hunt, "BossDownedSystem", "GoozmaDowned"));

        if (ModLoader.TryGetMod(SupportedMods.WrathOfTheGods, out Mod noxus))
        {
            Func<IList?> registry = StaticObject<IList>(noxus, "NoxusBoss.Core.World.WorldSaving.BossDownedSaveSystem", "downedRegistry");
            Add("wotg.Mars", () => registry()?.Contains("MarsBody"));
            Add("wotg.Avatar", () => registry()?.Contains("AvatarOfEmptiness"));
            Add("wotg.NamelessDeity", () => registry()?.Contains("NamelessDeityBoss"));

            // Solyn's quest chain (NoxusBoss.Core.SolynEvents.*, each a ModSystem with a Finished property).
            Add("wotg.Intro", SystemMember(noxus, "SolynIntroductionEvent", "Finished"));
            Add("wotg.Camp", SystemMember(noxus, "CampSetupEvent", "Finished"));
            Add("wotg.Stargaze", SystemMember(noxus, "StargazingEvent", "Finished"));
            Add("wotg.Keep", SystemMember(noxus, "PermafrostKeepEvent", "Finished"));
            Add("wotg.GenesisIntro", SystemMember(noxus, "GenesisIntroductionEvent", "Finished"));
            Add("wotg.Antiseed", SystemMember(noxus, "AntiseedEvent", "Finished"));
            Add("wotg.MarsQuest", SystemMember(noxus, "MarsCombatEvent", "Finished"));
            Add("wotg.GenesisDone", StaticMember(noxus, "NoxusBoss.Core.World.WorldSaving.WorldSaveSystem", "HasCompletedGenesis"));
        }

        if (ModLoader.TryGetMod(SupportedMods.Infernum, out Mod infernum))
        {
            Add("inf.Active", () => infernum.Call("GetInfernumActive") as bool?);
            Add("inf.BereftVassal", StaticMember(infernum, "InfernumMode.Core.GlobalInstances.Systems.WorldSaveSystem", "DownedBereftVassal"));
        }

        if (ModLoader.TryGetMod(SupportedMods.Fables, out Mod fables))
        {
            // With Calamity loaded, Fables replaces Calamity's Desert Scourge and Crabulon unless its
            // client config "ReplaceCalamity" (default on, reload required) is turned off.
            Func<object?>? configInstance = StaticReader(fables, "CalamityFables.CalCompatConfig", "Instance");
            Add("fables.ReplacesCalamity", () =>
            {
                if (!ModLoader.HasMod(SupportedMods.Calamity))
                    return false;
                object? config = configInstance?.Invoke();
                return config?.GetType().GetProperty("ReplaceCalamity")?.GetValue(config) as bool?;
            });
            Add("fables.DesertScourge", StaticMember(fables, "CalamityFables.Core.WorldProgressionSystem", "DefeatedDesertScourge"));
            Add("fables.Crabulon", StaticMember(fables, "CalamityFables.Core.WorldProgressionSystem", "DefeatedCrabulon"));
            Add("fables.ScourgeVsScarab", StaticMember(fables, "CalamityFables.Core.WorldProgressionSystem", "DefeatedScourgeVsScarab"));
            Add("fables.SirNautilus", StaticMember(fables, "CalamityFables.Content.Boss.SeaKnightMiniboss.SirNautilusDialogue", "DefeatedNautilus"));
        }

        if (ModLoader.TryGetMod(SupportedMods.SpiritReforged, out Mod spirit) && spirit.TryFind("Scarabeus", out ModNPC scarabeus))
        {
            Func<IEnumerable?> downed = StaticObject<IEnumerable>(spirit, "SpiritReforged.Common.NPCCommon.BossFlags", "DownedBossIds");
            int type = scarabeus.Type;
            Add("spirit.Scarabeus", () => downed()?.Cast<object>().Contains(type));
        }
    }

    // ------------------------------------------------------------------ Boss Checklist fallback

    /// <summary>
    /// Reads the "downed" function Boss Checklist exposes for an entry key such as
    /// "CalamityHunt Goozma". Only used for content without a dedicated flag above.
    /// </summary>
    private static bool? BossChecklistDowned(string entryKey)
    {
        if (_bossChecklistDowned is null)
        {
            _bossChecklistDowned = new Dictionary<string, Func<bool>>(StringComparer.OrdinalIgnoreCase);
            try
            {
                if (ModLoader.TryGetMod(SupportedMods.BossChecklist, out Mod bossChecklist)
                    && bossChecklist.Call("GetBossInfoDictionary", KhuMue.Instance, "2.0") is Dictionary<string, Dictionary<string, object>> entries)
                {
                    foreach ((string key, Dictionary<string, object> info) in entries)
                    {
                        if (info.TryGetValue("downed", out object? downed) && downed is Func<bool> func)
                            _bossChecklistDowned[key] = func;
                    }
                }
            }
            catch (Exception e)
            {
                KhuMue.Instance.Logger.Warn("Reading Boss Checklist entries failed; those states will show as unknown.", e);
            }
        }

        if (_bossChecklistDowned.TryGetValue(entryKey, out Func<bool>? getter))
        {
            try
            {
                return getter();
            }
            catch
            {
                return null;
            }
        }
        return null;
    }

    // ------------------------------------------------------------------ reflection helpers

    private const BindingFlags AnyStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    private const BindingFlags AnyInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>Getter for a static bool field or property; returns null if it no longer exists.</summary>
    private static Func<bool?> StaticMember(Mod mod, string typeName, string memberName)
    {
        Func<object?>? read = StaticReader(mod, typeName, memberName);
        if (read is null)
        {
            KhuMue.Instance.Logger.Warn($"Flag source {mod.Name}:{typeName}.{memberName} not found; its state will show as unknown.");
            return () => null;
        }
        return () => read() as bool?;
    }

    private static Func<T?> StaticObject<T>(Mod mod, string typeName, string memberName) where T : class
    {
        Func<object?>? read = StaticReader(mod, typeName, memberName);
        if (read is null)
        {
            KhuMue.Instance.Logger.Warn($"Flag source {mod.Name}:{typeName}.{memberName} not found; its state will show as unknown.");
            return () => null;
        }
        return () => read() as T;
    }

    private static Func<object?>? StaticReader(Mod mod, string typeName, string memberName)
    {
        Type? type = mod.Code?.GetType(typeName);
        if (type is null)
            return null;
        FieldInfo? field = type.GetField(memberName, AnyStatic);
        if (field is not null)
            return () => field.GetValue(null);
        PropertyInfo? property = type.GetProperty(memberName, AnyStatic);
        if (property?.GetMethod is not null)
            return () => property.GetValue(null);
        return null;
    }

    /// <summary>Getter for a bool member on a mod's ModSystem instance.</summary>
    private static Func<bool?> SystemMember(Mod mod, string systemName, string memberName)
    {
        if (!mod.TryFind(systemName, out ModSystem system))
        {
            KhuMue.Instance.Logger.Warn($"Flag source {mod.Name}:{systemName} not found; its state will show as unknown.");
            return () => null;
        }
        Type type = system.GetType();
        PropertyInfo? property = type.GetProperty(memberName, AnyInstance);
        if (property?.GetMethod is not null)
            return () => property.GetValue(system) as bool?;
        FieldInfo? field = type.GetField(memberName, AnyInstance);
        if (field is not null)
            return () => field.GetValue(system) as bool?;
        KhuMue.Instance.Logger.Warn($"Flag source {mod.Name}:{systemName}.{memberName} not found; its state will show as unknown.");
        return () => null;
    }
}
