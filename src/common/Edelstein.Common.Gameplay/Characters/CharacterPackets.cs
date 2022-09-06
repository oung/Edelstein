﻿using Edelstein.Common.Gameplay.Inventories.Items;
using Edelstein.Protocol.Gameplay.Characters;
using Edelstein.Protocol.Gameplay.Inventories;
using Edelstein.Protocol.Gameplay.Inventories.Items;
using Edelstein.Protocol.Util.Buffers.Packets;

namespace Edelstein.Common.Gameplay.Characters;

public static class CharacterPackets
{
    public static void WriteCharacterData(
        this IPacketWriter writer,
        ICharacter character,
        CharacterFlags flags = CharacterFlags.All
    )
    {
        writer.WriteLong((long)flags);
        writer.WriteByte(0);
        writer.WriteByte(0);

        if (flags.HasFlag(CharacterFlags.Character))
        {
            writer.WriteCharacterStats(character);

            writer.WriteByte(250); // nFriendMax
            writer.WriteBool(false);
        }

        if (flags.HasFlag(CharacterFlags.Money)) writer.WriteInt(character.Money);

        if (flags.HasFlag(CharacterFlags.InventorySize))
        {
            if (flags.HasFlag(CharacterFlags.ItemSlotEquip))
                writer.WriteByte((byte)character.Inventories[ItemInventoryType.Equip].SlotMax);
            if (flags.HasFlag(CharacterFlags.ItemSlotConsume))
                writer.WriteByte((byte)character.Inventories[ItemInventoryType.Consume].SlotMax);
            if (flags.HasFlag(CharacterFlags.ItemSlotInstall))
                writer.WriteByte((byte)character.Inventories[ItemInventoryType.Install].SlotMax);
            if (flags.HasFlag(CharacterFlags.ItemSlotEtc))
                writer.WriteByte((byte)character.Inventories[ItemInventoryType.Etc].SlotMax);
            if (flags.HasFlag(CharacterFlags.ItemSlotCash))
                writer.WriteByte((byte)character.Inventories[ItemInventoryType.Cash].SlotMax);
        }

        if (flags.HasFlag(CharacterFlags.AdminShopCount))
        {
            writer.WriteInt(0);
            writer.WriteInt(0);
        }

        if (flags.HasFlag(CharacterFlags.ItemSlotEquip))
        {
            var inventory = character.Inventories[ItemInventoryType.Equip].Items;
            var equip = inventory.Where(kv => kv.Key >= 0);
            var equipped = inventory.Where(kv => kv.Key is >= -100 and < 0);
            var equipped2 = inventory.Where(kv => kv.Key is >= -1000 and < -100);
            var dragon = inventory.Where(kv => kv.Key is >= -1100 and < -1000);
            var mechanic = inventory.Where(kv => kv.Key is >= -1200 and < -1100);

            foreach (var items in new[] { equipped, equipped2, equip, dragon, mechanic })
            {
                foreach (var kv in items)
                {
                    writer.WriteShort((short)Math.Abs(kv.Key % 100));
                    writer.WriteItemData(kv.Value);
                }

                writer.WriteShort(0);
            }
        }

        foreach (var t in new List<(CharacterFlags, ItemInventoryType)>
                     {
                         (CharacterFlags.ItemSlotConsume, ItemInventoryType.Consume),
                         (CharacterFlags.ItemSlotInstall, ItemInventoryType.Install),
                         (CharacterFlags.ItemSlotEtc, ItemInventoryType.Etc),
                         (CharacterFlags.ItemSlotCash, ItemInventoryType.Cash)
                     }
                     .Where(t => flags.HasFlag(t.Item1)))
        {
            var items = character.Inventories[t.Item2].Items;

            foreach (var kv in items)
            {
                writer.WriteShort(kv.Key);
                writer.WriteItemData(kv.Value);
            }

            writer.WriteByte(0);
        }

        if (flags.HasFlag(CharacterFlags.SkillRecord)) writer.WriteShort(0);

        if (flags.HasFlag(CharacterFlags.SkillCooltime)) writer.WriteShort(0);

        if (flags.HasFlag(CharacterFlags.QuestRecord)) writer.WriteShort(0);

        if (flags.HasFlag(CharacterFlags.QuestComplete)) writer.WriteShort(0);

        if (flags.HasFlag(CharacterFlags.MinigameRecord)) writer.WriteShort(0);

        if (flags.HasFlag(CharacterFlags.CoupleRecord))
        {
            writer.WriteShort(0); // Couple
            writer.WriteShort(0); // Friend
            writer.WriteShort(0); // Marriage
        }

        if (flags.HasFlag(CharacterFlags.MapTransfer))
        {
            for (var i = 0; i < 5; i++) writer.WriteInt(0);
            for (var i = 0; i < 10; i++) writer.WriteInt(0);
        }

        if (flags.HasFlag(CharacterFlags.NewYearCard)) writer.WriteShort(0);

        if (flags.HasFlag(CharacterFlags.QuestRecordEx)) writer.WriteShort(0);

        if (flags.HasFlag(CharacterFlags.WildHunterInfo))
            if (character.Job / 100 == 33)
            {
                writer.WriteByte(0);
                for (var i = 0; i < 5; i++) writer.WriteInt(0);
            }

        if (flags.HasFlag(CharacterFlags.QuestCompleteOld)) writer.WriteShort(0);

        if (flags.HasFlag(CharacterFlags.VisitorLog)) writer.WriteShort(0);
    }

    public static void WriteCharacterStats(
        this IPacketWriter writer,
        ICharacter character
    )
    {
        writer.WriteInt(character.ID);
        writer.WriteInt(character.ID);
        writer.WriteInt(0);
        writer.WriteString(character.Name, 13);

        writer.WriteByte(character.Gender);
        writer.WriteByte(character.Skin);
        writer.WriteInt(character.Face);
        writer.WriteInt(character.Hair);
        writer.WriteByte(0);
        writer.WriteByte(0);
        writer.WriteByte(0);

        writer.WriteByte(character.Level);
        writer.WriteShort(character.Job);
        writer.WriteShort(character.STR);
        writer.WriteShort(character.DEX);
        writer.WriteShort(character.INT);
        writer.WriteShort(character.LUK);
        writer.WriteInt(character.HP);
        writer.WriteInt(character.MaxHP);
        writer.WriteInt(character.MP);
        writer.WriteInt(character.MaxMP);

        writer.WriteShort(character.AP);
        writer.WriteByte(0); // TODO: extendSP

        writer.WriteLong(character.EXP);
        writer.WriteInt(character.POP);

        writer.WriteInt(0);
        writer.WriteInt(character.TempEXP);
        writer.WriteInt(character.FieldID);
        writer.WriteByte(character.FieldPortal);
        writer.WriteInt(character.PlayTime);
        writer.WriteShort(character.SubJob);

        writer.WriteByte(0);
        writer.WriteInt(0);
        writer.WriteInt(0);
        writer.WriteInt(0);
        writer.WriteInt(0);
        writer.WriteInt(0);
        writer.WriteInt(0);
        writer.WriteInt(0);

        // NonCombatStatDay
        writer.WriteShort(0);
        writer.WriteShort(0);
        writer.WriteShort(0);
        writer.WriteShort(0);
        writer.WriteShort(0);
        writer.WriteShort(0);
        writer.WriteByte(0);
        writer.WriteLong(0);

        writer.WriteInt(0);
        writer.WriteByte(0);
        writer.WriteInt(0);
        writer.WriteByte(2);
        writer.WriteByte(0);

        writer.WriteInt(0);
        writer.WriteByte(0);
        writer.WriteLong(0);
        writer.WriteInt(0);
        writer.WriteByte(0);

        for (var i = 0; i < 9; i++)
        {
            writer.WriteInt(0);
            writer.WriteByte(0);
            writer.WriteInt(0);
        }

        writer.WriteLong(0);
        writer.WriteBool(false);
    }

    public static void WriteCharacterLooks(
        this IPacketWriter writer,
        ICharacter character
    )
    {
        writer.WriteByte(character.Gender);
        writer.WriteByte(character.Skin);
        writer.WriteInt(character.Face);
        writer.WriteInt(character.Job);
        writer.WriteBool(false);
        writer.WriteInt(character.Hair);

        var inventory = character.Inventories[ItemInventoryType.Equip];
        var equipped = inventory.Items
            .Where(kv => kv.Key < 0)
            .Select(kv => Tuple.Create(Math.Abs(kv.Key), kv.Value))
            .ToList();
        var stickers = equipped
            .Where(t => t.Item1 > 100)
            .Select(t => Tuple.Create(t.Item1 - 100, t.Item2))
            .ToDictionary(
                kv => kv.Item1,
                kv => kv.Item2
            );
        var hidden = new Dictionary<short, IItemSlot>();

        foreach (var tuple in equipped.Where(t => t.Item1 < 100))
            if (!stickers.ContainsKey(tuple.Item1))
                stickers[tuple.Item1] = tuple.Item2;
            else
                hidden[tuple.Item1] = tuple.Item2;

        foreach (var kv in stickers)
        {
            writer.WriteByte((byte)kv.Key);
            writer.WriteInt(kv.Value.ID);
        }

        writer.WriteByte(0xFF);

        foreach (var kv in hidden)
        {
            writer.WriteByte((byte)kv.Key);
            writer.WriteInt(kv.Value.ID);
        }

        writer.WriteByte(0xFF);

        writer.WriteByte(0xFF);

        var item = character.Inventories[ItemInventoryType.Equip].Items;

        writer.WriteInt(
            item.ContainsKey(-111)
                ? item[-111].ID
                : 0
        );
        writer.WriteInt(
            item.ContainsKey(-11)
                ? item[-11].ID
                : 0
        );
        writer.WriteInt(0);
        writer.WriteBool(false);

        for (var i = 0; i < 3; i++)
            writer.WriteInt(0);

        writer.WriteByte(0);
        writer.WriteByte(0);
    }
}
