﻿namespace Edelstein.Common.Gameplay.Packets;

public enum PacketSendOperations : short
{
    CheckPasswordResult = 0,
    WorldInformation = 1,
    LatestConnectedWorld = 2,
    AliveReq = 18,
    PrivateServerPacket = 23,

    SetField = 428,

    UserEnterField = 516,
    UserLeaveField = 517
}
