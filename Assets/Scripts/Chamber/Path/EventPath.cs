using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPath : PathInfo
{
    public override void PathSelected()
    {
        ChamberManager.GetInstance().InteractWithPath(PathType.EVENT);
    }
}
