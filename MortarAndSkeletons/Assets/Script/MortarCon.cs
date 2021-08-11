using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MortarRoState //迫击炮旋转行为
{
    onRotate, //正在跟踪
    Catch,    //捕获到的
    Lock      //锁定无法旋转的
}


public class MortarCon : MonoBehaviour
{

    //Range范围提示相关

    public float maxRange=30.0f; //最大发射距离(m)
    public float minRange=10.0f; //最小发射距离(m)

    public float dmRange = 3.0f;//伤害范围

    [SerializeField]
    GameObject cirRGSP;

    [SerializeField]
    GameObject cirDMSP;

    SpriteRenderer cirRGRend;
    SpriteRenderer cirDMRend;

    bool rangeVis = false;//是否开启范围查看


    //碰撞检测
    RaycastHit raycast;
    int layerMask;
    float maxR2;
    float minR2;
    float rcx;
    float rcz;
    float lcx;
    float lcz;
    float rcdis;//距离
    Vector2 rcnormal;


    //旋转行为
    MortarRoState roState = MortarRoState.onRotate;
    public float roSpeed = 480.0f;//旋转跟踪速度(迫击炮LookAt 与 炮管共用这一速度)
    public float catchDeg = 3.0f;//捕获允许最大偏移
    public float launchLock = 1.5f;//发射锁定时间
    float nlTime;//当前锁定时间
    float roStep;
    Vector3 lookVec;
    Quaternion lookRo;



    //炮弹发射相关

    public float gravity = 9.8f;//重力加速度
    public float loadTime = 3.0f;//装填时间
    public Color hasbeenLoad;
    public Color notload;



    float nlodtime;//当前装填时间
    Transform shellPoint; //炮弹位置
    public GameObject shell;//炮弹

    float baseVelocity;//初速度
    float bv2;
    float deg;

    Transform cannon;//炮管 --- 设置角度

    bool isLoad = true;//炮弹是否已装填


    private void Awake()
    {

        //设置发射范围圈Sprite
        cirRGSP.transform.localScale = new Vector3(maxRange * 0.2f,maxRange * 0.2f,1);

        cirRGRend = cirRGSP.GetComponent<SpriteRenderer>();

        cirRGRend.material.SetFloat("uvRange", Mathf.Pow((minRange/maxRange)/2.0f,2)); //

        //设置伤害范围圈Sprite

        cirDMSP.transform.localScale = new Vector3(dmRange * 0.2f, dmRange * 0.2f, 1);

        cirDMRend = cirDMSP.GetComponent<SpriteRenderer>();
        cirDMRend.material.SetColor("_Color", hasbeenLoad);

        cirRGSP.SetActive(false);
        cirDMSP.SetActive(false);

        //碰撞相关
        maxR2 = maxRange * maxRange;
        minR2 = minRange * minRange;
        layerMask = 1<<LayerMask.NameToLayer("mcollider");

        //炮弹发射相关
        cannon = transform.GetChild(0);
        shellPoint = cannon.GetChild(0);
        baseVelocity = Mathf.Sqrt(gravity * maxRange);
        bv2 = baseVelocity * baseVelocity;

        shell.SetActive(false);

    }

    

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            rangeVis = !rangeVis;
            cirRGSP.SetActive(rangeVis);
            cirDMSP.SetActive(rangeVis);

            if(!rangeVis&&roState==MortarRoState.Catch) roState = MortarRoState.onRotate; //当范围不可见时，若已捕捉，转为旋转
        }

        if(roState == MortarRoState.Lock)
        {
            nlTime += Time.deltaTime;
            if (nlTime >= launchLock) roState = MortarRoState.onRotate;
        }

        if (!isLoad)
        { //装填冷却
            nlodtime += Time.deltaTime;
            if (nlodtime >= loadTime)
            {
                isLoad = true;
                cirDMRend.material.SetColor("_Color", hasbeenLoad);
                cirDMRend.material.SetFloat("load", 1);
            }
            else cirDMRend.material.SetFloat("load", nlodtime / loadTime);
        }

        if (rangeVis)
        {

            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out raycast,float.MaxValue,layerMask)) {//射线碰撞检测

                //位置计算
                rcx = raycast.point.x;
                rcz = raycast.point.z;
                lcx = rcx - transform.position.x;
                lcz = rcz - transform.position.z;

                rcdis = lcx*lcx+lcz*lcz;

                if (rcdis > maxR2)//夹取到最大范围
                {
                    rcnormal = new Vector2(lcx, lcz).normalized;
                    rcx = maxRange * rcnormal.x + transform.position.x;
                    rcz = maxRange * rcnormal.y + transform.position.z;
                    rcdis = maxRange;

                }
                else if (rcdis < minR2)
                {
                    rcnormal = new Vector2(lcx, lcz).normalized;
                    rcx = minRange * rcnormal.x + transform.position.x;
                    rcz = minRange * rcnormal.y + transform.position.z;
                    rcdis = minRange;
                }
                else rcdis = Mathf.Sqrt(rcdis);

                //设置迫击炮朝向&角度

                if(roState == MortarRoState.onRotate) { //正在旋转 尝试进入 Catch

                    roStep = roSpeed * Time.deltaTime;
                    lookVec = new Vector3(rcx, gameObject.transform.position.y, rcz) - gameObject.transform.position;
                    //旋转迫击炮
                    lookRo = Quaternion.LookRotation(lookVec, Vector3.up);
                    gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, lookRo, roStep);

                    //旋转炮管
                    deg = Mathf.Asin(rcdis * gravity / bv2) * Mathf.Rad2Deg / 2.0f;
                    cannon.eulerAngles = new Vector3(cannon.eulerAngles.x, cannon.eulerAngles.y,
                        Mathf.MoveTowards(cannon.eulerAngles.z,deg, roStep));

                    if (Vector3.Angle(transform.forward, lookVec) < catchDeg && Mathf.Abs(cannon.eulerAngles.z - deg) < catchDeg) roState = MortarRoState.Catch;



                }
                else if(roState == MortarRoState.Catch) //已捕获 
                {
                    gameObject.transform.LookAt(new Vector3(rcx, gameObject.transform.position.y, rcz), Vector3.up);

                    //设置炮管角度
                    deg = Mathf.Asin(rcdis * gravity / bv2) * Mathf.Rad2Deg / 2.0f;
                    cannon.eulerAngles = new Vector3(cannon.eulerAngles.x, cannon.eulerAngles.y, deg);

                }//else if(roState == MortarRoState.Lock) 锁定下不改变迫击炮和炮管的旋转

                //设置 dmRange worldPositon Rotate
                cirDMSP.transform.position = new Vector3(rcx, cirDMSP.transform.position.y, rcz);
                cirDMSP.transform.eulerAngles = new Vector3(90, 0, 0);

            }
        }


        if(Input.GetMouseButtonDown(0)&&rangeVis&&isLoad)
        {

            //对齐朝向  ---  可能在开启Range后立刻发射，还未完成跟踪

            if(roState == MortarRoState.onRotate)
            {
                gameObject.transform.LookAt(new Vector3(rcx, gameObject.transform.position.y, rcz), Vector3.up);

                //设置炮管角度
                deg = Mathf.Asin(rcdis * gravity / bv2) * Mathf.Rad2Deg / 2.0f;
                cannon.eulerAngles = new Vector3(cannon.eulerAngles.x, cannon.eulerAngles.y, deg);

                //设置 dmRange worldPositon Rotate
                cirDMSP.transform.position = new Vector3(rcx, cirDMSP.transform.position.y, rcz);
                cirDMSP.transform.eulerAngles = new Vector3(90, 0, 0);
            }

            //发射炮弹
            //var shellCon = Instantiate(shell).GetComponent<ShellCon>();
            //shellCon.gameObject.SetActive(true);
            //shellCon.shellInit(shellPoint.position, 
            //    new Vector3(cirDMSP.transform.position.x,transform.position.y, cirDMSP.transform.position.z), 
            //    baseVelocity * Mathf.Cos(deg * Mathf.Deg2Rad), 
            //    gravity);

            var shellCon = ObjPool.Instance.getObj("Shell").GetComponent<ShellCon>();
            shellCon.shellInit(shellPoint.position,
                new Vector3(cirDMSP.transform.position.x, transform.position.y, cirDMSP.transform.position.z),
                baseVelocity * Mathf.Cos(deg * Mathf.Deg2Rad),
                gravity);
            shellCon.startMove();


            //旋转锁定
            roState = MortarRoState.Lock;//设置锁定状态
            nlTime = 0f;

            //声效

            AudioCon.Instance.playClip("launch");

            //冷却
            rangeVis = false;
            cirRGSP.SetActive(false);
            cirDMSP.SetActive(false);
            isLoad = false;
            nlodtime = 0;

            cirDMRend.material.SetColor("_Color", notload);
            cirDMRend.material.SetFloat("load", 0);

        }

        
    }



}
