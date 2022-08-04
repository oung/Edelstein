﻿using Edelstein.Common.Gameplay.Packets;
using Edelstein.Common.Gameplay.Stages.Game.Objects.User.Messages;
using Edelstein.Protocol.Gameplay.Stages.Game;
using Edelstein.Protocol.Gameplay.Stages.Game.Objects.User;
using Edelstein.Protocol.Gameplay.Stages.Game.Objects.User.Messages;
using Edelstein.Protocol.Util.Buffers.Packets;
using Edelstein.Protocol.Util.Pipelines;

namespace Edelstein.Common.Gameplay.Stages.Game.Objects.User.Handlers;

public class UserTransferChannelRequestUserHandler : AbstractFieldUserHandler
{
    private readonly IPipeline<IUserTransferChannelRequest> _pipeline;

    public UserTransferChannelRequestUserHandler(IPipeline<IUserTransferChannelRequest> pipeline) => _pipeline = pipeline;

    public override short Operation => (short)PacketRecvOperations.UserTransferChannelRequest;

    public override bool Check(IGameStageUser user) => base.Check(user) && !user.IsMigrating;

    protected override Task Handle(IFieldUser user, IPacketReader reader)
    {
        var message = new UserTransferChannelRequest(
            user,
            reader.ReadByte()
        );

        return _pipeline.Process(message);
    }
}