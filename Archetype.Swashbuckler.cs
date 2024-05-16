using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.CharacterBuilder.FeatsDb;
using Dawnsbury.Mods.DawnniExpanded;
using Dawnsbury.Mods.Phoenix;
using Dawnsbury.Core;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Roller;
using Dawnsbury.Display.Illustrations;

namespace Dawnsbury.Mods.Dawnbridger;


public static class ArchetypeSwashbuckler
{
    public static Feat SwashbucklerDedicationFeat;
    public static Trait SwashbucklerArchetypeTrait;

    public static Feat CreateArchetypeStyle(String name, string flavorText, Skill skillType)
    {
        return new Feat(FeatName.CustomFeat, flavorText, "You have learned to move with the grace and precision of a Swashbuckler",
            new List<Trait> { DawnBridger.DBTrait }, null).WithCustomName(name);
    }

    public static readonly Feat PreciseStrike = new Feat(ModManager.RegisterFeatName("Precise Strike", "Precise Strike"), "You strike with flair.", "When you have panache and you Strike with an agile or finesse melee weapon or agile or finesse unarmed attack, you deal 1 additional precision damage. If the strike is part of a finisher, the additional damage is 1d6 precision damage instead.", new List<Trait>(), null)
        .WithPermanentQEffect("You deal 1 additional damage when using agile or finesse weapons.", delegate (QEffect qf)
        {
            qf.Name = "Precise Strike";
            qf.YouDealDamageWithStrike = delegate (QEffect qf, CombatAction action, DiceFormula diceFormula, Creature defender)
            {
                bool flag = action.HasTrait(Trait.Agile) || action.HasTrait(Trait.Finesse) || action.HasTrait(Trait.Unarmed);
                bool flag2 = action.Owner.HasEffect(AddSwash.PanacheId);
                bool flag3 = action.HasTrait(AddSwash.Finisher);
                bool flag4 = !action.HasTrait(Trait.Ranged) || (action.HasTrait(Trait.Thrown) && (action.Owner.PersistentCharacterSheet?.Calculated.AllFeats.Any(feat => feat.Name == "Flying Blade") ?? false) && (defender.DistanceTo(qf.Owner) <= action.Item!.WeaponProperties!.RangeIncrement));
                if (flag && flag3 && flag4)
                {
                    return diceFormula.Add(DiceFormula.FromText("1d6", "Precise Strike"));
                }
                else if (flag && flag2 && flag4)
                {
                    return diceFormula.Add(DiceFormula.FromText("1", "Precise Strike"));
                }
                return diceFormula;
            };
        });


    public static readonly Feat BasicFinisher = new Feat(ModManager.RegisterFeatName("BasicFinisher", "Basic Finisher{icon:Action}"), "You gain an elegant finishing move that you can use when you have panache.", "If you have panache, you can make a Strike that deals extra damage.", new List<Trait>(), null)
        .WithPermanentQEffect("If you have panache, you can make a Strike that deals extra damage", delegate (QEffect qf)
        {
            qf.ProvideStrikeModifier = delegate (Item item)
            {
                StrikeModifiers conf = new StrikeModifiers();
                bool flag = !item.HasTrait(Trait.Ranged) && (item.HasTrait(Trait.Agile) || item.HasTrait(Trait.Finesse) || item.HasTrait(Trait.Unarmed));
                bool flag2 = qf.Owner.HasEffect(AddSwash.PanacheId);
                if (flag && flag2)
                {
                    CombatAction basicFinisher = qf.Owner.CreateStrike(item, -1, conf);

                    basicFinisher.Name = "Basic Finisher";
                    basicFinisher.Illustration = new SideBySideIllustration(item.Illustration, IllustrationName.StarHit);
                    basicFinisher.ActionCost = 1;
                    basicFinisher.WithEffectOnChosenTargets(async delegate (Creature creature, ChosenTargets targets)
                    {
                        targets.ChosenCreature!.AddQEffect(new QEffect()
                        {
                            ExpiresAt = ExpirationCondition.Ephemeral
                        });
                        AddSwash.FinisherExhaustion(basicFinisher.Owner);
                    });
                    basicFinisher.Traits.Add(AddSwash.Finisher);
                    return basicFinisher;
                }
                return null;
            };
        });


    public static void LoadMod()

    {



        SwashbucklerArchetypeTrait = ModManager.RegisterTrait(
            "SwashbucklerArchetype",
            new TraitProperties("SwashbucklerArchetype", true, "", false)
            {
            });



        //Correct Lorem Ipsum
        SwashbucklerDedicationFeat = new TrueFeat(FeatName.CustomFeat,
        2,
        "You have learned to fight with style and flair, gaining the abilities of a Swashbuckler",
        "You gain a Swashbuckler Style.\n\nYou gain the panache class feature, and you can gain panache in all the ways a swashbuckler of your style can.\n\nYou become trained in Acrobatics or the skill associated with your style.\n\nIf you were already trained in both skills, you instead become trained in a skill of your choice.\n\nYou also become trained in swashbuckler class DC.\n\nYou don't gain any other effects of your chosen style.\r\n ",
                new Trait[] { FeatArchetype.DedicationTrait, FeatArchetype.ArchetypeTrait, DawnBridger.DBTrait, DawnniExpanded.DawnniExpanded.DETrait, SwashbucklerArchetypeTrait }, new List<Feat>()
                {
                    CreateArchetypeStyle("Braggart", "You boast, taunt, and psychologically needle your foe.\n\nYou are trained in Intimidation.\n\nYou gain panache during an encounter whenever you successfully Demoralize a foe.", Skill.Intimidation).WithOnSheet(
                        delegate(CalculatedCharacterSheetValues sheet)
                        {
                            if (

                                sheet.GetProficiency(Trait.Acrobatics) == Proficiency.Untrained ||
                                sheet.GetProficiency(Trait.Intimidation) == Proficiency.Untrained)
                            {
                                sheet.AddSelectionOption(
                                    new SingleFeatSelectionOption(
                                        "Braggart Dedication Skill",
                                        "Braggart Dedication Skill",
                                        -1,
                                        (ft) => ft.FeatName == FeatName.Acrobatics || ft.FeatName == FeatName.Intimidation));
                            }
                            else
                            {
                                sheet.AddSelectionOption(
                                    new SingleFeatSelectionOption(
                                        "Braggart Dedication Skill", "Braggart Dedication Skill", -1,
                                        (ft) => ft is SkillSelectionFeat));
                            }

                        }),
                    CreateArchetypeStyle("Fencer", "You move carefully, feinting and creating false openings to lead your foes into inopportune attacks.\n\nYou are trained in Deception.\n\nYou gain panache during an encounter whenever you successfully Feint or Create a Diversion against a foe.", Skill.Deception).WithOnSheet(
                        delegate(CalculatedCharacterSheetValues sheet)
                        {
                            if (

                                sheet.GetProficiency(Trait.Acrobatics) == Proficiency.Untrained ||
                                sheet.GetProficiency(Trait.Deception) == Proficiency.Untrained)
                            {
                                sheet.AddSelectionOption(
                                    new SingleFeatSelectionOption(
                                        "Fencer Dedication Skill1",
                                        "Fencer Dedication Skill",
                                        -1,
                                        (ft) => ft.FeatName == FeatName.Acrobatics || ft.FeatName == FeatName.Deception));
                            }
                            else
                            {
                                sheet.AddSelectionOption(
                                    new SingleFeatSelectionOption(
                                        "Braggart Dedication Skill", "Fencer Dedication Skill", -1,
                                        (ft) => ft is SkillSelectionFeat));
                            }

                        }),
                    CreateArchetypeStyle("Gymnast", "You reposition, maneuver, and bewilder your foes with daring feats of physical prowess.\n\nYou are trained in Athletics.\n\nYou gain panache during an encounter whenever you successfully Grapple, Shove, or Trip a foe.", Skill.Acrobatics).WithOnSheet(
                        delegate(CalculatedCharacterSheetValues sheet)
                        {
                            if (

                                sheet.GetProficiency(Trait.Acrobatics) == Proficiency.Untrained ||
                                sheet.GetProficiency(Trait.Athletics) == Proficiency.Untrained)
                            {
                                sheet.AddSelectionOption(
                                    new SingleFeatSelectionOption(
                                        "Gymnast Dedication Skill",
                                        "Gymnast Dedication Skill",
                                        -1,
                                        (ft) => ft.FeatName == FeatName.Acrobatics || ft.FeatName == FeatName.Athletics));
                            }
                            else
                            {
                                sheet.AddSelectionOption(
                                    new SingleFeatSelectionOption(
                                        "Gymnast Dedication Skill", "Gymnast Dedication Skill", -1,
                                        (ft) => ft is SkillSelectionFeat));
                            }
                            
                        })
                }
        )
                .WithCustomName("Swashbuckler Dedication")
                .WithPrerequisite(values => values.FinalAbilityScores.TotalScore(Ability.Charisma) >= 14, "You must have at least 14 Charisma.")
                .WithPrerequisite(values => values.FinalAbilityScores.TotalScore(Ability.Dexterity) >= 14, "You must have at least 14 Dexterity.")
                .WithPrerequisite(values => values.Sheet?.Class.ClassTrait != Phoenix.AddSwash.SwashTrait, "You already have this archetype as a main class.")
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

                {
                    sheet.AdditionalClassTraits.Add(Phoenix.AddSwash.SwashTrait);

                    if (sheet.GetProficiency(Phoenix.AddSwash.SwashTrait) == Proficiency.Untrained)
                    {
                        sheet.SetProficiency(Phoenix.AddSwash.SwashTrait, Proficiency.Trained);
                    }
                }).WithOnCreature((CalculatedCharacterSheetValues sheet, Creature cr) => cr.AddQEffect(Phoenix.AddSwash.PanacheGranter()));

        ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
                4,
                "You have captured some of a Swashbuckler's flair.",
                "You gain a 1st- or 2nd-level Swashbuckler feat.",
                new Trait[] { FeatArchetype.ArchetypeTrait, DawnBridger.DBTrait, SwashbucklerArchetypeTrait })
                .WithCustomName("Basic Flair")
                .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(SwashbucklerDedicationFeat), "You must have the Swashbuckler Dedication feat.")
                .WithOnSheet(delegate (CalculatedCharacterSheetValues sheet)

                {

                    sheet.AddSelectionOption(
                            new SingleFeatSelectionOption(
                                "Basic Swashbuckler's flair",
                                "Basic Swashbuckler's flair feat",
                                -1,
                                (Feat ft) =>
                                {
                                    if (ft.HasTrait(Phoenix.AddSwash.SwashTrait) && !ft.HasTrait(FeatArchetype.DedicationTrait) && !ft.HasTrait(FeatArchetype.ArchetypeTrait))
                                    {

                                        if (ft.CustomName == null)
                                        {
                                            TrueFeat FeatwithLevel = (TrueFeat)AllFeats.All.Find(feat => feat.FeatName == ft.FeatName);

                                            if (FeatwithLevel.Level <= 2)
                                            {
                                                return true;
                                            }
                                            else return false;

                                        }
                                        else
                                        {
                                            TrueFeat FeatwithLevel = (TrueFeat)AllFeats.All.Find(feat => feat.CustomName == ft.CustomName);

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
                })

        );
        //The below feat should be replaced by one which just gives you access to a (limited) finisher.  Probably a Qeffect?
        //So we'll want two things, first, precise strike, but it only deals on additional damage
        //Second 1d6 on a finisher
        //Neither of these increase with levels
        //Since this is different from base Swashbuckler, we probably need to rebuild/edit it as a new ability here
        ModManager.AddFeat(new TrueFeat(FeatName.CustomFeat,
            4,
            "You've learned to land daring blows when you have Panache.",
            "You gain the precise strike class feature but you deal 1 additional damage on a hit and 1d6 damage on a finisher. \n\nThis damage doesn't increase as you gain levels. In addition, you gain the Basic Finisher action.",
            new Trait[] { FeatArchetype.ArchetypeTrait, DawnBridger.DBTrait, SwashbucklerArchetypeTrait })
            .WithCustomName("Finishing Precision")
            .WithPrerequisite((CalculatedCharacterSheetValues values) => values.AllFeats.Contains<Feat>(SwashbucklerDedicationFeat), "You must have the Swashbuckler Dedication feat.")
            .WithOnSheet(delegate(CalculatedCharacterSheetValues sheet)
            {
                sheet.AddFeat(PreciseStrike, null);
                sheet.AddFeat(BasicFinisher, null);
            }));



    ModManager.AddFeat(SwashbucklerDedicationFeat);



    }
}




