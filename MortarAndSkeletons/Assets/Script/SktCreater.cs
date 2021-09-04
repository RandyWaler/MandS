using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SktCreater : MonoBehaviour
{

    public static SktCreater Instance = null;

    public List<SktCon> SkeletonS;

    //public Transform sktAim;//骷髅兵的移动目标

    public float creatTime = 2.0f;//生成间隔
    public int maxSktNum = 10;//场景中最大骷髅兵数量
    float ncTime = 0;

    List<SktCon> onbreakSktS;

    private void Awake()
    {
        if (!Instance) Instance = this; //这里不考虑Awake竞速 Awake阶段不应进行通信
        else if (Instance != this) Destroy(gameObject); //针对重复进入场景，DontDestroyOnLoad，保唯一自毁

        onbreakSktS = new List<SktCon>();
    }


    Vector3 gspos;//墓碑的位置
    Vector3 morpos;//迫击炮的位置
    Vector2 npos;//迫击炮---》模板的单位向量
    Vector3 tarpos;//骷髅兵的目标位置

    int roAngle;
    float cosra;
    float sinra;
    private void Update()
    {
        if(SkeletonS.Count<maxSktNum)
        {
            ncTime += Time.deltaTime;
            if(ncTime>=creatTime)
            {
                ncTime = 0;
                SktCon sCon = ObjPool.Instance.getObj("Skeleton").GetComponent<SktCon>();

                gspos = GSSquare.Instance.gStones[Random.Range(0, GSSquare.Instance.gStones.Count)].transform.position;
                morpos = MortarCon.Instance.transform.position;

                //随机180度圆域内的一个目标点
                npos = new Vector2(gspos.x - morpos.x, gspos.z - morpos.z).normalized;

                roAngle = Random.Range(-90, 91);
                cosra = Mathf.Cos(roAngle * Mathf.Deg2Rad);
                sinra = Mathf.Sin(roAngle * Mathf.Deg2Rad);

                tarpos = new Vector3(npos.x * cosra + npos.y * sinra, 0, npos.x * (-sinra) + npos.y * cosra); //通过绕Y轴的旋转矩阵，将单位向量旋转[-90,90]度

                tarpos *= Random.Range(MortarCon.Instance.minRange + 2, MortarCon.Instance.maxRange - 1);


                sCon.aim = tarpos;


                //设置位置，旋向，启动
                sCon.transform.position = gspos;
                sCon.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                sCon.gameObject.SetActive(true);
                sCon.startMove();

                SkeletonS.Add(sCon);
            }
        }
    }

    float dtx;
    float dtz;
    public void checkBreak(Vector3 shell,float dmgR2) //检查当前场景中是否有骷髅兵被命中
    {
        onbreakSktS.Clear();

        foreach(var skt in SkeletonS)
        {
            dtx = shell.x - skt.transform.position.x;
            dtz = shell.z - skt.transform.position.z;

            if(dtx*dtx+dtz*dtz<=dmgR2)
            {
                onbreakSktS.Add(skt);
            }
        }

        foreach(var skt in onbreakSktS) //不要一边遍历一边移除
        {
            skt.onBreak(shell);
        }
    }




}
