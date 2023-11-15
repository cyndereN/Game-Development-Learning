using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITutorial
{
    
}

/// <summary>
/// Tutorial is a queue of behaviors
/// - Enter
///     - show button
///     - animation
///     - bind action
/// - Exit
///     - next behavior
/// ...
/// </summary>

public interface ITutorialBehaviour
{
    void OnEnter(Action callBack);
    void OnExit();
}

public abstract class TutorialBehaviourBase : ITutorialBehaviour
{
    private Action _callBack;
    
    public virtual void OnEnter(Action callBack)
    {
        _callBack = callBack;
        OnEnterLogic();
    }

    protected abstract void OnEnterLogic();

    public virtual void OnExit()
    {
        OnExitLogic();
        if (_callBack != null)
            _callBack();
    }
    protected abstract void OnExitLogic();
}