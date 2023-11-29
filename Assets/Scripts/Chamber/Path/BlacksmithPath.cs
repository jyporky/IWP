using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlacksmithPath : PathInfo
{
    public override void PathSelected()
    {
        ChamberManager.GetInstance().InteractWithPath(PathType.UPGRADE_STATION);
    }
}
