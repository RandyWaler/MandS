using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SktCreater : MonoBehaviour
{

    public static SktCreater Instance = null;

    public List<SktCon> SkeletonS;

    public Transform sktAim;//���ñ����ƶ�Ŀ��

    public float creatTime = 2.0f;//���ɼ��
    public int maxSktNum = 10;//������������ñ�����
    public float randRange = 50.0f;//�������Ȧ��С
    float ncTime = 0;

    List<SktCon> onbreakSktS;

    private void Awake()
    {
        if (!Instance) Instance = this; //���ﲻ����Awake���� Awake�׶β�Ӧ����ͨ��
        else if (Instance != this) Destroy(gameObject); //����ظ����볡����DontDestroyOnLoad����Ψһ�Ի�

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
    public void checkBreak(Vector3 shell,float dmgR2) //��鵱ǰ�������Ƿ������ñ�������
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

        foreach(var skt in onbreakSktS) //��Ҫһ�߱���һ���Ƴ�
        {
            skt.onBreak(shell);
        }
    }




}