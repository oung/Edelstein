﻿using Edelstein.Protocol.Gameplay.Stages.Login;
using Edelstein.Protocol.Gameplay.Stages.Login.Contracts.Pipelines;

namespace Edelstein.Common.Gameplay.Stages.Login.Contracts.Pipelines;

public record SPWChange(
    ILoginStageUser User,
    string SPWCurrent,
    string SPWNew
) : ISPWChange;