using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    float rcdis2;//未开方距离
    Vector2 rcnormal;

    private void Awake()
    {

        //设置发射范围圈Sprite
        cirRGSP.transform.localScale = new Vector3(maxRange * 0.2f,maxRange * 0.2f,1);

        cirRGRend = cirRGSP.GetComponent<SpriteRenderer>();

        cirRGRend.material.SetFloat("maxRange", maxRange); //shader中计算因为是比值 无所谓换算
        cirRGRend.material.SetFloat("minRange", minRange);

        //设置伤害范围圈Sprite

        cirDMSP.transform.localScale = new Vector3(dmRange * 0.2f, dmRange * 0.2f, 1);

        cirRGSP.SetActive(false);
        cirDMSP.SetActive(false);

        //碰撞相关
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

            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out raycast,float.MaxValue,layerMask)) {//射线碰撞检测

                //位置计算
                rcx = raycast.point.x;
                rcz = raycast.point.z;
                lcx = rcx - transform.position.x;
                lcz = rcz - transform.position.z;

                rcdis2 = lcx*lcx+lcz*lcz;

                if (rcdis2>maxR2)//夹取到最大范围
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
                

                //设置 dmRange 位置
                cirDMSP.transform.position = new Vector3(rcx, cirDMSP.transform.position.y, rcz);


                //设置迫击炮朝向&角度

                gameObject.transform.LookAt(new Vector3(rcx, gameObject.transform.position.y, rcz), Vector3.up);

            }




        }
    }



}
