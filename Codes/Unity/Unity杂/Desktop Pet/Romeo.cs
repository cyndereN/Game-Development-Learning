using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Romeo : MonoBehaviour
{
    private Animator animator;
    private float timer = 1.0f;
    private bool isTimerStart = false; // 是否打开了伪定时器
    private bool isPlaying = false; // 是否在播放动画
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        Globle.isEnableAnimator = false;
        animator.enabled = false;
        //animator.SetBool("Nod Head", true);
    }
 
    // Update is called once per frame
    void Update()
    {
        if(isTimerStart && Globle.isEnableAnimator) // 伪定时器，每帧检查
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                isPlaying = false;
                Globle.isEnableAnimator = false;
                animator.enabled = Globle.isEnableAnimator;
                isTimerStart = false;
            }
        }
        if (Input.GetMouseButtonDown(1)) // 右键按下
        {
            isPlaying = true;
            Globle.isEnableAnimator = true;
            animator.enabled = Globle.isEnableAnimator;
            animator.SetBool("Dash", true);
            isTimerStart = false;
        }
        if (Input.GetMouseButtonUp(1)) // 右键释放
        {
            isPlaying = true;
            animator.SetBool("Dash", false);
            timer = 2.0f; // 开始定时
            isTimerStart = true;
        }
        if (Input.GetMouseButtonDown(0)) // 左键单击
        {
            isPlaying = true;
            Globle.isEnableAnimator = true;
            animator.enabled = Globle.isEnableAnimator;
            animator.SetTrigger("Jump"); 
            timer = 2.0f; // 开始定时
            isTimerStart = true;
        }
 
        // 鼠标移出边，框进入IDLE动画
        Vector3 mousePositionOnScreen = Input.mousePosition;
        if (mousePositionOnScreen.x < Screen.width * (-1/4) || mousePositionOnScreen.x > Screen.width * 5/4 || mousePositionOnScreen.y < Screen.height * (-1) / 4 || mousePositionOnScreen.y > Screen.height * 5/4)
        {
            Globle.isEnableAnimator = true;
            animator.enabled = true;
        }
        else
        {
            if (!isPlaying)
            {
                Globle.isEnableAnimator = false;
                animator.enabled = false;
            }
        }
    }
}