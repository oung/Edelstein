﻿using Edelstein.Common.Gameplay.Stages.Actions;
using Edelstein.Common.Gameplay.Stages.Login;
using Edelstein.Common.Gameplay.Stages.Login.Contexts;
using Edelstein.Common.Network.DotNetty.Transports;
using Edelstein.Common.Util.Pipelines;
using Edelstein.Protocol.Gameplay.Stages.Login;
using Edelstein.Protocol.Gameplay.Stages.Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Edelstein.Daemon.Server;

public class ProgramHost : IHostedService
{
    private readonly ILogger<ProgramHost> _logger;

    public ProgramHost(ILogger<ProgramHost> logger) => _logger = logger;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var pipelines = new LoginContextPipelines(
            new Pipeline<IStageUserOnPacket<ILoginStageUser>>(
                new StageUserOnPacketAction<ILoginStageUser>()
            ),
            new Pipeline<IStageUserOnException<ILoginStageUser>>(),
            new Pipeline<IStageUserOnDisconnect<ILoginStageUser>>()
        );
        var context = new LoginContext(pipelines);
        var initializer = new LoginStageUserInitializer(context);
        var acceptor = new NettyTransportAcceptor(initializer, 95, "1", 8);

        acceptor.Accept("127.0.0.1", 8484).Wait(cancellationToken);

        _logger.LogInformation("Host has started");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Host has stopped");
        return Task.CompletedTask;
    }
}
