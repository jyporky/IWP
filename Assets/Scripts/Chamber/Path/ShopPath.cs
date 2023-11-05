using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPath : PathInfo
{
    public override void PathSelected()
    {
        ChamberManager.GetInstance().InteractWithPath(PathType.SHOP);
    }
}
