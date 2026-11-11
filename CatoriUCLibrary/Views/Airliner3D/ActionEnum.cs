using System.Text.Json.Serialization;
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriUCLibrary.Views.Airliner3D
{
    [JsonConverter(typeof(JsonStringEnumConverter<ActionEnum>))]
    public enum ActionEnum
    {
        BankLeft = 0,
        BankRight = 1,
        InvertLeft = 2,
        InvertRight = 3,
        StartEngine1 = 4,
        StartEngine2 = 5,
        StopEngine1 = 6,
        StopEngine2 = 7,
        SetSpeed = 8,
        NoseUp = 9,
        NoseDown = 10,
        GearUp = 11,
        GearDown = 12,
        FlapUp = 13,
        FlapHalf = 14,
        FlapDown = 15,
        SetHeading = 16,
        SetScale = 17
    }
}
