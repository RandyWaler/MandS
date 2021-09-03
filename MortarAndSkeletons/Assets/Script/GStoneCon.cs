using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GStoneCon : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    //�ֶ�
    int openHash = Animator.StringToHash("open");
    Animator animator;

    bool candrag = true;//�ܷ���ק����Եλ�ÿ������û�л��е��浫Enter������ʱ�����϶� --- �ո�����϶�Updateû���ú�λ��&��λ�����϶�
    bool hvdrag = false;//�Ƿ���ק�ˣ�Ϊtrue MouseEixt����Anim Open������Update����

    Vector3 deltaVec;//��ʼ���߻���λ����gameObjecλ�õĲ���(��֤�϶���ƽ�����𲽲���λ)

    RaycastHit raycast;
    int layerMask;
    

    //Mono
    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        layerMask = 1 << LayerMask.NameToLayer("mcollider");
    }

    //Evnet�ӿ�

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(candrag) animator.SetBool(openHash, false);//����ק�Ÿ��跴��
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!hvdrag) animator.SetBool(openHash, true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!candrag) return;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycast, float.MaxValue, layerMask))
        {
            candrag = true;
            hvdrag = true;
            deltaVec = transform.position - raycast.point;


        }
        else candrag = false;

    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!candrag) return;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycast, float.MaxValue, layerMask))
        {
            transform.position = new Vector3(raycast.point.x + deltaVec.x, transform.position.y, raycast.point.z + deltaVec.z);
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        candrag = false;//��Ҫ�ȴ�Update����λ��&��λ
    }
}
