using Dawnbridger;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Mods.DawnniExpanded;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Mods.Phoenix;
using Dawnsbury.Mods.Classes.Champion;
using Dawnsbury.Mods.Dawnbridger;

namespace Dawnsbury.Mods.Dawnbridger
{
    public class DawnBridger
    {

        public static Trait DBTrait;
        public static Trait HomebrewTrait;


        [DawnsburyDaysModMainMethod]



        public static void LoadMod()
        {

            Dawnsbury.Modding.LoadOrder.WhenFeatsBecomeLoaded += () =>
            {
                DBTrait = ModManager.RegisterTrait(
                    "DBridgerEx",
                    new TraitProperties("DBridgerEx", true)
                );
                HomebrewTrait = ModManager.RegisterTrait(
                    "Homebrew",
                    new TraitProperties("Homebrew", true)
                );


                FeatArchetype.DedicationFeat = new TrueFeat(FeatName.CustomFeat,
                        2,
                        "Instead of a class feat, you gain an archetype dedication feat of your choice. You may have only one archetype.",
                        "You gain an archetype dedication feat.",
                        new Trait[]
                        {
                        FeatArchetype.ArchetypeTrait, FeatArchetype.DedicationTrait, AddSwash.SwashTrait, DawnsburyChampion.ChampionTrait,
                        Trait.ClassFeat, DBTrait
                        })
                    .WithCustomName("Archetype Dedication")
                    .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)



                    {
                        sheet.AddSelectionOption(
                            new SingleFeatSelectionOption(
                                "Archetype Dedication",
                                "Archetype Dedication feat",
                                -1,
                                (Feat ft) => ft.HasTrait(FeatArchetype.DedicationTrait) &&
                                             ft.CustomName != "Archetype Dedication"));
                    });

                FeatArchetype.ArchetypeFeat = new TrueFeat(FeatName.CustomFeat,
                        4,
                        "Instead of a class feat, you gain an archetype feat of your choice for your dedication.",
                        "You gain an archetype feat.",
                        new Trait[] { FeatArchetype.ArchetypeTrait, Trait.ClassFeat, AddSwash.SwashTrait, DawnsburyChampion.ChampionTrait, DBTrait })
                    .WithMultipleSelection()
                    .WithCustomName("Archetype Feat")
                    .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Any(Ft => Ft.HasTrait(FeatArchetype.DedicationTrait)), "You must have a Dedication feat.")
                    .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

                    {
                        sheet.AddSelectionOption(
                            new SingleFeatSelectionOption(
                                "Archetype",
                                "Archetype feat",
                                -1,
                                (Feat ft) => (ft.HasTrait(FeatArchetype.ArchetypeTrait) && !ft.HasTrait(FeatArchetype.DedicationTrait) && ft.CustomName != "Archetype Feat") || ft.CustomName == "None")
                        );

                    });

                ModManager.AddFeat(FeatArchetype.DedicationFeat);
                ModManager.AddFeat(FeatArchetype.ArchetypeFeat);



                ArchetypeSwashbuckler.LoadMod();
                ArchetypeChampion.LoadMod();
                MonasticWeaponry.LoadMod();
                MonkWeapons.LoadWeapons();
                VersatileHeritageIfrit.LoadMod();
            };




        }
    }




}