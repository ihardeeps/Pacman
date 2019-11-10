using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    private int level = 1;
    private int coins, targetCoins;
    private int gems;

    public int Level { get { return level; } }
    public int Coins { get { return coins; } }
    public int TargetCoins { get { return targetCoins; } }
    public int Gems { get { return gems; } }

    public void Collect(Collectible item)
    {
        switch (item.currency)
        {
            case Collectible.eCurrency.COIN:
                coins += item.value;
                break;
            case Collectible.eCurrency.GEM:
                gems += item.value;
                break;

        }
    }
    public void LevelUp()
    {
        level++;
    }
    //This is done for simplicity. This can be moved to Objective Manager where objectives can be defined accordingly.
    public void SetObjective(int coinsToCollect)
    {
        targetCoins = coinsToCollect;
    }
    public void Reset()
    {
        coins = targetCoins = gems = 0;
    }
}
