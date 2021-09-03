using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GStoneCon : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    //�ֶ�
    int openHash = Animator.StringToHash("open");
    Animator animator;

    RaycastHit raycast;//���߼��
    int layerMask;


    Transform roChild;//����ת�ڵ�

    public Transform lookTrans;//ע��Ŀ��
    public float outRange = 20.0f;//������ע��Ŀ����ٷ�Χ��
    float outR2;//�˷�
    public float moveSpeed=15.0f;//�ƶ��ٶ�
    public float roSpeed = 120f;//��תע���ٶ�

    Vector3 odir;//��ע��Ŀ�귴�����ƶ��ĵ�λ���� Awake EndDrag ������һ��

    bool candrag = false;//�ܷ���ק��IBeginDragHandler & IDragHandler���ã�ȴ�������߼���Ƿ���е��棬�ڱ���trueʱ��������ײ��������λ�õĲ���
    bool hvdrag = true;//�Ƿ���ק�ˣ�IEndDragHandler��true Update�е���λ��&��λ����Ҫ��
    bool enter = false;//���Enter/Exit������������ק��Update��λ��&��λ��Ϊ����Ҫ��ʱ������openHash
    bool isdrag = false;//��ǰ��һ�ε�Drag�Ƿ���Ч���Ƿ���Ϊhvdrag --- Update��������λ��&��λ ������ֹ

    Vector3 deltaVec;//��ʼ���߻���λ����gameObjecλ�õĲ���(��֤�϶���ƽ�����𲽲���λ)
    Vector3 udVec;//Ӧ�ò�������ʼ��������Drag���𽥱�����
    Vector3 rayHitVec;//���߻��е�λ��(��ӳ�䵽����transform.y)������Update�б�����קЧ��

    public float maxDeltaDis = 0.3f;//������������(С�������������0)
    public float moveDeltaSpeed = 100.0f;//�ƶ������������ٶ�
    float md2;//�˷�


    //temple ����


    Vector3 lookVec;

    float deltaAngle;
    float deltaDis;
    

    //Mono
    private void Awake()
    {
        roChild = transform.GetChild(0);
        animator = roChild.GetComponent<Animator>();
        layerMask = 1 << LayerMask.NameToLayer("mcollider");

        odir = new Vector3(transform.position.x - lookTrans.position.x, 0, transform.position.z - lookTrans.position.z);
        odir = odir.normalized;
        
        outR2 = outRange * outRange;
        md2 = maxDeltaDis * maxDeltaDis;
        
    }

    private void Update()
    {
        if(hvdrag&&lookTrans) //��Ҫ����λ��&��λ
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
            }

        }


        if(isdrag&&candrag) //����Drag��λ�õ�����ί�и�Update��������onDrag��һЩ�ӵ�����ص����棬��ֻ�϶���֡��������������ͣ�ڲ����ϴ��λ��
        {
            lookVec = new Vector3(transform.position.x - rayHitVec.x, 0, transform.position.z - rayHitVec.z); //����
            deltaDis = lookVec.x * lookVec.x + lookVec.z * lookVec.z;

            if (deltaDis <= md2) transform.position = rayHitVec;
            else //��Ҫ�������
            {
                deltaDis = Mathf.Sqrt(deltaDis);
                deltaDis -= moveDeltaSpeed * Time.deltaTime;
                lookVec = lookVec.normalized;
                lookVec *= deltaDis;

                transform.position = rayHitVec + lookVec;


            }

        }
    }

    //Evnet�ӿ�

    public void OnPointerEnter(PointerEventData eventData)
    {
        enter = true;
        if ((!candrag) && (!hvdrag) && (!isdrag)) animator.SetBool(openHash, false);//����ק�Ÿ��跴��
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        enter = false;
        if ((!candrag)&&(!hvdrag)&&(!isdrag))
        {
            Debug.Log("onExit");
            Debug.Log("canDrag:" + candrag);
            Debug.Log("hvDrag:" + hvdrag);
            animator.SetBool(openHash, true);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        
        if (hvdrag) return; //�Ѿ���ק����Updaeû�����ú�λ�÷�λ��������ק
        isdrag = true; //��ק��Ч
        animator.SetBool(openHash, false);//Ӧ�Ա�Ե���ӽ�λ��ק��Exit����OnBeginDrag����
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycast, float.MaxValue, layerMask))
        {
            candrag = true;
            deltaVec = transform.position - raycast.point;
            udVec = deltaVec;
        }
        else candrag = false;

    }
    public void OnDrag(PointerEventData eventData)
    {

        
        if (hvdrag||!isdrag) return;
        //Debug.Log("OnDrag");
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycast, float.MaxValue, layerMask))
        {
            if (!candrag) //��Եλ���ƻأ���onDrag��ĳһ����ʽ������ק
            {
                candrag = true;
                deltaVec = transform.position - raycast.point;
                udVec = deltaVec;
            }
            rayHitVec = new Vector3(raycast.point.x, transform.position.y, raycast.point.z);
            
        }
        else candrag = false;//�ϳ�ȥ�ٻ�������Ҫ�������

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isdrag = false;


        Debug.Log("OnEndDrag");
        hvdrag = true;
        candrag = false;
        odir = new Vector3(transform.position.x - lookTrans.position.x, 0, transform.position.z - lookTrans.position.z);
        odir = odir.normalized;
        
    }
}
