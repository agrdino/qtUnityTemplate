using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class hudBase : MonoBehaviour
{
    public virtual void Initialize()
    {
        InitEvent();
    }
    protected abstract void InitEvent();
    public abstract void InitObject();

    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
