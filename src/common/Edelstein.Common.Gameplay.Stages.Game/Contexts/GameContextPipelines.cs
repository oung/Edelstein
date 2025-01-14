﻿using Edelstein.Protocol.Gameplay.Stages.Contracts.Pipelines;
using Edelstein.Protocol.Gameplay.Stages.Game;
using Edelstein.Protocol.Gameplay.Stages.Game.Contexts;
using Edelstein.Protocol.Util.Pipelines;

namespace Edelstein.Common.Gameplay.Stages.Game.Contexts;

public record GameContextPipelines(
    IPipeline<ISocketOnMigrateIn<IGameStageUser>> SocketOnMigrateIn,
    IPipeline<ISocketOnMigrateOut<IGameStageUser>> SocketOnMigrateOut,
    IPipeline<ISocketOnAliveAck<IGameStageUser>> SocketOnAliveAck,
    IPipeline<ISocketOnPacket<IGameStageUser>> SocketOnPacket,
    IPipeline<ISocketOnException<IGameStageUser>> SocketOnException,
    IPipeline<ISocketOnDisconnect<IGameStageUser>> SocketOnDisconnect
) : IGameContextPipelines;
