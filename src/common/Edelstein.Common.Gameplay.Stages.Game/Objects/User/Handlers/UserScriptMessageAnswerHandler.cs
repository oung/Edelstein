﻿using Edelstein.Common.Gameplay.Packets;
using Edelstein.Common.Gameplay.Stages.Game.Conversations;
using Edelstein.Protocol.Gameplay.Stages.Game.Conversations.Messages;
using Edelstein.Protocol.Gameplay.Stages.Game.Objects.User;
using Edelstein.Protocol.Util.Buffers.Packets;

namespace Edelstein.Common.Gameplay.Stages.Game.Objects.User.Handlers;

public class UserScriptMessageAnswerHandler : AbstractFieldUserHandler
{
    public override short Operation => (short)PacketRecvOperations.UserScriptMessageAnswer;

    protected override async Task Handle(IFieldUser user, IPacketReader reader)
    {
        if (!user.IsConversing) return;
        if (user.Conversation == null) return;

        var type = (ConversationMessageType)reader.ReadByte();

        if (type is ConversationMessageType.AskQuiz or ConversationMessageType.AskSpeedQuiz)
        {
            await user.Conversation.Respond(
                new ConversationMessageResponse<string>(type, reader.ReadString())
            );
            return;
        }

        var answer = reader.ReadByte();

        if (
            (
                type != ConversationMessageType.Say &&
                type != ConversationMessageType.AskYesNo &&
                type != ConversationMessageType.AskAccept &&
                answer == byte.MinValue
            ) ||
            (
                type is
                    ConversationMessageType.Say or
                    ConversationMessageType.AskYesNo or
                    ConversationMessageType.AskAccept &&
                answer == byte.MaxValue
            )
        )
        {
            await user.EndConversation();
            return;
        }

        switch (type)
        {
            case ConversationMessageType.AskText:
            case ConversationMessageType.AskBoxText:
                await user.Conversation.Respond(
                    new ConversationMessageResponse<string>(type, reader.ReadString())
                );
                break;
            case ConversationMessageType.AskNumber:
            case ConversationMessageType.AskMenu:
            case ConversationMessageType.AskSlideMenu:
                await user.Conversation.Respond(
                    new ConversationMessageResponse<int>(type, reader.ReadInt())
                );
                break;
            case ConversationMessageType.AskAvatar:
            case ConversationMessageType.AskMemberShopAvatar:
                await user.Conversation.Respond(
                    new ConversationMessageResponse<byte>(type, reader.ReadByte())
                );
                break;
            case ConversationMessageType.AskYesNo:
            case ConversationMessageType.AskAccept:
                await user.Conversation.Respond(
                    new ConversationMessageResponse<bool>(type, Convert.ToBoolean(answer))
                );
                break;
            default:
                await user.Conversation.Respond(
                    new ConversationMessageResponse<byte>(type, answer)
                );
                break;
        }
    }
}
