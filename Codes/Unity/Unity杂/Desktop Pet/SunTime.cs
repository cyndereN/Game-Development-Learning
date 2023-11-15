using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunTime : MonoBehaviour
{
    int hour;
    int minute; // 四分钟一动
    int totalMinute;
    int angle;
    Light myLight;
    GameObject startPartical;
    // Start is called before the first frame update
    void Start()
    {
        myLight = GetComponent<Light>();
		//判断后开始协程
		if (!myLight)
		{
            Debug.LogError("请将脚本添加到Directional Light上");
		}

        startPartical = GameObject.Find("Start");
        if (!startPartical)
		{
            Debug.LogError("找不到星星特效");
		}

        StartCoroutine(WaitForTime()); // 开启一个每240s执行一次的无限循环协程，计算时间，改变太阳角度
    }

    IEnumerator WaitForTime(float time = 240f)
	{
        while (true)
		{
            hour = DateTime.Now.Hour;
            minute = DateTime.Now.Minute;

            totalMinute= hour * 60 + minute;
            angle = totalMinute / 4; //4min一度，6点0, 0点-90，12点90, 18点180

            transform.rotation = Quaternion.Euler(new Vector3(angle - 90, 0, 0));

            if (angle >= 0 && angle <= 180) // 0-12点光强度0-1，（当前度数+90）/180=光照强度比例
			{
                myLight.intensity = (angle - 90f + 90f) / 180f;
			}
            if (angle > 180 && angle < 360)
			{
                myLight.intensity = (angle - 90f - 270f) / (-180f);
			}
			//上面-90度是为了平行光的旋转，跟平行光有关的角度都要减去90度，而时间的角度anlge是不用减去90度的
			if ((angle > 290 && angle < 360) || (angle < 90)) //18-6
			{
                startPartical.SetActive(true);
			}
			else
			{
                startPartical.SetActive(false);
            }
            yield return new WaitForSeconds(time);
        }
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
