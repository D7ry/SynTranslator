using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Translator
{
    public class Program
    {
        static Lazy<Settings> Settings = null!;
        static int i = 0;
        static bool verboseLog = false;

        public static Task<int> Main(string[] args)
        {
            return SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings(
                    nickname: "Settings",
                    path: "settings.json",
                    out Settings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "Translator.esp")
                .Run(args);
        }
        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (Settings.Value.forwardSelectedLOMods)
            {
                Console.WriteLine("forwarding selected mods within LO");
                forwardLOTrans();
            }
            if (Settings.Value.forwardOutsideMods)
            {
                Console.WriteLine("forwarding selected mods from translation directory");
                forwardOutsideTrans();
            }

            void forwardOutsideTrans()
            {
                var translationDir = Settings.Value.translationDir;
                verboseLog = Settings.Value.verboseLog;
                List<String> pluginLst = Settings.Value.outsidePlugins;
                if (translationDir == "")
                {
                    Console.WriteLine("Warning: translation directory not set; please manually input translation directory path!");
                }
                if (pluginLst.Count == 0)
                {
                    Console.WriteLine("Warning: no plugin found in translation list; please manually input plugin name!");
                }

                for (int i = 0; i < pluginLst.Count; i++)
                {
                    String fileName = pluginLst[i];
                    if (fileName.Contains(".esp") || fileName.Contains(".esm") || fileName.Contains(".esl"))
                    {
                        Console.WriteLine("processing translation from " + fileName);
                        String espPath = $"{translationDir}/{fileName}";
                        SkyrimMod esp = SkyrimMod.CreateFromBinary(espPath, SkyrimRelease.SkyrimSE);
                        espCopy(esp);
                    }
                    else
                    {
                        Console.WriteLine("Error: wrong plugin name format; aborting process.");
                        return;
                    }
                }
            }

            void forwardLOTrans()
            {

            }
            


            //copy stuff that need to be translated into Synthesis.esp
            //condition check:
            //iff stuff with corresponding formID doens't exist, skip it.
            void espCopy(ISkyrimMod esp)
            {
                int k = 0;
                if (Settings.Value.weapon)
                {
                    foreach (var _rec in esp.Weapons)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IWeaponGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Weapons.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated weapon: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.armor)
                {
                    foreach (var _rec in esp.Armors)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IArmorGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Armors.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated armor: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.ingredient)
                {
                    foreach (var _rec in esp.Ingredients)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IIngredientGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Ingredients.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated ingredient: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.npc)
                {
                    foreach (var _rec in esp.Npcs)
                    {
                        FormKey key = _rec.FormKey;
                        if (state.LinkCache.TryResolve<INpcGetter>(key, out var rec) && _rec.Name != null)
                        {
                            var transPatch = state.PatchMod.Npcs.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated NPC name: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.worldSpace)
                {
                    foreach (var _rec in esp.Worldspaces)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IWorldspaceGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Worldspaces.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated world space: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.magicEffect)
                {
                    foreach (var _rec in esp.MagicEffects)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IMagicEffectGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.MagicEffects.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            if (_rec.Description != null)
                            {
                                transPatch.Description = _rec.Description.ToString();
                            }
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated magic effect: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.spell)
                {
                    foreach (var _rec in esp.MagicEffects)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<ISpellGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Spells.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            if (_rec.Description != null)
                            {
                                transPatch.Description = _rec.Description.ToString();
                            }
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated magic effect: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.message) {
                    foreach (var _rec in esp.Messages)
                    {
                        FormKey key = _rec.FormKey;
                        if (state.LinkCache.TryResolve<IMessageGetter>(key, out var rec) && _rec.Name != null)
                        {
                            var transPatch = state.PatchMod.Messages.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            transPatch.Description = _rec.Description.ToString();
                            transPatch.Quest = _rec.Quest.AsNullable();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated message: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.cell)
                {
                } //FIXME
                if (Settings.Value.container)
                {
                    foreach (var _rec in esp.Containers)
                    {
                        FormKey key = _rec.FormKey;
                        if (state.LinkCache.TryResolve<IContainerGetter>(key, out var rec) && _rec.Name != null)
                        {
                            var transPatch = state.PatchMod.Containers.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated container: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.objectEffect)
                {
                    foreach (var _rec in esp.ObjectEffects)
                    {
                        FormKey key = _rec.FormKey;
                        if (state.LinkCache.TryResolve<IObjectEffectGetter>(key, out var rec) && _rec.Name != null)
                        {
                            var transPatch = state.PatchMod.ObjectEffects.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated object effect: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.hazard)
                {
                    foreach (var _rec in esp.Hazards)
                    {
                        FormKey key = _rec.FormKey;
                        if (state.LinkCache.TryResolve<IHazardGetter>(key, out var rec) && _rec.Name != null)
                        {
                            var transPatch = state.PatchMod.Hazards.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated hazard: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.perk)
                {
                    foreach (var _rec in esp.Perks)
                    {
                        FormKey key = _rec.FormKey;
                        if (state.LinkCache.TryResolve<IPerkGetter>(key, out var rec) && _rec.Name != null)
                        {
                            var transPatch = state.PatchMod.Perks.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            transPatch.Description = _rec.Description.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated perk: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.quest)
                {
                    foreach (var _rec in esp.Quests)
                    {
                        FormKey key = _rec.FormKey;
                        if (state.LinkCache.TryResolve<IQuestGetter>(key, out var rec) && _rec.Name != null)
                        {
                            var transPatch = state.PatchMod.Quests.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated quest: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.activator)
                {
                    foreach (var _rec in esp.Activators)
                    {
                        FormKey key = _rec.FormKey;
                        if (state.LinkCache.TryResolve<IActivatorGetter>(key, out var rec) && _rec.Name != null)
                        {
                            var transPatch = state.PatchMod.Activators.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            if (_rec.ActivateTextOverride != null)
                            {
                                transPatch.ActivateTextOverride = _rec.ActivateTextOverride.ToString();
                            }
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated activator: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.ammunition)
                {
                    foreach (var _rec in esp.Ammunitions)
                    {
                        FormKey key = _rec.FormKey;
                        if (state.LinkCache.TryResolve<IAmmunitionGetter>(key, out var rec) && _rec.Name != null)
                        {
                            var transPatch = state.PatchMod.Ammunitions.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            if (_rec.Description != null)
                            {
                                transPatch.Description = _rec.Description.ToString();
                            }
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated ammunition: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.projectile)
                {
                    foreach (var _rec in esp.Projectiles)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IProjectileGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Projectiles.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated projectile: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.race)
                {
                    foreach (var _rec in esp.Races)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IRaceGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Races.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated race: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.scroll)
                {
                    foreach (var _rec in esp.Scrolls)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IScrollGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Scrolls.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated scroll: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.ingestible)
                {
                    foreach (var _rec in esp.Ingestibles)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IIngestibleGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Ingestibles.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated ingestible: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.ActorValueInformation)
                {
                    foreach (var _rec in esp.ActorValueInformation)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IActorValueInformationGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.ActorValueInformation.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            if (_rec.Description != null)
                            {
                                transPatch.Description = _rec.Description.ToString();
                            }
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated actor value information: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.book)
                {
                    foreach (var _rec in esp.Books)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IBookGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Books.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            transPatch.BookText = _rec.BookText.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated book: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.dialogueTopic)
                {
                    foreach (var _rec in esp.DialogTopics)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IDialogTopicGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.DialogTopics.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated dialogue topic: {rec.Name}");
                            }
                        }
                    }
                } //FIXME
                if (Settings.Value.door)
                {
                    foreach (var _rec in esp.Doors)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IDoorGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Doors.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated door: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.flora)
                {
                    foreach (var _rec in esp.Florae)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IFloraGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Florae.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            if (_rec.ActivateTextOverride != null)
                            {
                                transPatch.ActivateTextOverride = _rec.ActivateTextOverride.ToString();
                            }
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated flora: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.tree)
                {
                    foreach (var _rec in esp.Trees)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<ITreeGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Trees.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated tree: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.furniture)
                {
                    foreach (var _rec in esp.Furniture)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IFurnitureGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Furniture.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated furniture: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.key)
                {
                    foreach (var _rec in esp.Keys)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IKeyGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Keys.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated key: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.location)
                {
                    foreach (var _rec in esp.Locations)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<ILocationGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Locations.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated key: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.miscItem)
                {
                    foreach (var _rec in esp.MiscItems)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IMiscItemGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.MiscItems.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated misc item: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.shout)
                {
                    foreach (var _rec in esp.Shouts)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<IShoutGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.Shouts.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            if (_rec.Description != null)
                            {
                                transPatch.Description = _rec.Description.ToString();
                            }
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated shout: {rec.Name}");
                            }
                        }
                    }
                }
                if (Settings.Value.soulGem)
                {
                    foreach (var _rec in esp.SoulGems)
                    {
                        FormKey key = _rec.FormKey;
                        if (_rec.Name != null && state.LinkCache.TryResolve<ISoulGemGetter>(key, out var rec))
                        {
                            var transPatch = state.PatchMod.SoulGems.GetOrAddAsOverride(rec);
                            transPatch.Name = _rec.Name.ToString();
                            i++; k++;
                            if (verboseLog)
                            {
                                Console.WriteLine($"translated soul gem: {rec.Name}");
                            }
                        }
                    }
                }
                
                System.Console.WriteLine($"translated {k} records");

                
                //state.LoadOrder.PriorityOrder.SkyrimMajorRecord().WinningContextOverrides(state.LinkCache).ForEach(obj =>
                //{
                    //var record = obj.Record;
                    //if (record is IWeaponGetter item && item.FormKey)
                //});


            }



            System.Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");


            uint count = 0;
                // For every Npc that exists
            foreach (var npc in state.LoadOrder.PriorityOrder.Npc().WinningOverrides())
            {
                // For every Npc group in our target mods, in order
                //Console.WriteLine("NPC NAME: " + npc.Name);
            }
            foreach (var worldSpace in state.LoadOrder.PriorityOrder.Worldspace().WinningOverrides())
            {
                //var worldCopy = worldSpace.DeepCopy();
                //Console.WriteLine(worldCopy.Name);
            }
            foreach (var weapon in state.LoadOrder.PriorityOrder.Weapon().WinningOverrides())
            {
                //var weaponCopy = weapon.DeepCopy();
                //Console.WriteLine(weaponCopy.Name);
            }

            System.Console.WriteLine($"Translated {i} records.");
        }
    }
}
