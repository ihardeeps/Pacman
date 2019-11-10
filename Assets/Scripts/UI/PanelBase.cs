using UnityEngine;
using System.Collections;

public class PanelBase : MonoBehaviour
{
    public enum PanelTransition
    {
        NULL,
        TWEEN,
        ALPHA,
        DELAY
    }

    [HideInInspector]
    public PanelTransition trans;
    [HideInInspector]
    public bool autoEnable;
    [HideInInspector]
    public Vector3 posOn = Vector3.zero, posOff = new Vector3(0f, 4096f);
    [HideInInspector]
    public float delayOn, delayOff;
    [HideInInspector]
    public float alphaOn, alphaOff;

    private bool inTransition;
    private bool active = true, init;

    public bool InTransition { get { return inTransition; } set { inTransition = value; } }
    public bool Active { get { return active; } set { active = value; } }
    public bool Init { get { return init; } }

    public void Initialize(bool instant = false)
    {
        if (!instant)
            StartCoroutine("DelayedInitialize");
        else
        {
            InitializePanel();

            if (autoEnable) Enable();
            else Disable(true);

            init = true;
        }
    }
    private IEnumerator DelayedInitialize()
    {
        yield return null;
        InitializePanel();

        if (autoEnable) Enable();
        else Disable(true);

        init = true;
    }

    public void Enable(bool instant = false)
    {
        gameObject.SetActive(true);

        inTransition = true;
        PrepareEnablePanel(instant);

        switch (trans)
        {
            case PanelTransition.NULL:
                transform.localPosition = posOn;
                CompleteEnable();
                break;

            case PanelTransition.TWEEN:
                if (!instant && delayOn > 0f)
                {
                    //TODO
                }
                else
                {
                    transform.localPosition = posOn;
                    CompleteEnable();
                }
                break;

            case PanelTransition.ALPHA:
                if (!instant && delayOn > 0f)
                {
                    //TODO
                }
                else
                {
                    CompleteEnable();
                }
                break;

            case PanelTransition.DELAY:
                StopAllCoroutines();
                transform.localPosition = posOn;
                if (!instant && delayOn > 0f) StartCoroutine("DelayedEnable");
                else CompleteEnable();
                break;
        }
    }
    private IEnumerator DelayedEnable()
    {
        yield return new WaitForSeconds(delayOn);
        CompleteEnable();
    }
    public void CompleteEnable()
    {
        inTransition = false;
        CompleteEnablePanel();
        active = true;
    }

    public void Disable(bool instant = false)
    {
        inTransition = true;
        PrepareDisablePanel(instant);

        switch (trans)
        {
            case PanelTransition.NULL:
                transform.localPosition = posOff;
                CompleteDisable();
                break;

            case PanelTransition.TWEEN:
                if (!instant && delayOff > 0f)
                {
                    //TODO
                }
                else
                {
                    transform.localPosition = posOff;
                    CompleteDisable();
                }
                break;

            case PanelTransition.ALPHA:
                if (!instant && delayOff > 0f)
                {
                    //TODO
                }
                else
                {
                    CompleteDisable();
                }
                break;

            case PanelTransition.DELAY:
                StopAllCoroutines();
                if (Active && !instant && delayOff > 0f) StartCoroutine("DelayedDisable");
                else
                {
                    transform.localPosition = posOff;
                    CompleteDisable();
                }
                break;
        }
    }
    private IEnumerator DelayedDisable()
    {
        yield return new WaitForSeconds(delayOff);
        transform.localPosition = posOff;
        CompleteDisable();
    }
    public void CompleteDisable()
    {
        inTransition = false;
        CompleteDisablePanel();
        active = false;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        DestroyPanel();
    }

    public virtual void InitializePanel() { }
    public virtual void PrepareEnablePanel(bool instant) { }
    public virtual void CompleteEnablePanel() { }
    public virtual void PrepareDisablePanel(bool instant) { }
    public virtual void CompleteDisablePanel() { }
    public virtual void DestroyPanel() { }
}
