using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelGameHUD : PanelBase
{
    public static System.Action OnUpdateHUDEvent;
    public Text coinsLabel, playerLevelLabel;
    public void ArrowPressed(int directionIndex)
    {
        UnitController.eDirection direction = (UnitController.eDirection)directionIndex;
        LevelManager.OnPlayerInput(direction);
    }
    public void HomePressed()
    {
        GameManager.Instance.AttemptChangeState(GameManager.eGameState.MENU);
    }
    public static void Refresh()
    {
        if (OnUpdateHUDEvent != null)
            OnUpdateHUDEvent();
    }
    private void UpdateHUDEvent()
    {
        coinsLabel.text = string.Format("{0}/{1}", GameManager.PlayerData.Coins, GameManager.PlayerData.TargetCoins);
        playerLevelLabel.text = string.Format("Level {0}", GameManager.PlayerData.Level);
    }
    public override void InitializePanel()
    {
        base.InitializePanel();
        OnUpdateHUDEvent += UpdateHUDEvent;
        UpdateHUDEvent();
    }
    public override void DestroyPanel()
    {
        base.DestroyPanel();
        OnUpdateHUDEvent -= UpdateHUDEvent;
    }
}
