using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SktPhyCon : MonoBehaviour,PoolObj
{


    //��������-----------------------------------------------------
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


    //��ʧ����----------------------------------------------------
    bool ondisp = false;//�Ƿ�������ʧ
    public float dispTime=2.5f;//͸������ʧʱ��
    public float readTime=3.0f;//Ԥ��ʱ��

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

        //��͸������ʧ
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
                if(dspTime>=dispTime)//�����ʧ
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
    public void setBreakDir(Vector3 pos) //�����ƿ�����  �������꣬�ڵ�λ��
    {

        obdir = gameObject.transform.InverseTransformPoint(pos);

        bdir = new Vector2(-obdir.x, -obdir.z).normalized;//�ڵ��ķ���ȡ�����ٵ�λ��

        rmax = -2;
        rdir = new Vector2(1, 0);

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

        animator.SetFloat(rxpHash, rdir.x);//��ת�������������ȡ��ӽ�����һ��
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
        animator.Play(idleHash,0);//�������ö���active false֮ǰ����animator״̬
        gameObject.SetActive(false);
        belongPool?.backtoPool(this);
    }
}
