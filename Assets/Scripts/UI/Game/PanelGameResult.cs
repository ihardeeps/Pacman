using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelGameResult : PanelBase
{
    public Text statusLabel, nextButtonLable;
    public override void PrepareEnablePanel(bool instant)
    {
        base.PrepareEnablePanel(instant);
        statusLabel.text = LevelManager.Instance.State == LevelManager.eState.WIN ? "Victory" : "Lose";
        nextButtonLable.text = LevelManager.Instance.State == LevelManager.eState.WIN ? "Next" : "Retry";
        if(LevelManager.Instance.State == LevelManager.eState.WIN)
        {
            GameManager.PlayerData.LevelUp();
        }
    }
    public void HomeButtonPressed()
    {
        GameManager.Instance.AttemptChangeState(GameManager.eGameState.MENU);
    }
    public void NextButtonPressed()
    {
        GameManager.Instance.AttemptChangeState(GameManager.eGameState.GAME, true);
    }
}
