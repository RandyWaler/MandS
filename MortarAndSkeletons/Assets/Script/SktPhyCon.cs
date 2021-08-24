using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SktPhyCon : MonoBehaviour
{

    int xpHash = Animator.StringToHash("xpos");
    int ypHash = Animator.StringToHash("ypos");
    int rxpHash = Animator.StringToHash("rxpos");
    int rypHash = Animator.StringToHash("rypos");
    int brHash = Animator.StringToHash("Break");

    Animator animator;

    Vector2[] roDirVec = { new Vector2(0, 1),new Vector2(0,-1),new Vector2(-1,0),new Vector2(1,0),
        new Vector2(0.7071f,0.7071f),new Vector2(-0.7071f,0.7071f),new Vector2(0.7071f,-0.7071f),new Vector2(-0.7071f,-0.7071f) };


    public RuntimeAnimatorController animatorController;

    //Test
    public Transform testTrans;

    //Mono--------------------------------------------------------
    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        //Test
        if(Input.GetKeyDown(KeyCode.Space))
        {
          Debug.Log("onSpace");

          animator.runtimeAnimatorController = animatorController;

          animator.SetBool(brHash, true);
        }

        if(animator.GetBool(brHash))
        {
            obdir = gameObject.transform.InverseTransformPoint(testTrans.position);

            setBreakDir(obdir.x, obdir.z);
        }

    }


    //PhyCon-------------------------------------------------------

    Vector2 bdir;
    Vector2 rdir;
    float rmax;
    float tr;
    void setBreakDir(float x,float y) //�����ƿ�����  ������ת��������������
    {
        bdir = new Vector2(-x, -y).normalized;//�ڵ��ķ���ȡ�����ٵ�λ��

        rmax = -2;

        foreach(var v in roDirVec)
        {
            tr = Vector2.Dot(bdir, v); //����ж������Ľӽ��ԣ�ȡ��ӽ�����һ��Ro���
            if (tr>rmax)
            {
                rmax = tr;
                rdir = v;
            }
        }

        animator.SetFloat(xpHash, bdir.x); //λ�ƻ��ֱ����ת����ĵ�λ����
        animator.SetFloat(ypHash, bdir.y);

        animator.SetFloat(rxpHash, rdir.x);
        animator.SetFloat(rypHash, rdir.y);

    }

    Vector3 obdir;

    public void OnSktBreak(Vector3 pos) //�ܵ������������ڵ��Ĺ��������ݻ�
    {
        //Matchƥ��

        obdir = gameObject.transform.InverseTransformPoint(pos);

        setBreakDir(obdir.x, obdir.z);

        animator.SetBool(brHash, true);
    }


}
