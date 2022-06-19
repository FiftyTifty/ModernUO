using Server.Items;

namespace Server.Mobiles
{
    [TypeAlias("Server.Mobiles.Lavaserpant")]
    public class LavaSerpent : BaseCreature
    {
        [Constructible]
        public LavaSerpent() : base(AIType.AI_Melee)
        {
            Body = 90;
            BaseSoundID = 219;

            SetStr(386, 415);
            SetDex(56, 80);
            SetInt(66, 85);

            SetHits(232, 249);
            SetMana(0);

            SetDamage(10, 22);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 80);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 25.3, 70.0);
            SetSkill(SkillName.Tactics, 65.1, 70.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);

            Fame = 4500;
            Karma = -4500;

            VirtualArmor = 40;

            PackItem(new SulfurousAsh(3));
            PackItem(new Bone());
            // TODO: body parts, armour
        }

        public LavaSerpent(Serial serial) : base(serial)
        {
        }

        public override string CorpseName => "a lava serpent corpse";
        public override string DefaultName => "a lava serpent";

        public override bool DeathAdderCharmable => true;

        public override bool HasBreath => true; // fire breath enabled
        public override int Meat => 4;
        public override int Hides => 15;
        public override HideType HideType => HideType.Spined;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
        }

        public override void Serialize(IGenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(IGenericReader reader)
        {
            base.Deserialize(reader);

            var version = reader.ReadInt();

            if (BaseSoundID == -1)
            {
                BaseSoundID = 219;
            }
        }
    }
}
