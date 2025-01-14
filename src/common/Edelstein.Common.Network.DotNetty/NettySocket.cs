﻿using System.Net;
using DotNetty.Transport.Channels;
using Edelstein.Protocol.Network;
using Edelstein.Protocol.Util.Buffers.Packets;

namespace Edelstein.Common.Network.DotNetty;

public class NettySocket : ISocket
{
    private readonly IChannel _channel;

    public NettySocket(IChannel channel, uint seqSend, uint seqRecv, bool isDataEncrypted = true)
    {
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        SeqSend = seqSend;
        SeqRecv = seqRecv;
        IsDataEncrypted = isDataEncrypted;
    }

    public string ID => _channel.Id.AsLongText();

    public EndPoint AddressLocal => _channel.LocalAddress;
    public EndPoint AddressRemote => _channel.RemoteAddress;

    public uint SeqSend { get; set; }
    public uint SeqRecv { get; set; }

    public bool IsDataEncrypted { get; }

    public DateTime LastAliveSent { get; set; }
    public DateTime LastAliveRecv { get; set; }

    public Task Close() => _channel.DisconnectAsync();

    public async Task Dispatch(IPacket packet)
    {
        if (_channel.IsWritable)
            await _channel.WriteAndFlushAsync(packet);
    }
}
