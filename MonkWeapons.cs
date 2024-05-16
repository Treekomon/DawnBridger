using Dawnsbury.Core;
using Dawnsbury.Core.Mechanics.Enumerations;
using Dawnsbury.Core.Mechanics.Treasure;
using Dawnsbury.Modding;
using Dawnsbury.Mods.Dawnbridger;


namespace Dawnbridger
{
    public static class MonkWeapons
    {
        public static void LoadWeapons()
        {
            ModManager.RegisterNewItemIntoTheShop("Elven Branched Spear", itemName =>
                new Item(IllustrationName.Spear, "Elven Branched Spear", new Trait[8] { Trait.Elf, Trait.Finesse, Trait.DeadlyD8, Trait.Martial, Trait.Spear, Trait.Melee, Trait.TwoHanded, DawnBridger.DBTrait })
                    {
                        ItemName = itemName,

                    }
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Piercing)));

            //Missing Versatile Bludgeoning
            ModManager.RegisterNewItemIntoTheShop("Sai", itemName =>
                new Item(IllustrationName.Dagger, "Sai", new Trait[8] { Trait.Monk, Trait.Finesse, Trait.Agile, Trait.Disarm, Trait.Martial, Trait.Knife, Trait.Melee, DawnBridger.DBTrait })
                    {
                        ItemName = itemName,

                    }
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Piercing)));

            ModManager.RegisterNewItemIntoTheShop("Kama", itemName =>
                new Item(IllustrationName.Dagger, "Kama", new Trait[7] { Trait.Monk, Trait.Agile, Trait.Trip, Trait.Martial, Trait.Knife, Trait.Melee, DawnBridger.DBTrait })
                    {
                        ItemName = itemName,

                    }
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Slashing)));

            ModManager.RegisterNewItemIntoTheShop("Fighting Fan", itemName =>
                new Item(IllustrationName.Dagger, "Fighting Fan", new Trait[9] { Trait.Monk, Trait.Finesse, Trait.Agile, Trait.Backstabber, Trait.DeadlyD6, Trait.Martial, Trait.Knife, Trait.Melee, DawnBridger.DBTrait })
                    {
                        ItemName = itemName,

                    }
                    .WithWeaponProperties(new WeaponProperties("1d4", DamageKind.Slashing)));

            ModManager.RegisterNewItemIntoTheShop("Nunchaku", itemName =>
                new Item(IllustrationName.Club, "Nunchaku", new Trait[8] { Trait.Monk, Trait.Backswing, Trait.Finesse, Trait.Disarm, Trait.Martial, Trait.Club, Trait.Melee, DawnBridger.DBTrait })
                    {
                        ItemName = itemName,

                    }
                    .WithWeaponProperties(new WeaponProperties("1d6", DamageKind.Bludgeoning)));
        }
    }
}
