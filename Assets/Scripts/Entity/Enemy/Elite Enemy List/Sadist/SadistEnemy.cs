using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SadistEnemy : EnemyBase
{
    [SerializeField] Keyword buffSelfStatusEffect;
    public override void PlayCard(CardSO cardPlayed)
    {
        base.PlayCard(cardPlayed);
        foreach (Keyword kw in cardPlayed.keywordsList)
        {
            if (kw.keywordType == KeywordType.Damage && kw.inflictSelf)
            {
                AddStatusEffect(kw);
            }
        }
    }
}
