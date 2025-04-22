using System.Collections.Generic;
using UnityEngine;

public static class PegScoreValues
{
    public static readonly Dictionary<PegType, int> PegScoreMap = new Dictionary<PegType, int>
    {
        { PegType.FillerPeg, 11 },
        { PegType.ObjectivePeg, 333 },
        { PegType.SpecialPeg, 22 }
    };
}
