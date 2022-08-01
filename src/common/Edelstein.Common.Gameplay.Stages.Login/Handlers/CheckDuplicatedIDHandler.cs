﻿using Edelstein.Common.Gameplay.Packets;
using Edelstein.Common.Gameplay.Stages.Login.Messages;
using Edelstein.Protocol.Gameplay.Stages.Login;
using Edelstein.Protocol.Gameplay.Stages.Login.Messages;
using Edelstein.Protocol.Util.Buffers.Bytes;
using Edelstein.Protocol.Util.Pipelines;

namespace Edelstein.Common.Gameplay.Stages.Login.Handlers;

public class CheckDuplicatedIDHandler : AbstractLoginPacketHandler
{
    private readonly IPipeline<ICheckDuplicatedID> _pipeline;

    public CheckDuplicatedIDHandler(IPipeline<ICheckDuplicatedID> pipeline) => _pipeline = pipeline;

    public override short Operation => (short)PacketRecvOperations.CheckDuplicatedID;

    public override bool Check(ILoginStageUser user) => user.State == LoginState.SelectCharacter;

    public override Task Handle(ILoginStageUser user, IByteReader reader)
    {
        var message = new CheckDuplicatedID(
            user,
            reader.ReadString()
        );

        return _pipeline.Process(message);
    }
}