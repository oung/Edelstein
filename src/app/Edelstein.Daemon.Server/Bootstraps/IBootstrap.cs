﻿namespace Edelstein.Daemon.Server.Bootstraps;

public interface IBootstrap
{
    Task Start();
    Task Stop();
}