using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum eCurrency
    {
        COIN,
        GEM
    }
    public int value = 10;
    public eCurrency currency;
    public void Collected()
    {
        Destroy(gameObject);
    }
}
