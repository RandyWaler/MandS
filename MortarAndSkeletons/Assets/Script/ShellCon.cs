using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShellState
{
    onFly, //���ڷ���
    onDisappear, //��ع��� & ��ʧ
    over //��������
}

public class ShellCon : MonoBehaviour
{

    //״̬��ǩ
    ShellState state = ShellState.over; //��ʼ״̬

    //�������
    public float shellRadius = 0.5f;//�뾶 / �����߶�
    public float bfTime = 0.9f;//����Ԥ��ʱ��
    

    float nbfT; //��ǰԤ��ʱ��
    float flyTime;//��ǰ����ʱ��
    float totalTime;//��ʱ��

    float vx;
    float vy;

    Vector3 basePos; //��ʼλ��
    Vector3 tarPos; //Ŀ��λ��

    float moveX;
    float moveZ;
    float moveY;
    float flyPercent;

    float tarY;

    //��غ� ����ģ�� ��ʧ

    Rigidbody rgbody;
    MeshRenderer meshRenderer;
    public float disTime = 1.5f;//��غ���ʧʱ��
    float ndisTime; //��ǰ��ʧʱ��

    private void Awake()
    {

        rgbody = gameObject.GetComponent<Rigidbody>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }


    public void shellInit(Vector3 bpos,Vector3 tpos,float y,float g) //��ʼ�� �����
    {
        //����ת��
        basePos = bpos;
        tarPos = new Vector3(tpos.x,tpos.y+shellRadius,tpos.z);

        vy = y;

        //��ʼ��
        transform.position = basePos;

        nbfT = 0;
        flyTime = 0;
        ndisTime = 0;
        totalTime = 2 * vy / g;

        tarY = 0.5f * vy * vy / g;

        state = ShellState.onFly;

        vx = Vector3.Distance(new Vector3(basePos.x, 0, basePos.z), new Vector3(tarPos.x, 0, tarPos.y)) / totalTime;

        rgbody.isKinematic = true;
        rgbody.useGravity = false;

    }

    private void Update()
    {
        if(state == ShellState.onFly)
        {
            if (nbfT < bfTime) //Ԥ��
            {
                nbfT += Time.deltaTime;
            }else //����
            {

                flyTime += Time.deltaTime;

                if (flyTime >= totalTime)
                {
                    //���
                    gameObject.transform.position = tarPos;
                    state = ShellState.onDisappear;
                    rgbody.isKinematic = false;
                    rgbody.useGravity = true;
                    rgbody.velocity = (tarPos - basePos).normalized * vx;

                    //��Ч
                    AudioCon.Instance.playClip("hit");

                }
                else
                {

                    flyPercent = flyTime / totalTime;

                    //XZƽ�������˶�
                    moveX = basePos.x + (tarPos.x - basePos.x) * flyPercent;
                    moveZ = basePos.z + (tarPos.z - basePos.z) * flyPercent;

                    //Y�����˶�  0~1 ==> sin(0~pi)

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
                state = ShellState.over;
                gameObject.SetActive(false);
            }

            meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, 1-ndisTime / disTime);
        }
    }
}