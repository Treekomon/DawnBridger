using Dawnsbury.Audio;
using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core.CharacterBuilder;
using Dawnsbury.Core.CharacterBuilder.Selections.Options;
using Dawnsbury.Mods.DawnniExpanded.Ancestries;
using Dawnsbury.Mods.Dawnbridger;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Core.CharacterBuilder.Spellcasting;


namespace Dawnbridger
{
    public class VersatileHeritageIfrit
    {
        public static Trait IfritTrait = ModManager.RegisterTrait(
            "Ifrit",
            new TraitProperties("Ifrit", true, "", false)
            {
                IsAncestryTrait = true
            });
        public static Trait GenieTrait = ModManager.RegisterTrait("Genie"
            , new TraitProperties("Ifrit", true, "", false)
            );
        public static Feat Heritage = new VersatileHeritageSelectionFeat("Ifrit",
                "You descend from fire elementals or bear the mark of the Inner Spheres, and your features illustrate the influence that elemental fire has over you.",
                "You gain the ifrit trait, in addition to the traits from your ancestry. You gain resistance to fire equal to half your level rounded up. You can choose from ifrit feats and feats from your ancestry whenever you gain an ancestry feat.", new List<Trait> { DawnBridger.DBTrait , IfritTrait, Trait.Uncommon }, IfritTrait
            ).WithOnSheet((Action<CalculatedCharacterSheetValues>)(sheet => sheet.Ancestries.Add(IfritTrait)))
            .WithOnCreature(delegate (Creature creature)
            {
                creature.Traits.Add(IfritTrait);
                creature.Traits.Add(GenieTrait);
                //This, probably, is where you would add fire resistance.  Probably check Kineticist to see how
            });

        //This now just needs to, like, actually do something :P  Look for traits that modify skills to reference
        public static Feat Brigthsoul = new TrueFeat(FeatName.CustomFeat, 1,
            "The fire inside you asserts itself as beaming, radiant light. Your body is naturally luminescent, glowing with the effects of a primal light cantrip. \n\n The light is involuntary and constant",
            "You suffer a –2 circumstance penalty on Stealth checks to Hide or Sneak and gain a + 1 circumstance bonus on saves against light effects and effects that inflict the blinded or dazzled conditions..",
            new Trait[]
            {
                IfritTrait, DawnBridger.DBTrait
            }).WithCustomName("Brightsoul").WithOnCreature(sheet => sheet.AddQEffect(new QEffect()
        {
            BonusToSkills = (Func<Skill, Bonus>)(skill =>
                skill == Skill.Stealth ? (Bonus)null : new Bonus(-2, BonusType.Circumstance, "Brightsoul"))
        })).WithPermanentQEffect("You have +1 to saves against light and visual effects", (Action<QEffect>)(qf =>
        {
            qf.BonusToDefenses = (Func<QEffect, CombatAction, Defense, Bonus>)((effect, action, defense) =>
                action != null && (action.HasTrait(Trait.Light) || action.HasTrait(Trait.Visual))
                    ? new Bonus(1, BonusType.Circumstance, "Brightsoul")
                    : (Bonus)null);
        }));

        public static Feat ElementalLore = new TrueFeat(FeatName.CustomFeat, 1,
            "You've devoted yourself to researching the secrets of the Inner Sphere.",
            "You gain the trained proficiency in your choice of Survival and either Arcana or Nature. \n\n If you would automatically become trained in Survival (from your background or class, for example), you instead become trained in a skill of your choice. ",
            new Trait[]
            {
                IfritTrait, DawnBridger.DBTrait
            }).WithCustomName("Elemental Lore").WithOnSheet(sheet =>
        {
            if (sheet.GetProficiency(Trait.Survival) == Proficiency.Untrained)
            {
                sheet.SetProficiency(Trait.Survival, Proficiency.Trained);
            }
            else
            {
                sheet.AddSelectionOption(
                    new SingleFeatSelectionOption(
                        "Elemental Lore Skill 1", "Elemental Lore Skill 1", -1,
                        (ft) => ft is SkillSelectionFeat));
            }

            if ((sheet.GetProficiency(Trait.Arcana) == Proficiency.Untrained) ||
                (sheet.GetProficiency(Trait.Nature) == Proficiency.Untrained))
            {
                sheet.AddSelectionOption(
                    new SingleFeatSelectionOption(
                        "Elemental Lore Skill 2",
                        "Elemental Lore Skill 2",
                        -1,
                        (ft) => ft.FeatName == FeatName.Arcana || ft.FeatName == FeatName.Nature));
            }
            else
            {
                sheet.AddSelectionOption(
                    new SingleFeatSelectionOption(
                        "Elemental Lore Skill 2", "Elemental Lore Skill 2", -1,
                        (ft) => ft is SkillSelectionFeat));
            }
        });

        //Should be done
        public static Feat GenieWeaponFamiliarity = new TrueFeat(FeatName.CustomFeat, 1,
            "You've trained with weapons used by your genie ancestors.",
            "You are trained with falchions, ranseurs, scimitars, and tridents. \n\n For the purpose of determining your proficiency, martial geniekin weapons are simple weapons and advanced geniekin weapons are martial weapons.",
            new Trait[]
            {
                IfritTrait, DawnBridger.DBTrait
            }).WithCustomName("Genie Weapon Familiarity").WithOnSheet((sheet =>
        {
            sheet.Proficiencies.Set(IfritTrait, Proficiency.Trained);
            sheet.Proficiencies.AddProficiencyAdjustment((Func<List<Trait>, bool>)(item => item.Contains(GenieTrait) && item.Contains(Trait.Martial)), Trait.Simple);
            sheet.Proficiencies.AddProficiencyAdjustment((Func<List<Trait>, bool>)(item => item.Contains(GenieTrait) && item.Contains(Trait.Advanced)), Trait.Martial);
            sheet.Proficiencies.Set(Trait.Falchion, Proficiency.Trained);
            sheet.Proficiencies.Set(Trait.Scimitar, Proficiency.Trained);
            sheet.Proficiencies.Set(Trait.Trident, Proficiency.Trained);
        })).WithCustomName("Genie Weapon Familiarity");

        public static Feat InnerFire = new TrueFeat(FeatName.CustomFeat, 1,
            "You can call the fire inside you into the palm of your hand.",
            "You can cast the produce flame cantrip as an innate primal or arcane spell at will. A cantrip is heightened to a spell level equal to half your level rounded up.",
            new Trait[]
            {
                IfritTrait, DawnBridger.DBTrait
            }).WithCustomName("Inner Fire").WithRulesBlockForSpell(SpellId.ProduceFlame, IfritTrait).WithOnSheet((Action<CalculatedCharacterSheetValues>)(sheet => sheet.SetProficiency(Trait.Spell, Proficiency.Trained))).WithOnCreature((Action<Creature>)(creature => creature.GetOrCreateSpellcastingSource(SpellcastingKind.Innate, IfritTrait, Ability.Charisma, Trait.Arcane).WithSpells(new SpellId[1]
        {
            SpellId.ProduceFlame
        }, creature.MaximumSpellRank)));

        public static Feat SinisterAppearance = new TrueFeat(FeatName.CustomFeat, 1,
            "You possess horns, a tail, or red eyes, or could otherwise be mistaken for a tiefling.",
            "You gain the trained proficiency rank in Intimidation.\n\n If you would automatically become trained in Intimidation (from your background or class, for example), you instead become trained in a skill of your choice.\n\n You gain the Intimidating Glare skill feat",
            new Trait[]
            {
                IfritTrait, DawnBridger.DBTrait
            }).WithCustomName("Sinister Appearance").WithOnSheet(sheet =>
        {
            if (sheet.GetProficiency(Trait.Intimidation) == Proficiency.Untrained)
            {
                sheet.GrantFeat(FeatName.Intimidation);
            }
            else
            {
                sheet.AddSelectionOption(
                    new SingleFeatSelectionOption(
                        "Sinister Appearance Skill Skill", "Sinister Appearance Skill", -1,
                        (ft) => ft is SkillSelectionFeat));
            }
            sheet.GrantFeat(FeatName.IntimidatingGlare, null);
        });

        public static void LoadMod()
        {
            ModManager.AddFeat(Heritage);
            ModManager.AddFeat(Brigthsoul);
            ModManager.AddFeat(ElementalLore);
            ModManager.AddFeat(GenieWeaponFamiliarity);
            ModManager.AddFeat(InnerFire);
            ModManager.AddFeat(SinisterAppearance);

           
        }
    }



    }
