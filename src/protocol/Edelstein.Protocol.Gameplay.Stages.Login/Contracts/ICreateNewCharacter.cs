﻿using Edelstein.Protocol.Gameplay.Stages.Contracts;

namespace Edelstein.Protocol.Gameplay.Stages.Login.Contracts;

public interface ICreateNewCharacter : IStageUserContract<ILoginStageUser>
{
    string Name { get; }
    int Race { get; }
    short SubJob { get; }
    int Face { get; }
    int Hair { get; }
    int HairColor { get; }
    int Skin { get; }
    int Coat { get; }
    int Pants { get; }
    int Shoes { get; }
    int Weapon { get; }
    byte Gender { get; }
}