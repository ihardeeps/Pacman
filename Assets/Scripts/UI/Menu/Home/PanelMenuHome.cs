using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelMenuHome : PanelBase
{
    public void OnPlayButtonPressed()
    {
        GameManager.Instance.AttemptChangeState(GameManager.eGameState.GAME);
    }
}
