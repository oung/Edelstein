﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edelstein.Protocol.Interop;
using Edelstein.Protocol.Interop.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Edelstein.Common.Interop
{
    public class ServerRegistryService : IServerRegistryService
    {
        private readonly ILogger _logger;
        private readonly ServerRegistryRepository _repository;
        private readonly ISessionRegistryService _sessionRegistryService;
        // TODO: Messaging to ensure synchronization across machines

        public ServerRegistryService(
            ISessionRegistryService sessionRegistryService,
            ILogger<ServerRegistryService> logger = null
        )
        {
            _repository = new ServerRegistryRepository();
            _sessionRegistryService = sessionRegistryService;
            _logger = logger ?? new NullLogger<ServerRegistryService>();
        }

        public async Task<ServiceRegistryResponse> RegisterServer(RegisterServerRequest request)
        {
            var response = new ServiceRegistryResponse { Result = ServiceRegistryResult.Ok };

            if (await _repository.Retrieve(request.Server.Id) == null)
            {
                await _repository.Insert(new ServerRegistryEntry(request.Server, DateTime.UtcNow));
                _logger.LogInformation($"Registered server {request.Server.Id} to the registry");
            }
            else response.Result = ServiceRegistryResult.Failed;

            return response;
        }

        public async Task<ServiceRegistryResponse> DeregisterServer(DeregisterServerRequest request)
        {
            var response = new ServiceRegistryResponse { Result = ServiceRegistryResult.Ok };

            if (await _repository.Retrieve(request.Id) != null)
            {
                await _repository.Delete(request.Id);
                _logger.LogInformation($"Deregistered server {request.Id} from the registry");
            }
            else response.Result = ServiceRegistryResult.Failed;

            return response;
        }

        public async Task<ServiceRegistryResponse> UpdateServer(UpdateServerRequest request)
        {
            var response = new ServiceRegistryResponse { Result = ServiceRegistryResult.Ok };
            var entry = await _repository.Retrieve(request.Server.Id);

            if (entry != null)
            {
                entry.Server = request.Server;
                entry.LastUpdate = DateTime.UtcNow;
                await _repository.Update(entry);
            }
            else response.Result = ServiceRegistryResult.Failed;

            return response;
        }

        public async Task<DescribeServerResponse> DescribeServer(DescribeServerRequest request)
        {
            var response = new DescribeServerResponse { Result = ServiceRegistryResult.Ok };
            var entry = await _repository.Retrieve(request.Id);

            if (entry != null) response.Server = entry.Server;
            else response.Result = ServiceRegistryResult.Failed;

            return response;
        }

        public async Task<DescribeServersResponse> DescribeServers(DescribeServersRequest request)
        {
            var response = new DescribeServersResponse { Result = ServiceRegistryResult.Ok };
            var servers = (await _repository.RetrieveAll())
                .Where(e => request.Tags.All(t => e.Server.Tags.ContainsKey(t.Key) && e.Server.Tags[t.Key] == t.Value))
                .Select(e => e.Server)
                .ToList();

            response.Servers.AddRange(servers);

            return response;
        }

        public async Task<DispatchResponse> Dispatch(DispatchRequest request)
        {
            var response = new DispatchResponse { Result = DispatchResult.Ok };
            var dispatch = new DispatchObject { Packet = request.Packet };

            await Task.WhenAll((await _repository.RetrieveAll()).Select(e => e.Dispatch.Writer.WriteAsync(dispatch).AsTask()));

            return response;
        }

        public async Task<DispatchResponse> DispatchToServer(DispatchToServerRequest request)
        {
            var response = new DispatchResponse { Result = DispatchResult.Ok };
            var dispatch = new DispatchObject { Packet = request.Packet };
            var entry = await _repository.Retrieve(request.Server);

            if (entry != null)
                await entry.Dispatch.Writer.WriteAsync(dispatch);
            else response.Result = DispatchResult.Failed;

            return response;
        }
        public async Task<DispatchResponse> DispatchToServers(DispatchToServersRequest request)
        {
            var response = new DispatchResponse { Result = DispatchResult.Ok };
            var dispatch = new DispatchObject { Packet = request.Packet };
            var targets = (await _repository.RetrieveAll())
                .Where(e => request.Tags.All(t => e.Server.Tags.ContainsKey(t.Key) && e.Server.Tags[t.Key] == t.Value))
                .ToList();

            await Task.WhenAll(targets.Select(e => e.Dispatch.Writer.WriteAsync(dispatch).AsTask()));

            if (targets.Count == 0) response.Result = DispatchResult.Failed;

            return response;
        }

        public async Task<DispatchResponse> DispatchToCharacter(DispatchToCharacterRequest request)
        {
            var response = new DispatchResponse() { Result = DispatchResult.Ok };
            var dispatch = new DispatchObject() { Packet = request.Packet };
            var description = await _sessionRegistryService.DescribeSessionByCharacter(new DescribeSessionByCharacterRequest { Character = request.Target });

            dispatch.Targets.Add(request.Target);

            if (description.Result == SessionRegistryResult.Ok)
            {
                var server = description.Session.Server;
                var entry = await _repository.Retrieve(server);

                if (entry != null) await entry.Dispatch.Writer.WriteAsync(dispatch);
                else response.Result = DispatchResult.Failed;
            }
            else response.Result = DispatchResult.Failed;

            return response;
        }

        public async Task<DispatchResponse> DispatchToCharacters(DispatchToCharactersRequest request)
        {
            var response = new DispatchResponse() { Result = DispatchResult.Ok };
            var dispatch = new DispatchObject() { Packet = request.Packet };
            var targets = (await _repository.RetrieveAll()).ToList();

            dispatch.Targets.AddRange(request.Targets);

            await Task.WhenAll(targets.Select(e => e.Dispatch.Writer.WriteAsync(dispatch).AsTask()));

            return response;
        }

        public async IAsyncEnumerable<DispatchObject> SubscribeDispatch(DispatchSubscription request)
        {
            var entry = await _repository.Retrieve(request.Server);

            if (entry != null)
            {
                await foreach (var dispatch in entry.Dispatch.Reader.ReadAllAsync())
                    yield return dispatch;
            }

            yield break;
        }
    }
}
