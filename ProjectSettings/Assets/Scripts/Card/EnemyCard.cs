using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCard : MonoBehaviour
{
    [SerializeField] Sprite actionCardBack;
    [SerializeField] Sprite utilityCardBack;
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
