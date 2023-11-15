using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TutorialBehaviorGroupBase
{
    private Queue<ITutorialBehaviour> _behaviours;

    public void OnEnter()
    {
        _behaviours = GetBehaviours();
    }

    protected abstract Queue<ITutorialBehaviour> GetBehaviours();

    private bool ExecuteBehavior()
    {
        if (_behaviours.Count > 0)
        {
            ITutorialBehaviour behaviour = _behaviours.Dequeue();
            behaviour.OnEnter(BehaviourEnd);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void BehaviourEnd()
    {
        if (!ExecuteBehavior())
        {
            //TODO: what to do after guiding
        }
    }
}
