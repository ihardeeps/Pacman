using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public Text stateLabel;
    public RectTransform rectTransform;
    public Image image;
    public void SetState(Node.eState state)
    {
        switch (state)
        {
            case Node.eState.BLOCKER:
                image.sprite = GameManager.PersistentData.GameConfiguration.blocked;
                break;
            case Node.eState.START:
                image.sprite = GameManager.PersistentData.GameConfiguration.start;
                break;
            case Node.eState.END_OPEN:
                image.sprite = GameManager.PersistentData.GameConfiguration.open_door;
                break;
            case Node.eState.END_LOCKED:
                image.sprite = GameManager.PersistentData.GameConfiguration.closed_door;
                break;
            default:
                image.sprite = GameManager.PersistentData.GameConfiguration.empty;
                break;
        }
    }
}
