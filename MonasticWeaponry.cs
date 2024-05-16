using Dawnsbury.Core.CharacterBuilder.Feats;
using Dawnsbury.Core.Creatures;
using Dawnsbury.Core.Mechanics;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Modding;
using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Core.Possibilities;
using Dawnsbury.Core.CombatActions;
using Dawnsbury.Display.Illustrations;
using Dawnsbury.Core.Mechanics.Targeting;
using Dawnsbury;
using Dawnsbury.Core.CharacterBuilder.FeatsDb.Common;
using Dawnsbury.Core.Coroutines.Options;
using Dawnsbury.Core.Coroutines.Requests;
using Dawnsbury.Core.Mechanics.Core;
using Dawnsbury.Mods.Dawnbridger;

namespace Dawnbridger
{
    public class MonasticWeaponry
    {
        public static Feat MonasticWeaponryFeat;

        public static void LoadMod()
        {
            Feat AncestralWeaponryFeat = new TrueFeat(ModManager.RegisterFeatName("AncestralWeaponry", "Ancestral Weaponry"), 1, "You can use elven weapons as monk weapons",
                "You can use racial weapons with which you are trained like monk weapons",
                new Trait[] { Trait.Monk, DawnBridger.DBTrait }).WithCustomName("Ancestral Weaponry");

            ModManager.AddFeat(AncestralWeaponryFeat);


            Feat MonasticWeaponryFeat = new TrueFeat(FeatName.CustomFeat, 1, "You can use monk weapons",
                    "You can use monk weapons like they were unarmed attacks",
                    new Trait[] { Trait.Monk, DawnBridger.DBTrait })
                .WithOnCreature((sheet, creature) =>
                {

                    //We want a QEffect which gives them the option to weapon flurry, but only if they have a qualifying (monk) weapon
                    //I think that, after this, we might? Be good, given that we tag monk weapons with the 'unarmed' trait
                    //Since ki strike checks for that and other unarmed stuff might (?) as well?
                    QEffect WeaponFlurryHolder = new QEffect()
                    {
                        Name = "Weapon Flurry Granter",
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                        ProvideMainAction = (Func<QEffect, Possibility>)(qfSelf =>
                            (Possibility)(ActionPossibility)new CombatAction(qfSelf.Owner,
                                (Illustration)IllustrationName.FlurryOfBlows, "Weapon Flurry", new Trait[3]
                                {
                                    Trait.Monk, Trait.Flourish, Trait.Unarmed
                                },
                                "Make two monk weapon Strikes.\n\nIf both hit the same creature, combine their damage for the purpose of resistances and weaknesses.Apply your multiple attack penalty to the Strikes normally.\n\nAs it has the flourish trait, you can use Flurry of Blows only once per turn.",
                                (Target)Target.Self().WithAdditionalRestriction((Func<Creature, string>)(
                                    self =>
                                    {
                                        foreach (Item obj in self.MeleeWeapons)
                                        {
                                            if (!obj.HasTrait(Trait.Monk) && 
                                                    !(self.HasFeat(AncestralWeaponryFeat.FeatName) && self.HasFeat(FeatName.ElvenWeaponFamiliarity) && obj.HasTrait(Trait.Elf)) &&
                                                    !(self.HasFeat(AncestralWeaponryFeat.FeatName) && self.HasFeat(FeatName.DwarvenWeaponFamiliarity) && obj.HasTrait(Trait.Dwarf)) &&
                                                    !(self.HasFeat(AncestralWeaponryFeat.FeatName) && self.HasFeat(FeatName.OrcWeaponFamiliarity) && obj.HasTrait(Trait.Orc)) &&
                                                    !(self.HasFeat(AncestralWeaponryFeat.FeatName) && self.HasFeat(FeatName.DwarvenWeaponFamiliarity) && obj.HasTrait(Trait.Dwarf))

                                                    )
                                            {
                                                return "You must be wielding a qualifying weapon to make a Weapon Flurry.";
                                            }

                                        }
                                        foreach (Item obj in self.MeleeWeapons.Where<Item>(
                                                     (Func<Item, bool>)(weapon => weapon.HasTrait(Trait.Monk))))
                                        {
                                            if (self.CreateStrike(obj).CanBeginToUse(self).CanBeUsed)
                                                return (string)null;
                                        }

                                        if (self.HasFeat(FeatName.ElvenWeaponFamiliarity))
                                        {
                                            foreach (Item obj in self.MeleeWeapons.Where<Item>(
                                                         (Func<Item, bool>)(weapon => weapon.HasTrait(Trait.Elf))))
                                            {
                                                if (self.CreateStrike(obj).CanBeginToUse(self).CanBeUsed)
                                                    return (string)null;
                                            }
                                        }
                                        if (self.HasFeat(FeatName.DwarvenWeaponFamiliarity))
                                        {
                                            foreach (Item obj in self.MeleeWeapons.Where<Item>(
                                                         (Func<Item, bool>)(weapon => weapon.HasTrait(Trait.Dwarf))))
                                            {
                                                if (self.CreateStrike(obj).CanBeginToUse(self).CanBeUsed)
                                                    return (string)null;
                                            }
                                        }
                                        if (self.HasFeat(FeatName.OrcWeaponFamiliarity))
                                        {
                                            foreach (Item obj in self.MeleeWeapons.Where<Item>(
                                                         (Func<Item, bool>)(weapon => weapon.HasTrait(Trait.Orc))))
                                            {
                                                if (self.CreateStrike(obj).CanBeginToUse(self).CanBeUsed)
                                                    return (string)null;
                                            }
                                        }

                                        return "There is no nearby enemy or you can't make attacks.";
                                    })))
                            {
                                ShortDescription = "Make two monk weapon Strikes."
                            }.WithActionCost(1).WithActionId(ActionId.FlurryOfBlows).WithEffectOnEachTarget(
                                (Delegates.EffectOnEachTarget)(async (spell, self, target, irrelevantResult) =>
                                {
                                    List<Creature> chosenCreatures = new List<Creature>();
                                    int hpBefore = -1;
                                    for (int i = 0; i < 2; ++i)
                                    {
                                        await self.Battle.GameLoop.StateCheck();
                                        List<Option> options = new List<Option>();

                                        if (self.HasFeat(AncestralWeaponryFeat.FeatName))
                                        {
                                            foreach (Item obj in self.MeleeWeapons.Where<Item>(
                                                         (Func<Item, bool>)(weapon => weapon.HasTrait(Trait.Monk) || weapon.HasTrait(Trait.Elf) || weapon.HasTrait(Trait.Orc) || weapon.HasTrait(Trait.Dwarf))))
                                            {
                                                CombatAction strike = self.CreateStrike(obj);
                                                strike.WithActionCost(0);
                                                strike.Traits.Add(Trait.Unarmed);
                                                GameLoop.AddDirectUsageOnCreatureOptions(strike, options, true);
                                            }
                                        }
                                        else
                                        {
                                            foreach (Item obj in self.MeleeWeapons.Where<Item>(
                                                         (Func<Item, bool>)(weapon => weapon.HasTrait(Trait.Monk))))
                                            {
                                                CombatAction strike = self.CreateStrike(obj);
                                                strike.WithActionCost(0);
                                                strike.Traits.Add(Trait.Unarmed);
                                                GameLoop.AddDirectUsageOnCreatureOptions(strike, options, true);
                                            }
                                        }

                                        if (self.HasEffect(QEffectId.FlurryOfManeuvers))
                                        {
                                            GameLoop.AddDirectUsageOnCreatureOptions(
                                                Dawnsbury.Core.Possibilities.Possibilities.CreateGrapple(self)
                                                    .WithActionCost(0), options, true);
                                            GameLoop.AddDirectUsageOnCreatureOptions(
                                                Dawnsbury.Core.Possibilities.Possibilities.CreateShove(self)
                                                    .WithActionCost(0), options, true);
                                            GameLoop.AddDirectUsageOnCreatureOptions(
                                                Dawnsbury.Core.Possibilities.Possibilities.CreateTrip(self)
                                                    .WithActionCost(0), options, true);
                                        }

                                        if (options.Count > 0)
                                        {
                                            Option chosenOption;
                                            if (options.Count >= 2 || i == 0)
                                            {
                                                if (i == 0)
                                                    options.Add((Option)new CancelOption(true));
                                                chosenOption = (await self.Battle.SendRequest(
                                                    new AdvancedRequest(self, "Choose a creature to Strike.", options)
                                                    {
                                                        TopBarText = i == 0
                                                            ? "Choose a creature to Strike or right-click to cancel. (1/2)"
                                                            : "Choose a creature to Strike. (2/2)",
                                                        TopBarIcon = (Illustration)IllustrationName.Fist
                                                    })).ChosenOption;
                                            }
                                            else
                                                chosenOption = options[0];

                                            if (chosenOption is CreatureOption creatureOption2)
                                            {
                                                if (hpBefore == -1)
                                                    hpBefore = creatureOption2.Creature.HP;
                                                chosenCreatures.Add(creatureOption2.Creature);
                                            }

                                            if (chosenOption is CancelOption)
                                            {
                                                spell.RevertRequested = true;
                                                chosenCreatures = (List<Creature>)null;
                                                return;
                                            }

                                            int num = await chosenOption.Action() ? 1 : 0;
                                        }
                                    }

                                    if (self.HasEffect(QEffectId.StunningFist) &&
                                        (chosenCreatures.Count == 1 || chosenCreatures.Count == 2 &&
                                            chosenCreatures[0] == chosenCreatures[1]) &&
                                        chosenCreatures[0].HP < hpBefore)
                                    {
                                        CombatAction simple = CombatAction.CreateSimple(self, "Stunning Fist");
                                        simple.SpellLevel = self.MaximumSpellRank;
                                        simple.Traits.Add(Trait.Incapacitation);
                                        CheckResult checkResult = CommonSpellEffects.RollSavingThrow(chosenCreatures[0],
                                            simple, Defense.Fortitude,
                                            (Func<Creature, int>)(caster =>
                                                caster.Proficiencies.Get(Trait.Monk).ToNumber(caster.ProficiencyLevel) +
                                                caster.Abilities.Get(caster.Abilities.KeyAbility) + 10));
                                        if (checkResult <= CheckResult.Failure)
                                            chosenCreatures[0]
                                                .AddQEffect(QEffect.Stunned(checkResult == CheckResult.CriticalFailure
                                                    ? 3
                                                    : 1));
                                    }

                                    chosenCreatures = (List<Creature>)null;

                                })))
                    };
                    creature.AddQEffect(WeaponFlurryHolder);
                }).WithCustomName("Monastic Weaponry");

            
            ModManager.AddFeat(MonasticWeaponryFeat);

            
        }
    }
}

