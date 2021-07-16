﻿using System;
using Edelstein.Protocol.Datastore;

namespace Edelstein.Protocol.Gameplay.Users
{
    public class Account : IDataDocument
    {
        public int ID { get; init; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string PIN { get; set; }
        public string SPW { get; set; }

        public byte? Gender { get; set; }

        public byte? LatestConnectedWorld { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
