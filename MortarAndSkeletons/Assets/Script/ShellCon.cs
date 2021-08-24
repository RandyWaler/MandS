using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShellState
{
    onFly, //正在飞行
    onDisappear, //落地滚动 & 消失
    over //结束表现
}

public class ShellCon : MonoBehaviour,PoolObj
{

    //状态标签
    ShellState state = ShellState.over; //初始状态

    //发射控制
    public float shellRadius = 0.5f;//半径 / 悬浮高度
    public float bfTime = 0.9f;//发射预卷时间
    

    float nbfT; //当前预卷时间
    float flyTime;//当前飞行时间
    float totalTime;//总时间

    float vx;
    float vy;

    Vector3 basePos; //初始位置
    Vector3 tarPos; //目标位置

    float moveX;
    float moveZ;
    float moveY;
    float flyPercent;

    float tarY;

    //落地后 物理模拟 消失

    Rigidbody rgbody;
    MeshRenderer meshRenderer;
    public float disTime = 1.5f;//落地后消失时间
    float ndisTime; //当前消失时间

    TrailRenderer trailRenderer;

    //伤害范围
    public float dmgRange;

    //ObjPool

    ObjPool belongPool;
    public ObjPool BelongPool { get { return belongPool; } set { belongPool = value; } }

    string objName;
    public string ObjName { get { return objName; } set { objName = value; } }

    public GameObject Obj { get { return gameObject; } }

    private void Awake()
    {

        rgbody = gameObject.GetComponent<Rigidbody>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        trailRenderer = gameObject.GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        if(state == ShellState.onFly)
        {
            if (nbfT < bfTime) //预卷
            {
                nbfT += Time.deltaTime;
            }else //飞行
            {

                flyTime += Time.deltaTime;

                if (flyTime >= totalTime)
                {
                    //落地
                    gameObject.transform.position = tarPos;
                    state = ShellState.onDisappear;
                    rgbody.isKinematic = false;
                    rgbody.useGravity = true;
                    rgbody.velocity = (tarPos - basePos).normalized * vx;

                    //音效
                    AudioCon.Instance.playClip("hit");

                    //判定击中

                    SktCreater.Instance.checkBreak(tarPos, dmgRange*dmgRange);

                }
                else
                {

                    flyPercent = flyTime / totalTime;

                    //XZ平面匀速运动
                    moveX = basePos.x + (tarPos.x - basePos.x) * flyPercent;
                    moveZ = basePos.z + (tarPos.z - basePos.z) * flyPercent;

                    //Y上抛运动  0~1 ==> sin(0~pi)

                    if (flyPercent < 0.5f) //basePos.y ===> tarY
                    {
                        moveY = basePos.y + (tarY - basePos.y) * Mathf.Sin(flyPercent * Mathf.PI);
                    }
                    else //tarY ===> tpos.y
                    {
                        moveY = tarPos.y + (tarY - tarPos.y) * Mathf.Sin(flyPercent * Mathf.PI);
                    }

                    transform.position = new Vector3(moveX, moveY, moveZ);
                }
            }
        }else if(state == ShellState.onDisappear)
        {
            ndisTime += Time.deltaTime;
            if(ndisTime>= disTime)
            {
                hangUp();           
            }

            meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, 1-ndisTime / disTime);
        }
    }


    public void shellInit(Vector3 bpos, Vector3 tpos, float y, float g)
    {
        //参数转移
        basePos = bpos;
        tarPos = new Vector3(tpos.x, tpos.y + shellRadius, tpos.z);

        vy = y;

        //初始化
        
        transform.position = basePos;
        trailRenderer.Clear();

        nbfT = 0;
        flyTime = 0;
        ndisTime = 0;
        totalTime = 2 * vy / g;

        tarY = 0.5f * vy * vy / g;

        vx = Vector3.Distance(new Vector3(basePos.x, 0, basePos.z), new Vector3(tarPos.x, 0, tarPos.y)) / totalTime;
    }

    public void reSetObj()
    {
        rgbody.isKinematic = true;
        rgbody.useGravity = false;
        rgbody.velocity = Vector3.zero;
        trailRenderer.Clear();

        meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, 1.0f);
    }

    public void startMove()
    {
        state = ShellState.onFly;
        gameObject.SetActive(true);
           
    }

    public void hangUp()
    {

        state = ShellState.over;
        gameObject.SetActive(false);
        belongPool.backtoPool(this);
    }
}
