using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//*****************************************
//创建人： Trigger 
//功能说明：
//***************************************** 
public class PlayerController : MonoBehaviour
{
    public float moveSpeed;//移动速度
    public float rotateSpeed;//旋转角度
    private float angleY;//左右看角度，绕Y轴转
    private float angleX;//上下看角度，绕X轴转
    public Animator animator;//动画控制器
    public Rigidbody rigid;//刚体
    public float jumpForce;//跳跃力
    public Transform gunPointTrans;//枪口位置
    public GameObject bloodEffectGo;//溅血特效预制体
    public GameObject grassEffectGo;//射击到地面特效预制体
    public GameObject pinkFlowerEffectGo;//粉色花
    public GameObject woodEffectGo;//树干
    public GameObject riverEffectGo;//水
    public GameObject otherEffectGo;//其他特效

    public float attackCD;//攻击CD
    public float attackTimer;//记录上一次攻击时间
    public Transform attackEffectTrans;//生成攻击特效位置
    public GameObject singleAttackEffectGo;//点射枪攻击特效

    public GUNTYPE gunType;//当前使用的枪类型
    public GameObject autoAttackEffectGo;//机关枪攻击特效
    public GameObject snipingAttackEffectGo;//狙击枪攻击特效
    //背包子弹总数
    private Dictionary<GUNTYPE, int> bulletsBag=new Dictionary<GUNTYPE, int>();
    //弹夹里的子弹数
    private Dictionary<GUNTYPE, int> bulletsClip = new Dictionary<GUNTYPE, int>();

    public int maxSingleShotBullets;
    public int maxAutoShotBullets;
    public int maxSnipingShotBullets;

    public bool isReloading;//正在填充子弹

    public GameObject[] gunGo;

    public GameObject scope;//倍镜

    public int HP;

    //每种武器对应的子弹威力
    private Dictionary<GUNTYPE, int> gunWeaponDamage = new Dictionary<GUNTYPE, int>();
    
    public AudioSource audioSource;
    public AudioClip singleShootAudio;
    public AudioClip autoShootAudio;
    public AudioClip snipingShootAudio;
    public AudioClip reloadAudio;
    public AudioClip hitGroundAudio;
    public AudioClip jumpAudio;
    public AudioSource moveAudioSource;

    public Text playerHPText;

    public GameObject[] gunUIGos;

    public Text bulletText;

    public GameObject bloodUIGo;

    public GameObject scopeUIGo;

    public GameObject gameOverPanel;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        bulletsBag.Add(GUNTYPE.SINGLESHOT,30);
        bulletsBag.Add(GUNTYPE.AUTO, 50);
        bulletsBag.Add(GUNTYPE.SNIPING, 5);
        bulletsClip.Add(GUNTYPE.SINGLESHOT, maxSingleShotBullets);
        bulletsClip.Add(GUNTYPE.AUTO, maxAutoShotBullets);
        bulletsClip.Add(GUNTYPE.SNIPING, maxSnipingShotBullets);
        gunWeaponDamage.Add(GUNTYPE.SINGLESHOT, 2);
        gunWeaponDamage.Add(GUNTYPE.AUTO, 1);
        gunWeaponDamage.Add(GUNTYPE.SNIPING, 5);
    }

    void Update()
    {
        PlayerMove();
        LookAround();
        Attack();
        Jump();
        ChangeGun();
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
        OpenOrCloseScope();
    }
    /// <summary>
    /// 玩家移动
    /// </summary>
    private void PlayerMove()
    {
        //玩家垂直轴向输入
        float verticalInput = Input.GetAxis("Vertical");
        //玩家水平轴向输入
        float horizontalInput = Input.GetAxis("Horizontal");
        //玩家在垂直轴向上的位移
        Vector3 movementV = transform.forward * verticalInput * moveSpeed
            * Time.deltaTime;
        //玩家在水平轴向上的位移
        Vector3 movementH = transform.right * horizontalInput * moveSpeed
            * Time.deltaTime;
        //把每一秒移动的位移加到位置上
        transform.position += movementV + movementH;
        animator.SetFloat("MoveX",horizontalInput);
        animator.SetFloat("MoveY", verticalInput);
        if (verticalInput>0||horizontalInput>0)
        {
            if (!moveAudioSource.isPlaying)
            {
                moveAudioSource.Play();
            }
        }
        else
        {
            if (moveAudioSource.isPlaying)
            {
                moveAudioSource.Stop();
            }
        }
    }
    /// <summary>
    /// 转头看
    /// </summary>
    private void LookAround()
    {
        //左右看
        //鼠标水平轴向输入
        float mouseX = Input.GetAxis("Mouse X");
        //左右看，改变y的值(玩家操作值)
        float lookHAngleY = mouseX * rotateSpeed;
        angleY = angleY + lookHAngleY;

        //上下看
        //鼠标垂直轴向输入(玩家操作值)
        float mouseY = -Input.GetAxis("Mouse Y");
        //上下看，改变x的值
        float lookVAngleX = mouseY * rotateSpeed;
        angleX =Mathf.Clamp(angleX + lookVAngleX,-60,60);

        transform.eulerAngles = new Vector3(
            angleX, angleY, transform.eulerAngles.z);
    }
    /// <summary>
    /// 攻击
    /// </summary>
    private void Attack()
    {
        if (!isReloading)
        {
            switch (gunType)
            {
                case GUNTYPE.SINGLESHOT:
                    SingleShotAttack();
                    break;
                case GUNTYPE.AUTO:
                    AutoAttack();
                    break;
                case GUNTYPE.SNIPING:
                    SnipingAttack();
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 跳跃
    /// </summary>
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigid.AddForce(jumpForce*Vector3.up);
            audioSource.PlayOneShot(jumpAudio);
        }
    }
    /// <summary>
    /// 切换武器
    /// </summary>
    private void ChangeGun()
    {
        if (Input.GetKeyDown(KeyCode.C)&&!isReloading)
        {
            gunType++;
            if (gunType>GUNTYPE.SNIPING)
            {
                gunType = 0;
            }
            switch (gunType)
            {
                case GUNTYPE.SINGLESHOT:
                    attackCD = 0.2f;
                    ChangeGunGameObject(0);
                    break;
                case GUNTYPE.AUTO:
                    attackCD = 0.1f;
                    ChangeGunGameObject(1);
                    break;
                case GUNTYPE.SNIPING:
                    attackCD = 1;
                    ChangeGunGameObject(2);
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 显示对应的游戏物体
    /// </summary>
    private void ChangeGunGameObject(int gunLevel)
    {
        for (int i = 0; i < gunGo.Length; i++)
        {
            gunGo[i].SetActive(false);
            gunUIGos[i].SetActive(false);
        }
        gunGo[gunLevel].SetActive(true);
        gunUIGos[gunLevel].SetActive(true);
        bulletText.text ="X"+ bulletsClip[gunType].ToString();
    }

    /// <summary>
    /// 点射枪攻击
    /// </summary>
    private void SingleShotAttack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - attackTimer >= attackCD)
        {
            //取到当前使用枪对应的弹夹的子弹数量
            if (bulletsClip[gunType] > 0)//弹夹还有子弹，可以攻击
            {
                PlaySound(singleShootAudio);
                bulletsClip[gunType]--;
                bulletText.text = "X" + bulletsClip[gunType].ToString();
                animator.SetTrigger("SingleAttack");
                GameObject go = Instantiate(singleAttackEffectGo, attackEffectTrans);
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                Invoke("GunAttack", 0.1f);
            }
            else//弹夹里边子弹用完了，需要从背包里拿子弹填充到弹夹里
            {
                Reload();
            }
        }
    }
    /// <summary>
    /// 机关枪攻击
    /// </summary>
    private void AutoAttack()
    {
        if (Input.GetMouseButton(0)&&Time.time-attackTimer>=attackCD)
        {
            //取到当前使用枪对应的弹夹的子弹数量
            if (bulletsClip[gunType] > 0)//弹夹还有子弹，可以攻击
            {
                PlaySound(autoShootAudio);
                bulletsClip[gunType]--;
                bulletText.text = "X" + bulletsClip[gunType].ToString();
                animator.SetBool("AutoAttack", true);
                GameObject go = Instantiate(autoAttackEffectGo, attackEffectTrans);
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                GunAttack();
            }
            else//弹夹里边子弹用完了，需要从背包里拿子弹填充到弹夹里
            {
                Reload();
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool("AutoAttack", false);
        }
    }
    /// <summary>
    /// 狙击枪攻击
    /// </summary>
    private void SnipingAttack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - attackTimer >= attackCD)
        {
            //取到当前使用枪对应的弹夹的子弹数量
            if (bulletsClip[gunType] > 0)//弹夹还有子弹，可以攻击
            {
                PlaySound(snipingShootAudio);
                bulletsClip[gunType]--;
                bulletText.text = "X" + bulletsClip[gunType].ToString();
                animator.SetTrigger("SnipingAttack");
                GameObject go = Instantiate(snipingAttackEffectGo, attackEffectTrans);
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
                Invoke("GunAttack", 0.25f);
            }
            else//弹夹里边子弹用完了，需要从背包里拿子弹填充到弹夹里
            {
                Reload();
            }
        }
    }
    /// <summary>
    /// 具体的射击功能
    /// </summary>
    private void GunAttack()
    {
        RaycastHit hit;
        attackTimer = Time.time;
        if (Physics.Raycast(gunPointTrans.position, gunPointTrans.forward, out hit, 5))
        {
            //特效生成
            switch (hit.collider.tag)
            {
                case "Enemy":
                    hit.transform.GetComponent<Enemy>().TakeDamage(gunWeaponDamage[gunType]);
                    Instantiate(bloodEffectGo, hit.point, Quaternion.identity);
                    break;
                case "Flowers":
                    PlaySound(hitGroundAudio);
                    Instantiate(pinkFlowerEffectGo, hit.point, Quaternion.identity);
                    break;
                case "Hourse":
                    PlaySound(hitGroundAudio);
                    Instantiate(bloodEffectGo, hit.point, Quaternion.identity);
                    break;
                case "Grass":
                    PlaySound(hitGroundAudio);
                    Instantiate(grassEffectGo, hit.point, Quaternion.identity);
                    break;
                case "Wood":
                    PlaySound(hitGroundAudio);
                    Instantiate(woodEffectGo, hit.point, Quaternion.identity);
                    break;
                case "Others":
                    PlaySound(hitGroundAudio);
                    Instantiate(otherEffectGo, hit.point, Quaternion.identity);
                    break;
                case "River":
                    PlaySound(hitGroundAudio);
                    Instantiate(riverEffectGo, hit.point, Quaternion.identity);
                    break;
                default:
                    break;
            }
        }
    }
    /// <summary>
    /// 填充子弹
    /// </summary>
    private void Reload()
    {
        bool canReload=false;//是否可以装子弹
        switch (gunType)
        {
            case GUNTYPE.SINGLESHOT:
                if (bulletsClip[gunType]<maxSingleShotBullets)
                {
                    canReload = true;
                }                
                break;
            case GUNTYPE.AUTO:
                if (bulletsClip[gunType] < maxAutoShotBullets)
                {
                    canReload = true;
                }
                break;
            case GUNTYPE.SNIPING:
                if (bulletsClip[gunType] <maxSnipingShotBullets)
                {
                    canReload = true;
                }
                break;
            default:
                break;
        }
        if (canReload)//弹夹不满可以填充
        {
            PlaySound(reloadAudio);
            if (bulletsBag[gunType] > 0)//背包里没子弹的时候
            {
                isReloading = true;
                Invoke("RecoverAttackState", 2.667f);
                animator.SetTrigger("Reload");
                switch (gunType)
                {
                    case GUNTYPE.SINGLESHOT:
                        if (bulletsBag[gunType] >= maxSingleShotBullets)
                        //背包里的剩余子弹数是足够填充满弹夹的
                        {
                            if (bulletsClip[gunType] > 0) //如果弹夹里有剩余，补满
                            {
                                //补充数量
                                int bulletNum = maxSingleShotBullets - bulletsClip[gunType];
                                bulletsBag[gunType] -= bulletNum;
                                bulletsClip[gunType] += bulletNum;
                            }
                            else  //没剩余，则需要装入最大数量
                            {
                                bulletsBag[gunType] -= maxSingleShotBullets;
                                bulletsClip[gunType] += maxSingleShotBullets;
                            }
                        }
                        else
                        {
                            //不够加满的时候就把剩余的都填充到弹夹里
                            bulletsClip[gunType] += bulletsBag[gunType];
                            bulletsBag[gunType] = 0;
                        }
                        break;
                    case GUNTYPE.AUTO:
                        if (bulletsBag[gunType] >= maxAutoShotBullets)
                        {
                            if (bulletsClip[gunType] > 0) //如果弹夹里有剩余，补满
                            {
                                //补充数量
                                int bulletNum = maxAutoShotBullets - bulletsClip[gunType];
                                bulletsBag[gunType] -= bulletNum;
                                bulletsClip[gunType] += bulletNum;
                            }
                            else  //没剩余，则需要装入最大数量
                            {
                                bulletsBag[gunType] -= maxAutoShotBullets;
                                bulletsClip[gunType] += maxAutoShotBullets;
                            }
                        }
                        else
                        {
                            bulletsClip[gunType] += bulletsBag[gunType];
                            bulletsBag[gunType] = 0;
                        }
                        break;
                    case GUNTYPE.SNIPING:
                        if (bulletsBag[gunType] >= maxSnipingShotBullets)
                        {
                            if (bulletsClip[gunType] > 0) //如果弹夹里有剩余，补满
                            {
                                //补充数量
                                int bulletNum = maxSnipingShotBullets - bulletsClip[gunType];
                                bulletsBag[gunType] -= bulletNum;
                                bulletsClip[gunType] += bulletNum;
                            }
                            else  //没剩余，则需要装入最大数量
                            {
                                bulletsBag[gunType] -= maxSnipingShotBullets;
                                bulletsClip[gunType] += maxSnipingShotBullets;
                            }
                        }
                        else
                        {
                            bulletsClip[gunType] += bulletsBag[gunType];
                            bulletsBag[gunType] = 0;
                        }
                        break;
                    default:
                        break;
                }
            }
            bulletText.text = "X" + bulletsClip[gunType].ToString();
        }
        animator.SetBool("AutoAttack", false);
    }
    /// <summary>
    /// 填充完毕，可以攻击
    /// </summary>
    private void RecoverAttackState()
    {
        isReloading = false;
    }
    /// <summary>
    /// 开关倍镜
    /// </summary>
    private void OpenOrCloseScope()
    {
        if (Input.GetMouseButton(1)&&gunType==GUNTYPE.SNIPING)
        {
            scope.SetActive(true);
            scopeUIGo.SetActive(true);
        }
        else
        {
            scope.SetActive(false);
            scopeUIGo.SetActive(false);
        }
    }
    /// <summary>
    /// 收到伤害
    /// </summary>
    /// <param name="value"></param>
    public void TakeDamage(int value)
    {
        bloodUIGo.SetActive(true);
        Invoke("HideBloodUIGo", 0.5f);
        HP -= value;
        if (HP<=0)
        {
            HP = 0;
            gameOverPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        playerHPText.text = HP.ToString();
    }
    /// <summary>
    /// 播放音频
    /// </summary>
    public void PlaySound(AudioClip ac)
    {
        audioSource.PlayOneShot(ac);
    }
    private void HideBloodUIGo()
    {
        bloodUIGo.SetActive(false);
    }

    public void Replay()
    {
        SceneManager.LoadScene(0);
    }
}
[System.Serializable]
public enum GUNTYPE
{ 
    SINGLESHOT,
    AUTO,
    SNIPING
}
