using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCard : MonoBehaviour
{
    private CardSO cardSO;

    public void SetCardSO(CardSO cardSOReference)
    {
        cardSO = cardSOReference;
    }

    public CardSO GetCardSO()
    {
        return cardSO;
    }
}
