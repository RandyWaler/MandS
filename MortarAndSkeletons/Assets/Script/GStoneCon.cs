using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GStoneCon : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    //字段
    int openHash = Animator.StringToHash("open");
    Animator animator;

    bool candrag = true;//能否拖拽，边缘位置可能鼠标没有击中地面但Enter进来此时不能拖动 --- 刚刚完成拖动Update没有置好位置&方位不能拖动
    bool hvdrag = false;//是否被拖拽了，为true MouseEixt不会Anim Open并交由Update处理

    Vector3 deltaVec;//初始射线击中位置与gameObjec位置的差量(保证拖动的平滑，起步不跳位)

    RaycastHit raycast;
    int layerMask;
    

    //Mono
    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        layerMask = 1 << LayerMask.NameToLayer("mcollider");
    }

    //Evnet接口

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(candrag) animator.SetBool(openHash, false);//能拖拽才给予反馈
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
        candrag = false;//需要等待Update设置位置&方位
    }
}
