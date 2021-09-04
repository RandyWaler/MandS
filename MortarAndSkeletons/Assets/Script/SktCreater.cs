using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SktCreater : MonoBehaviour
{

    public static SktCreater Instance = null;

    public List<SktCon> SkeletonS;

    //public Transform sktAim;//���ñ����ƶ�Ŀ��

    public float creatTime = 2.0f;//���ɼ��
    public int maxSktNum = 10;//������������ñ�����
    float ncTime = 0;

    List<SktCon> onbreakSktS;

    private void Awake()
    {
        if (!Instance) Instance = this; //���ﲻ����Awake���� Awake�׶β�Ӧ����ͨ��
        else if (Instance != this) Destroy(gameObject); //����ظ����볡����DontDestroyOnLoad����Ψһ�Ի�

        onbreakSktS = new List<SktCon>();
    }


    Vector3 gspos;//Ĺ����λ��
    Vector3 morpos;//�Ȼ��ڵ�λ��
    Vector2 npos;//�Ȼ���---��ģ��ĵ�λ����
    Vector3 tarpos;//���ñ���Ŀ��λ��

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

                //���180��Բ���ڵ�һ��Ŀ���
                npos = new Vector2(gspos.x - morpos.x, gspos.z - morpos.z).normalized;

                roAngle = Random.Range(-90, 91);
                cosra = Mathf.Cos(roAngle * Mathf.Deg2Rad);
                sinra = Mathf.Sin(roAngle * Mathf.Deg2Rad);

                tarpos = new Vector3(npos.x * cosra + npos.y * sinra, 0, npos.x * (-sinra) + npos.y * cosra); //ͨ����Y�����ת���󣬽���λ������ת[-90,90]��

                tarpos *= Random.Range(MortarCon.Instance.minRange + 2, MortarCon.Instance.maxRange - 1);


                sCon.aim = tarpos;


                //����λ�ã���������
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
