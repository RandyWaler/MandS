using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GStoneCon : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    //字段
    int openHash = Animator.StringToHash("open");
    Animator animator;

    RaycastHit raycast;//射线检测
    int layerMask;


    Transform roChild;//子旋转节点

    public Transform lookTrans;//注视目标

    [HideInInspector]
    public float outRange = 20.0f;//保持在注视目标多少范围外
    float outR2;//乘方
    public float moveSpeed=15.0f;//移动速度
    public float roSpeed = 120f;//旋转注视速度

    Vector3 odir;//向注视目标反方向移动的单位向量 Awake EndDrag 会设置一次

    bool candrag = false;//能否拖拽，IBeginDragHandler & IDragHandler设置，却决于射线检测是否击中地面，在被置true时将设置碰撞点与物体位置的差量
    bool hvdrag = true;//是否被拖拽了，IEndDragHandler置true Update中调整位置&方位符合要求
    bool enter = false;//鼠标Enter/Exit，被用于在拖拽后，Update将位置&方位置为符合要求时，设置openHash
    bool isdrag = false;//当前这一次的Drag是否有效，是否不因为hvdrag --- Update正在设置位置&方位 而被阻止

    Vector3 deltaVec;//初始射线击中位置与gameObjec位置的差量(保证拖动的平滑，起步不跳位)
    Vector3 rayHitVec;//射线击中的位置(已映射到物体transform.y)，用于Update中表现拖拽效果

    public float moveDeltaSpeed = 100.0f;//移动补掉差量的速度
    public float maxDeltaDis = 6f;//最大容许差量，拖拽中可能反复出现 OnPointerEnter 和 OnPointerExit 导致错误的开启差量矫正，诱发位置突变
    float md2=0;
    bool outflag = false;//是否超出过最大容许量


    //temple 变量


    Vector3 lookVec;

    float deltaAngle;
    float deltaDis;

    static bool firstDrag = false;
    

    //Mono
    private void Awake()
    {
        roChild = transform.GetChild(0);
        animator = roChild.GetComponent<Animator>();
        layerMask = 1 << LayerMask.NameToLayer("scollider");

        odir = new Vector3(transform.position.x - lookTrans.position.x, 0, transform.position.z - lookTrans.position.z);
        odir = odir.normalized;
        
        outR2 = outRange * outRange;
        md2 = maxDeltaDis * maxDeltaDis;
        
    }

    private void Update()
    {
        if(hvdrag&&lookTrans) //需要调整位置&方位
        {
            lookVec = new Vector3(lookTrans.position.x - transform.position.x, 0, lookTrans.position.z - transform.position.z);
            deltaAngle = Vector3.Angle(roChild.forward, lookVec);
            if (deltaAngle >= 1.0f)
            {
                roChild.rotation = Quaternion.RotateTowards(roChild.rotation,
                    Quaternion.LookRotation(lookVec, Vector3.up), roSpeed * Time.deltaTime);
            }
            else roChild.LookAt(new Vector3(lookTrans.position.x,transform.position.y,lookTrans.position.z),Vector3.up);


            deltaDis = lookVec.x * lookVec.x + lookVec.z * lookVec.z;
            if(deltaDis<=outR2)
            {
                transform.Translate(odir * moveSpeed * Time.deltaTime);
            }


            if (deltaDis >= outR2 && deltaAngle < 1.0f)
            {
                hvdrag = false;
                animator.SetBool(openHash, !enter);
                GSSquare.Instance.gStones.Add(this);
            }

        }


        if(isdrag&&candrag) //这里Drag中位置的设置委托给Update，而不是onDrag，一些从地面外回到地面，但只拖动几帧的情况，物体可能停在差量较大的位置
        {

            lookVec = new Vector3(transform.position.x - rayHitVec.x, 0, transform.position.z - rayHitVec.z); //盗用
            deltaDis = lookVec.x * lookVec.x + lookVec.z * lookVec.z;

            if (!outflag)
            {
                if (deltaDis >= md2)
                {
                    outflag = true;
                }
            }
            else if (enter&&deltaDis<md2) outflag = false;//如果超出容许差量，必须重新enter

            if (enter||!outflag) transform.position = rayHitVec+deltaVec;
            else //需要补齐差量
            {
                deltaDis = Mathf.Sqrt(deltaDis);
                deltaDis -= moveDeltaSpeed * Time.deltaTime;
                lookVec = lookVec.normalized;
                lookVec *= deltaDis;
                transform.position = rayHitVec + lookVec;
                deltaVec = new Vector3(transform.position.x - raycast.point.x, 0, transform.position.z - raycast.point.z);//重计算差量
            }

        }
    }

    //Evnet接口

    public void OnPointerEnter(PointerEventData eventData)
    {
        enter = true;
        if ((!candrag) && (!hvdrag) && (!isdrag)&& !MortarCon.Instance.rangeVis)
        {
            animator.SetBool(openHash, false);//能拖拽才给予反馈
            if (!firstDrag) Text3DCon.Instance.setTxt(transform.position, "Drag");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        enter = false;
        if (!firstDrag&&!MortarCon.Instance.rangeVis) Text3DCon.Instance.disVisable();
        if ((!candrag) && (!hvdrag) && (!isdrag)) animator.SetBool(openHash, true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
        
        if (hvdrag||MortarCon.Instance.rangeVis) return; //已经拖拽过，Updae没有设置好位置方位，不能拖拽，Mortar开启范围显示不能拖拽
        isdrag = true; //拖拽有效
        animator.SetBool(openHash, false);//应对边缘，从角位拖拽，Exit先于OnBeginDrag触发
        GSSquare.Instance.sprite.enabled = true;
        GSSquare.Instance.gStones.Remove(this);
        if(!firstDrag)
        {
            firstDrag = true;
            if(Text3DCon.Instance.txtMesh.text=="Drag") Text3DCon.Instance.disVisable();
        }
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycast, float.MaxValue, layerMask))
        {
            candrag = true;
            deltaVec = new Vector3(transform.position.x - raycast.point.x, 0, transform.position.z - raycast.point.z);
        }
        else candrag = false;

    }
    public void OnDrag(PointerEventData eventData)
    {
        if (hvdrag||!isdrag) return;
        //Debug.Log("OnDrag");
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycast, float.MaxValue, layerMask))
        {
            if (!candrag) //边缘位置移回，从onDrag的某一刻正式开启拖拽
            {
                candrag = true;
                deltaVec = new Vector3(transform.position.x - raycast.point.x,0, transform.position.z - raycast.point.z);
            }
            rayHitVec = new Vector3(raycast.point.x, transform.position.y, raycast.point.z);
            
        }
        else candrag = false;//拖出去再回来，需要重算差量

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");
        isdrag = false;
        hvdrag = true;
        candrag = false;
        GSSquare.Instance.sprite.enabled = false;
        odir = new Vector3(transform.position.x - lookTrans.position.x, 0, transform.position.z - lookTrans.position.z);
        odir = odir.normalized;
        
    }
}
