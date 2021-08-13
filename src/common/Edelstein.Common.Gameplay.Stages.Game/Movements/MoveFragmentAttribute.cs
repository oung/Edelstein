﻿namespace Edelstein.Common.Gameplay.Stages.Game.Movements
{
    public enum MoveFragmentAttribute
    {
        Normal = 0x0,
        Jump = 0x1,
        Impact = 0x2,
        Immediate = 0x3,
        Teleport = 0x4,
        HangOnBack = 0x5,
        Assaulter = 0x6,
        Assassination = 0x7,
        Rush = 0x8,
        StatChange = 0x9,
        SitDown = 0xA,
        StartFallDown = 0xB,
        FallDown = 0xC,
        StartWings = 0xD,
        Wings = 0xE,
        AranAdjust = 0xF,
        MobToss = 0x10,
        FlyingBlock = 0x11,
        DashSlide = 0x12,
        BmageAdjust = 0x13,
        FlashJump = 0x14,
        RocketBooster = 0x15,
        BackStepShot = 0x16,
        MobPowerKnockBack = 0x17,
        VerticalJump = 0x18,
        CustomImpact = 0x19,
        CombatStep = 0x1A,
        Hit = 0x1B,
        TimeBombAttack = 0x1C,
        SnowballTouch = 0x1D,
        BuffZoneEffect = 0x1E,
        MobLadder = 0x1F,
        MobRightAngle = 0x20,
        MobStopNodeStart = 0x21,
        MobBeforeNode = 0x22,
        MobAttackRush = 0x23,
        MobAttackRushStop = 0x24
    }
}