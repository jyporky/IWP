using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingPath : PathInfo
{
    public override void PathSelected()
    {
        ChamberManager.GetInstance().InteractWithPath(PathType.STARTING);
    }
}
