using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SktPhyCon : MonoBehaviour,PoolObj
{


    //动画控制-----------------------------------------------------
    int xpHash = Animator.StringToHash("xpos");
    int ypHash = Animator.StringToHash("ypos");
    int rxpHash = Animator.StringToHash("rxpos");
    int rypHash = Animator.StringToHash("rypos");
    int brHash = Animator.StringToHash("Break");

    int idleHash = Animator.StringToHash("Idle");

    [HideInInspector]
    public Animator animator;

    Vector2[] roDirVec = { new Vector2(0, 1),new Vector2(0,-1),new Vector2(-1,0),new Vector2(1,0),
        new Vector2(0.7071f,0.7071f),new Vector2(-0.7071f,0.7071f),new Vector2(0.7071f,-0.7071f),new Vector2(-0.7071f,-0.7071f) };


    //消失控制----------------------------------------------------
    bool ondisp = false;//是否正在消失
    public float dispTime=2.5f;//透明化消失时间
    public float readTime=3.0f;//预卷时间

    bool setShader = false;

    float dspTime=0;
    float rdTime=0;

    float dpercent;

    public Material bodyMat;
    public Material daggerMat;

    ////Test
    //public Transform testTrans;

    //Mono--------------------------------------------------------
    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        bodyMat = gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material;
        daggerMat = gameObject.GetComponentInChildren<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {

        //逐渐透明化消失
        if(ondisp)
        {
            if (rdTime < readTime) rdTime += Time.deltaTime;
            else
            {
                if(!setShader)
                {
                    setShader = true;
                    bodyMat.shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
                    daggerMat.shader = Shader.Find("Legacy Shaders/Transparent/Diffuse");
                    Debug.Log("Change Shader");
                }
                dspTime += Time.deltaTime;
                if(dspTime>=dispTime)//完成消失
                {
                    hangUp();
                    Debug.Log("Disappear");
                }

                dpercent = 1.0f - dspTime / dispTime;

                bodyMat.color = new Color(bodyMat.color.r, bodyMat.color.g, bodyMat.color.b, dpercent);
                daggerMat.color = new Color(daggerMat.color.r, daggerMat.color.g, daggerMat.color.b,dpercent);

            }
        }


        ////Test
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    reSetObj();
        //    setBreakDir(testTrans.position);
        //    startMove();
        //}

    }


    //PhyCon-------------------------------------------------------

    Vector2 bdir;
    Vector2 rdir;
    Vector3 obdir;
    float rmax;
    float tr;
    public void setBreakDir(Vector3 pos) //设置破开方向  世界坐标，炮弹位置
    {

        obdir = gameObject.transform.InverseTransformPoint(pos);

        bdir = new Vector2(-obdir.x, -obdir.z).normalized;//炮弹的方向取反，再单位化

        rmax = -2;
        rdir = new Vector2(1, 0);

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

        animator.SetFloat(rxpHash, rdir.x);//旋转根据向量点积，取最接近的那一套
        animator.SetFloat(rypHash, rdir.y);

    }

    



    //ObjPool--------------------------------------------------------------
    ObjPool belongPool = null;
    public ObjPool BelongPool { get { return belongPool; } set { belongPool = value; } }

    string objName;
    public string ObjName { get { return objName; } set { objName = value; } }

    public GameObject Obj { get { return gameObject; } }

    public void reSetObj()
    {
        setShader = false;
        ondisp = false;
        bodyMat.shader = Shader.Find("Legacy Shaders/Diffuse");
        daggerMat.shader = Shader.Find("Legacy Shaders/Diffuse");
        bodyMat.color = new Color(bodyMat.color.r, bodyMat.color.g, bodyMat.color.b, 1.0f);
        daggerMat.color = new Color(daggerMat.color.r, daggerMat.color.g, daggerMat.color.b, 1.0f);
        dspTime = 0;
        rdTime = 0;
    }

    public void startMove()
    {
        animator.SetBool(brHash, true);
        ondisp = true;
    }

    public void hangUp()
    {
        animator.Play(idleHash,0);//这里必须趁对象active false之前设置animator状态
        gameObject.SetActive(false);
        belongPool?.backtoPool(this);
    }
}
