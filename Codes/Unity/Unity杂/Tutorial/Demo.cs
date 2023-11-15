using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoGroup : TutorialBehaviorGroupBase

{
    protected override Queue<ITutorialBehaviour> GetBehaviours()
    {
        Queue<ITutorialBehaviour> behaviours = new Queue<ITutorialBehaviour>();
        behaviours.Enqueue(new ClickButtonA());
        return behaviours;
    }
}

public class ClickButtonA : TutorialBehaviourBase
{
    protected override void OnEnterLogic()
    {
        // TODO:
        // Initialize button, bind event
        // After clicking execute OnExitLogic();
    }

    protected override void OnExitLogic()
    {
        
    }
}