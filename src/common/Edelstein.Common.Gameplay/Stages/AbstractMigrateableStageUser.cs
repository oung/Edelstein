﻿using System;
using System.Net;
using System.Threading.Tasks;
using Edelstein.Common.Gameplay.Handling;
using Edelstein.Protocol.Gameplay.Stages;
using Edelstein.Protocol.Interop.Contracts;
using Edelstein.Protocol.Network;
using Edelstein.Protocol.Network.Transport;

namespace Edelstein.Common.Gameplay.Stages
{
    public abstract class AbstractMigrateableStageUser<TStage, TUser> : AbstractStageUser<TStage, TUser>, IMigrateableStageUser<TStage, TUser>
        where TStage : AbstractMigrateableStage<TStage, TUser>
        where TUser : AbstractMigrateableStageUser<TStage, TUser>
    {
        public static TimeSpan SessionDisconnectDuration = TimeSpan.FromMinutes(1);
        public static TimeSpan SessionUpdateDuration = TimeSpan.FromSeconds(30);

        public long Key { get; set; }
        public bool IsMigrating { get; set; }
        public bool IsLoggingIn { get; set; }

        private DateTime LastSentHeartbeatDate { get; set; }
        private DateTime LastRecvHeartbeatDate { get; set; }

        protected AbstractMigrateableStageUser(ISocket socket) : base(socket) { }

        public override async Task Update()
        {
            if (Account != null)
                await Stage.AccountRepository.Update(Account);
            if (AccountWorld != null)
                await Stage.AccountWorldRepository.Update(AccountWorld);
            if (Character != null)
                await Stage.CharacterRepository.Update(Character);
        }

        public override async Task OnDisconnect()
        {
            if (!IsMigrating)
            {
                await Update();

                if (Account != null)
                {
                    var session = new SessionObject
                    {
                        Account = Account.ID,
                        State = SessionState.Offline
                    };

                    await Stage.SessionRegistry.UpdateSession(new UpdateSessionRequest { Session = session });
                }
            }
            await base.OnDisconnect();
        }

        public async Task<bool> MigrateIn(int character, long key)
        {
            var claim = await Stage.MigrationRegistryService.Claim(new ClaimMigrationRequest
            {
                Character = character,
                Key = key,
                Server = Stage.ID
            });

            if (claim.Result != MigrationRegistryResult.Ok) return false;

            Character = await Stage.CharacterRepository.Retrieve(character);
            AccountWorld = await Stage.AccountWorldRepository.Retrieve(Character.AccountWorldID);
            Account = await Stage.AccountRepository.Retrieve(AccountWorld.AccountID);

            Key = claim.Migration.Key;

            var session = new SessionObject
            {
                Account = Account.ID,
                Character = Character.ID,
                Server = Stage.ID,
                State = SessionState.LoggedIn
            };

            await Stage.SessionRegistry.UpdateSession(new UpdateSessionRequest { Session = session });
            return true;
        }

        public async Task<bool> MigrateTo(string server)
        {
            var describe = await Stage.ServerRegistryService.DescribeServer(new DescribeServerRequest
            {
                Id = server
            });

            return await MigrateTo(describe.Server);
        }

        public async Task<bool> MigrateTo(ServerObject server)
        {
            if (server == null) return false;

            var request = await Stage.MigrationRegistryService.Register(new RegisterMigrationRequest
            {
                Migration = new MigrationObject
                {
                    Character = Character.ID,
                    Key = Key,
                    FromServer = Stage.ID,
                    ToServer = server.Id
                }
            });

            if (request.Result != MigrationRegistryResult.Ok) return false;

            IsMigrating = true;

            var session = new SessionObject
            {
                Account = Account.ID,
                Character = Character.ID,
                Server = Stage.ID,
                State = SessionState.Migrating
            };

            await Stage.SessionRegistry.UpdateSession(new UpdateSessionRequest { Session = session });

            var endpoint = new IPEndPoint(IPAddress.Parse(server.ServerConnection.Host), server.ServerConnection.Port);
            var address = endpoint.Address.MapToIPv4().GetAddressBytes();
            var port = endpoint.Port;

            await Update();
            await Dispatch(GetMigratePacket(address, (short)port));
            return true;
        }

        protected virtual IPacket GetMigratePacket(byte[] address, short port)
        {
            var packet = new UnstructuredOutgoingPacket(PacketSendOperations.MigrateCommand);

            packet.WriteBool(true);

            foreach (var b in address) packet.WriteByte(b);
            packet.WriteShort(port);
            return packet;
        }

        public async Task TrySendAliveReq()
        {
            if ((DateTime.UtcNow - LastRecvHeartbeatDate) >= SessionDisconnectDuration)
            {
                await Disconnect();
                return;
            }

            if ((DateTime.UtcNow - LastSentHeartbeatDate) >= SessionUpdateDuration)
            {
                LastSentHeartbeatDate = DateTime.UtcNow;
                await Dispatch(GetAliveReqPacket());
            }
        }

        public async Task TryRecvAliveAck()
        {
            LastRecvHeartbeatDate = DateTime.UtcNow;

            if (Account == null) return;

            var session = new SessionObject
            {
                Account = Account.ID,
                Character = Character.ID,
                Server = Stage.ID,
                State = IsLoggingIn ? SessionState.LoggingIn : SessionState.LoggedIn
            };

            await Stage.SessionRegistry.UpdateSession(new UpdateSessionRequest { Session = session });
        }

        protected virtual IPacket GetAliveReqPacket()
           => new UnstructuredOutgoingPacket(PacketSendOperations.AliveReq);
    }
}