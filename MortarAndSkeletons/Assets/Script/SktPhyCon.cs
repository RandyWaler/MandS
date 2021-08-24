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
    void setBreakDir(float x,float y) //设置破开方向  坐标已转换至本地坐标下
    {
        bdir = new Vector2(-x, -y).normalized;//炮弹的方向取反，再单位化

        rmax = -2;

        foreach(var v in roDirVec)
        {
            tr = Vector2.Dot(bdir, v); //点积判定向量的接近性，取最接近的那一个Ro输出
            if (tr>rmax)
            {
                rmax = tr;
                rdir = v;
            }
        }

        animator.SetFloat(xpHash, bdir.x); //位移混合直接用转换后的单位向量
        animator.SetFloat(ypHash, bdir.y);

        animator.SetFloat(rxpHash, rdir.x);
        animator.SetFloat(rypHash, rdir.y);

    }

    Vector3 obdir;

    public void OnSktBreak(Vector3 pos) //受到世界坐标下炮弹的攻击，被摧毁
    {
        //Match匹配

        obdir = gameObject.transform.InverseTransformPoint(pos);

        setBreakDir(obdir.x, obdir.z);

        animator.SetBool(brHash, true);
    }


}
