﻿using Server.Engines.XmlSpawner2;
using System;

namespace Server.Items
{

    public class SiegeCatapult : BaseSiegeWeapon
    {
        public override int LabelNumber => 1025704;
        public override double WeaponLoadingDelay => 20;  // base delay for loading this weapon
        public override double WeaponStorageDelay => 15.0;  // base delay for packing away this weapon

        //override inutili
        //public override double WeaponDamageFactor { get { return base.WeaponDamageFactor * 1.0; } } // damage multiplier for the weapon
        //public override double WeaponRangeFactor { get { return base.WeaponRangeFactor * 1.0; } } //  range multiplier for the weapon

        public override int MinTargetRange => 2;  // target must be further away than this
        public override int MinStorageRange => 1;  // player must be at least this close to store the weapon
        public override int MinFiringRange => 3;  // player must be at least this close to fire the weapon

        public override bool Parabola => true;  // whether the weapon needs to consider line of sight when selecting a target
        public override int BaseLenght => CatapultNorth.Length;
        // facing 0
        public static int[] CatapultWest = new int[] { 5846, 5823, 5822, 5821, 5820, 5819, 5818, 5817, 5826, 5831, 5841, 5836 };
        public static int[] CatapultWestXOffset = new int[] { 2, -2, 0, 0, 1, 2, 2, 2, 1, 1, -1, -1 };
        public static int[] CatapultWestYOffset = new int[] { 1, -1, -1, 1, 0, -1, 0, 1, 1, -1, -1, 1 };
        public static int[] CatapultWestLaunch = new int[] { 5847, 5848, 5849 };
        // facing 1
        public static int[] CatapultNorth = new int[] { 5809, 5784, 5786, 5789, 5785, 5783, 5782, 5780, 5781, 5799, 5794, 5808 };
        public static int[] CatapultNorthXOffset = new int[] { 1, 1, -1, 1, -1, 0, -1, 1, 0, 1, -1, -1 };
        public static int[] CatapultNorthYOffset = new int[] { 1, -1, -3, 0, -1, 0, 1, 1, 1, -2, 0, -2 };
        public static int[] CatapultNorthLaunch = new int[] { 5810, 5810, 5810 };
        // facing 2
        public static int[] CatapultEast = new int[] { 5773, 5763, 5758, 5746, 5747, 5748, 5750, 5751, 5752, 5753, 5768, 5749 };
        public static int[] CatapultEastXOffset = new int[] { 1, -1, -1, 1, 0, 0, -2, -2, -1, 1, 1, -2 };
        public static int[] CatapultEastYOffset = new int[] { 1, -1, 1, 0, 1, -1, 0, 1, 0, 1, -1, -1 };
        public static int[] CatapultEastLaunch = new int[] { 5773, 5775, 5776 };
        // facing 3
        public static int[] CatapultSouth = new int[] { 5731, 5704, 5716, 5721, 5710, 5709, 5708, 5707, 5714, 5706, 5730, 5705 };
        public static int[] CatapultSouthXOffset = new int[] { 1, 0, 1, -1, 1, 0, -1, -1, 1, 1, -1, 0 };
        public static int[] CatapultSouthYOffset = new int[] { 1, -1, -1, -1, 0, -2, -2, 0, 1, -2, 1, 1 };
        public static int[] CatapultSouthLaunch = new int[] { 5732, 5733, 5734 };

        private Type[] m_allowedprojectiles = new Type[] { typeof(SiegeCannonball) };

        public override Type[] AllowedProjectiles => m_allowedprojectiles;

        private InternalTimer m_Timer;

        public void DoTimer(Mobile from, SiegeCatapult weapon, IEntity target, Point3D targetloc, Item projectile, TimeSpan damagedelay, int step)
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
            }

            if (step > 4 || step < 0)
            {
                return;
            }

            m_Timer = new InternalTimer(from, weapon, target, targetloc, projectile, damagedelay, step);
            m_Timer.Start();
        }

        // animation timer that begins on firing
        private class InternalTimer : Timer
        {
            private SiegeCatapult m_weapon;
            private int m_step;
            private Item m_Projectile;
            private Point3D m_targetloc;
            private IEntity m_target;
            private Mobile m_from;
            private TimeSpan m_damagedelay;

            public InternalTimer(Mobile from, SiegeCatapult weapon, IEntity target, Point3D targetloc, Item projectile, TimeSpan damagedelay, int step)
                : base(TimeSpan.FromMilliseconds(250))
            {
                Priority = TimerPriority.FiftyMS;
                m_weapon = weapon;
                m_Projectile = projectile;
                m_target = target;
                m_targetloc = targetloc;
                m_from = from;
                m_step = step;
                m_damagedelay = damagedelay;
            }

            protected override void OnTick()
            {
                if (m_weapon != null && !m_weapon.Deleted && m_Projectile is ISiegeProjectile pitem)
                {
                    int animationid = pitem.AnimationID;
                    int animationhue = pitem.AnimationHue;

                    switch (m_step)
                    {
                        case 0:
                        case 4:
                            m_weapon.DisplayLaunch(0);
                            break;
                        case 1:
                        case 3:
                            m_weapon.DisplayLaunch(1);
                            break;
                        case 2:
                            m_weapon.DisplayLaunch(2);
                            // launch sounds
                            Effects.PlaySound(m_weapon, m_weapon.Map, 0x4C9);
                            Effects.PlaySound(m_weapon, m_weapon.Map, 0x2B2);

                            // show the projectile moving to the target
                            if (m_target is Mobile)
                            {
                                XmlSiege.SendMovingProjectileEffect(m_weapon, null, animationid, m_weapon.ProjectileLaunchPoint, m_targetloc, 7, 0, false, true, animationhue);
                            }
                            else
                            {
                                XmlSiege.SendMovingProjectileEffect(m_weapon, m_target, animationid, m_weapon.ProjectileLaunchPoint, m_targetloc, 7, 0, false, true, animationhue);
                            }
                            // delayed damage at the target to account for travel distance of the projectile
                            if (!m_Projectile.Deleted)
                            {
                                DelayCall<(Mobile, BaseSiegeWeapon, IEntity, Point3D, Item)>(m_damagedelay, m_weapon.DamageTarget_Callback, (m_from, m_weapon, m_target, m_targetloc, m_Projectile));
                            }

                            break;
                    }

                    // advance to the next step
                    m_weapon.DoTimer(m_from, m_weapon, m_target, m_targetloc, m_Projectile, m_damagedelay, ++m_step);
                }

            }
        }

        private void DisplayLaunch(int frame)
        {
            int[] LaunchIDArray = null;

            switch (Facing)
            {
                case 0:
                    LaunchIDArray = CatapultWestLaunch;
                    break;
                case 1:
                    LaunchIDArray = CatapultNorthLaunch;
                    break;
                case 2:
                    LaunchIDArray = CatapultEastLaunch;
                    break;
                case 3:
                    LaunchIDArray = CatapultSouthLaunch;
                    break;
            }

            if (LaunchIDArray != null && Components != null && frame < LaunchIDArray.Length)
            {
                Components[0].ItemID = LaunchIDArray[frame];
            }
        }


        [Constructable]
        public SiegeCatapult() : this(0)
        {
        }

        [Constructable]
        public SiegeCatapult(int facing)
        {
            // addon the components
            for (int i = 0; i < CatapultNorth.Length; ++i)
            {
                AddComponent(new SiegeComponent(0, Name), 0, 0, 0);
            }

            // assign the facing
            if (facing < 0)
            {
                facing = 3;
            }

            if (facing > 3)
            {
                facing = 0;
            }

            Facing = facing;

            // set the default props
            Weight = 60f;

            // make them siegable by default
            // XmlSiege( hitsmax, resistfire, resistphysical, wood, iron, stone, destroyed itemid 0=distruzione non recuperabile)
            XmlAttach.AttachTo(this, new XmlSiege(450, 260, 260, 20, 30, 0, 0, 0));

            // and draggable
            XmlAttach.AttachTo(this, new XmlDrag() { Distance = -3 });

            // undo the temporary hue indicator that is set when the xmlsiege attachment is added
            Hue = 0;

        }

        public SiegeCatapult(Serial serial)
            : base(serial)
        {
        }

        public override Point3D ProjectileLaunchPoint
        {
            get
            {
                if (Components != null && Components.Count > 0)
                {
                    switch (Facing)
                    {
                        case 0:
                            return new Point3D(CatapultWestXOffset[0] + Location.X - 2, CatapultWestYOffset[0] + Location.Y - 1, Location.Z + 5);
                        case 1:
                            return new Point3D(CatapultNorthXOffset[0] + Location.X - 1, CatapultNorthYOffset[0] + Location.Y - 1, Location.Z + 5);
                        case 2:
                            return new Point3D(CatapultEastXOffset[0] + Location.X - 2, CatapultEastYOffset[0] + Location.Y - 1, Location.Z + 5);
                        case 3:
                            return new Point3D(CatapultSouthXOffset[0] + Location.X, CatapultSouthYOffset[0] + Location.Y - 1, Location.Z + 5);
                    }
                }

                return (Location);
            }
        }

        public override void LaunchProjectile(Mobile from, Item projectile, IEntity target, Point3D targetloc, TimeSpan delay)
        {
            // launch animation and delayed projectile release
            DoTimer(from, this, target, targetloc, projectile, delay, 0);
            // play the launch sound
            Effects.PlaySound(this, Map, 0x531);
        }

        public override void SpecialEffects(Mobile from, Item projectile)
        {
            // launch animation and delayed projectile release
            DoTimer(from, this, null, Location, projectile, TimeSpan.Zero, 0);
            // play the launch sound
            Effects.PlaySound(this, Map, 0x531);
        }

        public override void UpdateDisplay()
        {
            if (Components != null && Components.Count > 2)
            {
                int z = Components[1].Location.Z;

                int[] itemid = null;
                int[] xoffset = null;
                int[] yoffset = null;

                switch (Facing)
                {
                    case 0: // West
                        itemid = CatapultWest;
                        xoffset = CatapultWestXOffset;
                        yoffset = CatapultWestYOffset;
                        break;
                    case 1: // North
                        itemid = CatapultNorth;
                        xoffset = CatapultNorthXOffset;
                        yoffset = CatapultNorthYOffset;
                        break;
                    case 2: // East
                        itemid = CatapultEast;
                        xoffset = CatapultEastXOffset;
                        yoffset = CatapultEastYOffset;
                        break;
                    case 3: // South
                        itemid = CatapultSouth;
                        xoffset = CatapultSouthXOffset;
                        yoffset = CatapultSouthYOffset;
                        break;
                }

                if (itemid != null && xoffset != null && yoffset != null && Components.Count == itemid.Length)
                {
                    for (int i = 0; i < Components.Count; ++i)
                    {
                        Components[i].ItemID = itemid[i];
                        Point3D newoffset = new Point3D(xoffset[i], yoffset[i], 0);
                        Components[i].Offset = newoffset;
                        Components[i].Location = new Point3D(newoffset.X + X, newoffset.Y + Y, z);
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            if (version < 1)
            {
                Weight = 60f;
                Name = null;
            }
        }
    }
}
