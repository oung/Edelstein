﻿using System;
using Edelstein.Protocol.Gameplay.Users;
using Edelstein.Protocol.Gameplay.Users.Stats.Modify;
using Edelstein.Protocol.Network;
using Edelstein.Protocol.Network.Utils;

namespace Edelstein.Common.Gameplay.Users.Stats.Modify
{
    public class ModifyStatContext : IModifyStatContext, IPacketWritable
    {
        public ModifyStatType Flag { get; set; }

        public byte Skin
        {
            get => _character.Skin;
            set
            {
                Flag |= ModifyStatType.Skin;
                _character.Skin = value;
            }
        }

        public int Face
        {
            get => _character.Face;
            set
            {
                Flag |= ModifyStatType.Face;
                _character.Face = value;
            }
        }

        public int Hair
        {
            get => _character.Hair;
            set
            {
                Flag |= ModifyStatType.Hair;
                _character.Hair = value;
            }
        }

        public long Pet1
        {
            get => _character.Pets[0];
            set
            {
                Flag |= ModifyStatType.Pet;
                _character.Pets[0] = value;
            }
        }

        public long Pet2
        {
            get => _character.Pets[1];
            set
            {
                Flag |= ModifyStatType.Pet2;
                _character.Pets[1] = value;
            }
        }

        public long Pet3
        {
            get => _character.Pets[2];
            set
            {
                Flag |= ModifyStatType.Pet3;
                _character.Pets[2] = value;
            }
        }

        public byte Level
        {
            get => _character.Level;
            set
            {
                Flag |= ModifyStatType.Level;
                _character.Level = value;
            }
        }

        public short Job
        {
            get => _character.Job;
            set
            {
                Flag |= ModifyStatType.Job;
                _character.Job = value;
            }
        }

        public short STR
        {
            get => _character.STR;
            set
            {
                Flag |= ModifyStatType.STR;
                _character.STR = value;
            }
        }

        public short DEX
        {
            get => _character.DEX;
            set
            {
                Flag |= ModifyStatType.DEX;
                _character.DEX = value;
            }
        }

        public short INT
        {
            get => _character.INT;
            set
            {
                Flag |= ModifyStatType.INT;
                _character.INT = value;
            }
        }

        public short LUK
        {
            get => _character.LUK;
            set
            {
                Flag |= ModifyStatType.LUK;
                _character.LUK = value;
            }
        }

        public int HP
        {
            get => _character.HP;
            set
            {
                Flag |= ModifyStatType.HP;
                _character.HP = value;
            }
        }

        public int MaxHP
        {
            get => _character.MaxHP;
            set
            {
                Flag |= ModifyStatType.MaxHP;
                _character.MaxHP = value;
            }
        }

        public int MP
        {
            get => _character.MP;
            set
            {
                Flag |= ModifyStatType.MP;
                _character.MP = value;
            }
        }

        public int MaxMP
        {
            get => _character.MaxMP;
            set
            {
                Flag |= ModifyStatType.MaxMP;
                _character.MaxMP = value;
            }
        }

        public short AP
        {
            get => _character.AP;
            set
            {
                Flag |= ModifyStatType.AP;
                _character.AP = value;
            }
        }

        public short SP
        {
            get => _character.SP;
            set
            {
                Flag |= ModifyStatType.SP;
                _character.SP = value;
            }
        }

        public int EXP
        {
            get => _character.EXP;
            set
            {
                // TODO: level up logic, max exp
                Flag |= ModifyStatType.EXP;
                _character.EXP = value;
            }
        }

        public short POP
        {
            get => _character.POP;
            set
            {
                Flag |= ModifyStatType.POP;
                _character.POP = value;
            }
        }

        public int Money
        {
            get => _character.Money;
            set
            {
                Flag |= ModifyStatType.Money;
                _character.Money = value;
            }
        }

        public int TempEXP
        {
            get => _character.TempEXP;
            set
            {
                Flag |= ModifyStatType.TempEXP;
                _character.TempEXP = value;
            }
        }

        private readonly Character _character;

        public ModifyStatContext(Character character)
            => _character = character;

        public byte GetExtendSP(byte jobLevel)
            => (byte)(_character.ExtendSP.ContainsKey(jobLevel) ? _character.ExtendSP[jobLevel] : 0);

        public void SetExtendSP(byte jobLevel, byte point)
        {
            Flag |= ModifyStatType.SP;
            _character.ExtendSP[jobLevel] = point;
        }

        public void IncExtendSP(byte jobLevel, byte point)
            => SetExtendSP(jobLevel, (byte)(GetExtendSP(jobLevel) + point));

        public void LevelUp()
        {
            if (Level >= 200) return;

            Level++;

            var random = new Random();
            var hpBonus = random.Next(10, 16);
            var mpBonus = random.Next(10, 12);

            MaxHP = Math.Min(999999, MaxHP + hpBonus);
            MaxMP = Math.Min(999999, MaxMP + mpBonus);

            if (HP < MaxHP) HP = MaxHP;
            if (MP < MaxMP) MP = MaxMP;

            if (Level <= 10) return;
            // TODO: SP Handling

            AP += 5;
        }

        public void WriteToPacket(IPacketWriter writer)
        {
            writer.WriteInt((int)Flag);

            if ((Flag & ModifyStatType.Skin) != 0) writer.WriteByte(Skin);
            if ((Flag & ModifyStatType.Face) != 0) writer.WriteInt(Face);
            if ((Flag & ModifyStatType.Hair) != 0) writer.WriteInt(Hair);

            if ((Flag & ModifyStatType.Pet) != 0) writer.WriteLong(Pet1);
            if ((Flag & ModifyStatType.Pet2) != 0) writer.WriteLong(Pet2);
            if ((Flag & ModifyStatType.Pet3) != 0) writer.WriteLong(Pet3);

            if ((Flag & ModifyStatType.Level) != 0) writer.WriteByte(Level);
            if ((Flag & ModifyStatType.Job) != 0) writer.WriteShort(Job);
            if ((Flag & ModifyStatType.STR) != 0) writer.WriteShort(STR);
            if ((Flag & ModifyStatType.DEX) != 0) writer.WriteShort(DEX);
            if ((Flag & ModifyStatType.INT) != 0) writer.WriteShort(INT);
            if ((Flag & ModifyStatType.LUK) != 0) writer.WriteShort(LUK);

            if ((Flag & ModifyStatType.HP) != 0) writer.WriteInt(HP);
            if ((Flag & ModifyStatType.MaxHP) != 0) writer.WriteInt(MaxHP);
            if ((Flag & ModifyStatType.MP) != 0) writer.WriteInt(MP);
            if ((Flag & ModifyStatType.MaxMP) != 0) writer.WriteInt(MaxMP);

            if ((Flag & ModifyStatType.AP) != 0) writer.WriteShort(AP);
            if ((Flag & ModifyStatType.SP) != 0)
            {
                if (_character.Job / 1000 == 3 || _character.Job / 100 == 22 || _character.Job == 2001) // TODO: constants
                    writer.WriteShort(SP);
                else writer.WriteCharacterExtendSP(_character);
            }

            if ((Flag & ModifyStatType.EXP) != 0) writer.WriteInt(EXP);
            if ((Flag & ModifyStatType.POP) != 0) writer.WriteShort(POP);

            if ((Flag & ModifyStatType.Money) != 0) writer.WriteInt(Money);
            if ((Flag & ModifyStatType.TempEXP) != 0) writer.WriteInt(TempEXP);
        }
    }
}
