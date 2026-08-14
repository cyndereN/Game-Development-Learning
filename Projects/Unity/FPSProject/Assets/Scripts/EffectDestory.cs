using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：特效销毁
//***************************************** 
public class EffectDestory : MonoBehaviour
{
    public float destoryTime;//销毁时间

    void Start()
    {
        Destroy(gameObject, destoryTime);
    }

    void Update()
    {

    } 
}
