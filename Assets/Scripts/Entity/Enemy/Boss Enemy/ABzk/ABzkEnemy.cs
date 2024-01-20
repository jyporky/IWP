using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABzkEnemy : EnemyBase
{
    public override void ChangeHealth(int healthChanged)
    {
        float finalHealthChanged = healthChanged;
        // if player threshold is below 25%, cut damage by half
        if (currentHP < (float)maxHP * 0.25f)
        {
            base.ChangeHealth((int)(finalHealthChanged *= 0.5f));
        }
        // if player threshold is below 50%, cut damage by 25%
        else if (currentHP < (float)maxHP * 0.5f)
        {
            base.ChangeHealth((int)(finalHealthChanged *= 0.75f));
        }

        base.ChangeHealth(healthChanged);
    }
}
