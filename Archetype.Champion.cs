using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Mods.DawnniExpanded;
using Dawnsbury.Mods.Classes.Champion;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using ChampionFocusSpells;

namespace Dawnsbury.Mods.Dawnbridger;

public static class ArchetypeChampion
{

    public static Feat ChampionDedicationFeat;
    public static Trait ChampionArchetypeTrait;

    public static void LoadMod()

    {

        

        ChampionArchetypeTrait = ModManager.RegisterTrait(
            "ChampionArchetype",
            new TraitProperties("ChampionArchetype", true, "", false)
            {
            });

        ChampionDedicationFeat = new TrueFeat(FeatName.CustomFeat,
                2,
                "You are the champion, my friend",
                "Keep on fighting till the end",
                new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnBridger.DBTrait, DawnniExpanded.DawnniExpanded.DETrait, ChampionArchetypeTrait }, new List<Feat>()
                {
                    new ChampionCauseFeat("Liberator", "You're commited to defending the freedom of others.",
                        "With further progression, you can gain the {b}Liberating Step{/b} champion's reaction and the {b}{link:" +
                        ChampionSpells.LayOnHandsId.ToString() + "} Lay on Hands{/link}{/b} devotion spell.", new List<Trait>(),
                        new List<Feat>()).WithOnSheet((Action<CalculatedCharacterSheetValues>)(
                        sheet => { })),
                    new ChampionCauseFeat("Paladin", "You're honorable, forthright, and committed to pushing back the forces of cruelty.", "With further progression, you can gain the {b}Retributive Strike{/b} champion's reaction and the {b}{link:" + ChampionSpells.LayOnHandsId.ToString() + "} Lay on Hands{/link}{/b} devotion spell.", new List<Trait>(),
                        new List<Feat>()).WithOnSheet((Action<CalculatedCharacterSheetValues>)(
                        sheet => { })),
                    new ChampionCauseFeat("Redeemer", "You're full of kindness and forgiveness.", "With further progression, you can gain the {b}Glimpse of Redemption{/b} champion's reaction and the {b}{link:" + ChampionSpells.LayOnHandsId.ToString() + "} Lay on Hands{/link}{/b} devotion spell.", new List<Trait>(),
                        new List<Feat>()).WithOnSheet((Action<CalculatedCharacterSheetValues>)(
                        sheet => { }))


                })
                .WithCustomName("Champion Dedication")
                .WithPrerequisite(values => values.FinalAbilityScores.TotalScore(Ability.Charisma) >= 14, "You must have at least 14 Charisma.")
                .WithPrerequisite(values => values.FinalAbilityScores.TotalScore(Ability.Strength) >= 14, "You must have at least 14 Strength.")
                .WithPrerequisite(values => values.Sheet?.Class.ClassTrait != DawnsburyChampion.ChampionTrait, "You already have this archetype as a main class.")

                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

                {
                    sheet.AdditionalClassTraits.Add(DawnsburyChampion.ChampionTrait);
                    CalculatedCharacterSheetValues sheet2 = sheet;
                    sheet2.AddSelectionOption((SelectionOption)new SingleFeatSelectionOption("championDeity", "Deity", -1, (Func<Feat, bool>)(feat => feat is ChampionDeitySelectionFeat)));
                    if (sheet.GetProficiency(DawnsburyChampion.ChampionTrait) == Proficiency.Untrained)
                    {
                        sheet.SetProficiency(DawnsburyChampion.ChampionTrait, Proficiency.Trained);
                    }

                    //Champion Skill Prof
                    //It's religion and Deity's skill
                    //So you need to pick a deity here as well
                    //And a cause, which, I imagine, will look a lot like the Swash's style

                    if (sheet.GetProficiency(Trait.Religion) == Proficiency.Untrained)
                    {
                        sheet.AddSelectionOption(
                            new SingleFeatSelectionOption(
                                "Champion Dedication Skill",
                                "Champion Dedication skill",
                                -1,
                                (ft) => ft.FeatName == FeatName.Religion)

                                );
                    }
                    else
                    {

                        sheet.AddSelectionOption(
                            new SingleFeatSelectionOption(
                                "Champion Dedication Skill1",
                                "Champion Dedication skill",
                                -1,
                                (ft) => ft is SkillSelectionFeat)

                                );
                    }
                }).WithOnSheet(sheet =>
                {

                    if (sheet.GetProficiency(Trait.LightArmor) == Proficiency.Trained
                        && sheet.GetProficiency(Trait.MediumArmor) == Proficiency.Trained
                        && sheet.GetProficiency(Trait.HeavyArmor) == Proficiency.Untrained)
                    {
                        sheet.SetProficiency(Trait.HeavyArmor, Proficiency.Trained);
                    }

                    if (sheet.GetProficiency(Trait.LightArmor) == Proficiency.Untrained)
                    {
                        sheet.SetProficiency(Trait.LightArmor, Proficiency.Trained);
                    }

                    if (sheet.GetProficiency(Trait.MediumArmor) == Proficiency.Untrained)
                    {
                        sheet.SetProficiency(Trait.MediumArmor, Proficiency.Trained);
                    }
                })
            ;


        ;

        ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
                4,
                "You have grown further devoted to your cause.",
                "You gain a 1st- or 2nd-level Champion feat.",
                new Trait[] { FeatArchetype.ArchetypeTrait, DawnBridger.DBTrait, ChampionArchetypeTrait })
            .WithCustomName("Basic Devotion")
            .WithPrerequisite(
                (CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(ChampionDedicationFeat),
                "You must have the Champion Dedication feat.")
            .WithOnSheet(delegate(CalculatedCharacterSheetValues sheet)
            {
                sheet.AddSelectionOption(
                    new SingleFeatSelectionOption(
                        "Basic Champion's devotion",
                        "Basic Champion's devotion feat",
                        -1,
                        (Feat ft) =>
                        {
                            if (ft.HasTrait(DawnsburyChampion.ChampionTrait) &&
                                !ft.HasTrait(FeatArchetype.DedicationTrait) &&
                                !ft.HasTrait(FeatArchetype.ArchetypeTrait))
                            {

                                if (ft.CustomName == null)
                                {
                                    TrueFeat FeatwithLevel =
                                        (TrueFeat)AllFeats.All.Find(feat => feat.FeatName == ft.FeatName);

                                    if (FeatwithLevel.Level <= 2)
                                    {
                                        return true;
                                    }
                                    else return false;

                                }
                                else
                                {
                                    TrueFeat FeatwithLevel =
                                        (TrueFeat)AllFeats.All.Find(feat => feat.CustomName == ft.CustomName);

                                    if (FeatwithLevel.Level <= 2)
                                    {
                                        return true;
                                    }

                                    return false;
                                }
                            }

                            return false;
                        })
                );
            }));

        ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
                        4,
                        "Your champion training has made your more resilient.",
                        "You gain 3 additional Hit Points for each champion archetype class feat you have.\n\nAs you continue selecting champion archetype class feats, you continue to gain additional Hit Points in this way.",
                        new Trait[] { FeatArchetype.ArchetypeTrait, DawnBridger.DBTrait, ChampionArchetypeTrait })
                    .WithCustomName("Champion Resiliency")
                    .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(ChampionDedicationFeat), "You must have the Champion Dedication feat.")
                    .WithPrerequisite((CalculatedCharacterSheetValues values) =>
                            values.Sheet?.Class.ClassTrait != Trait.Ranger &&
                            values.Sheet?.Class.ClassTrait != Trait.Barbarian &&
                            values.Sheet?.Class.ClassTrait != Trait.Monk

                        , "You have a class granting more than Hit Points per level than 8 + your Constitution modifier")
                    .WithOnCreature((CalculatedCharacterSheetValues sheet, Creature cr) =>
                    {

                        int ReslientHP = 3 * sheet.AllFeats.Count(x => x.HasTrait(ChampionArchetypeTrait));
                        cr.MaxHP += ReslientHP;

                    }));

                ModManager.AddFeat((new TrueFeat(FeatName.CustomFeat,
                        4, "You can lay on hands.", "Do lay on hands things",
                        new Trait[] { FeatArchetype.ArchetypeTrait, DawnBridger.DBTrait, ChampionArchetypeTrait })
                    .WithCustomName("Healing Touch")
                    .WithPrerequisite(
                        (CalculatedCharacterSheetValues values) =>
                            values.AllFeats.Contains<Feat>(ChampionDedicationFeat),
                        "You must have the Champion Dedication feat.")
                    .WithOnSheet((CalculatedCharacterSheetValues sheet) =>
                        sheet.AddFocusSpellAndFocusPoint(DawnsburyChampion.ChampionTrait, Ability.Charisma,
                            ChampionSpells.LayOnHandsId)
                    )));

                ModManager.AddFeat(ChampionDedicationFeat);
    }
        

        
    }
