using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SktCreater : MonoBehaviour
{

    public static SktCreater Instance = null;

    public List<SktCon> SkeletonS;

    public Transform sktAim;//骷髅兵的移动目标

    public float creatTime = 2.0f;//生成间隔
    public int maxSktNum = 10;//场景中最大骷髅兵数量
    public float randRange = 50.0f;//生成随机圈大小
    float ncTime = 0;

    List<SktCon> onbreakSktS;

    private void Awake()
    {
        if (!Instance) Instance = this; //这里不考虑Awake竞速 Awake阶段不应进行通信
        else if (Instance != this) Destroy(gameObject); //针对重复进入场景，DontDestroyOnLoad，保唯一自毁

        onbreakSktS = new List<SktCon>();
    }


    Vector2 npos;
    private void Update()
    {
        if(SkeletonS.Count<maxSktNum)
        {
            ncTime += Time.deltaTime;
            if(ncTime>=creatTime)
            {
                ncTime = 0;
                SktCon sCon = ObjPool.Instance.getObj("Skeleton").GetComponent<SktCon>();

                sCon.aim = sktAim;
                npos = Random.insideUnitCircle * randRange;
                sCon.transform.position = new Vector3(npos.x, 0, npos.y);
                sCon.startMove();
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
