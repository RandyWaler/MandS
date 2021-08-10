using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarCon : MonoBehaviour
{

    //Range��Χ��ʾ���

    public float maxRange=30.0f; //��������(m)
    public float minRange=10.0f; //��С�������(m)

    public float dmRange = 3.0f;//�˺���Χ

    [SerializeField]
    GameObject cirRGSP;

    [SerializeField]
    GameObject cirDMSP;

    SpriteRenderer cirRGRend;

    bool rangeVis = false;//�Ƿ�����Χ�鿴


    //��ײ���
    RaycastHit raycast;
    int layerMask;
    float maxR2;
    float minR2;
    float rcx;
    float rcz;
    float lcx;
    float lcz;
    float rcdis2;//δ��������
    Vector2 rcnormal;

    private void Awake()
    {

        //���÷��䷶ΧȦSprite
        cirRGSP.transform.localScale = new Vector3(maxRange * 0.2f,maxRange * 0.2f,1);

        cirRGRend = cirRGSP.GetComponent<SpriteRenderer>();

        cirRGRend.material.SetFloat("maxRange", maxRange); //shader�м�����Ϊ�Ǳ�ֵ ����ν����
        cirRGRend.material.SetFloat("minRange", minRange);

        //�����˺���ΧȦSprite

        cirDMSP.transform.localScale = new Vector3(dmRange * 0.2f, dmRange * 0.2f, 1);

        cirRGSP.SetActive(false);
        cirDMSP.SetActive(false);

        //��ײ���
        maxR2 = maxRange * maxRange;
        minR2 = minRange * minRange;
        layerMask = 1<<LayerMask.NameToLayer("mcollider");
    }

    

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            rangeVis = !rangeVis;
            cirRGSP.SetActive(rangeVis);
            cirDMSP.SetActive(rangeVis);
        }

        if(rangeVis)
        {

            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out raycast,float.MaxValue,layerMask)) {//������ײ���

                //λ�ü���
                rcx = raycast.point.x;
                rcz = raycast.point.z;
                lcx = rcx - transform.position.x;
                lcz = rcz - transform.position.z;

                rcdis2 = lcx*lcx+lcz*lcz;

                if (rcdis2>maxR2)//��ȡ�����Χ
                {
                    rcnormal = new Vector2(lcx, lcz).normalized;
                    rcx = maxRange * rcnormal.x + transform.position.x;
                    rcz = maxRange * rcnormal.y + transform.position.z;

                }else if(rcdis2<minR2)
                {
                    rcnormal = new Vector2(lcx, lcz).normalized;
                    rcx = minRange * rcnormal.x + transform.position.x;
                    rcz = minRange * rcnormal.y + transform.position.z;
                }
                

                //���� dmRange λ��
                cirDMSP.transform.position = new Vector3(rcx, cirDMSP.transform.position.y, rcz);


                //�����Ȼ��ڳ���&�Ƕ�

                gameObject.transform.LookAt(new Vector3(rcx, gameObject.transform.position.y, rcz), Vector3.up);

            }




        }
    }



}
