﻿using Edelstein.Protocol.Gameplay.Stages;
using Edelstein.Protocol.Gameplay.Stages.Contracts;
using Edelstein.Protocol.Util.Buffers.Packets;

namespace Edelstein.Common.Gameplay.Stages.Contracts;

public record SocketOnPacket<TStageUser>(
    TStageUser User,
    IPacket Packet
) : ISocketOnPacket<TStageUser> where TStageUser : IStageUser<TStageUser>;