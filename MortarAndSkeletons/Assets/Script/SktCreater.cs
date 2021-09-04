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
                sCon.aim = MortarCon.Instance.transform;

                Debug.Log(GSSquare.Instance.gStones.Count);
                sCon.transform.position = GSSquare.Instance.gStones[Random.Range(0, GSSquare.Instance.gStones.Count)].transform.position;
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
