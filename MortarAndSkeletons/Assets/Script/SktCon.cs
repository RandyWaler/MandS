using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SktCon : MonoBehaviour,PoolObj
{
    public Vector3 aim = new Vector3(-5000,0,-50000);//目标位置

    public float standRange = 2.0f;//距离目标多近时，无需再奔跑移动，可以停下
    float sr2;

    public float roSpeed = 90.0f;//旋转速度
    float roStep;
    Quaternion lookRo;

    Animator animator;
    AnimatorStateInfo stateInfo;
    int runHash = Animator.StringToHash("Run");
    int idleHash = Animator.StringToHash("Idle");


    //Mono---------------------------------------------------------------------------
    private void Awake()
    {
        sr2 = standRange * standRange;
        animator = gameObject.GetComponent<Animator>();
    }


    float dtx;
    float dtz;

    private void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        dtx = aim.x - transform.position.x;
        dtz = aim.z - transform.position.z;

        if (stateInfo.shortNameHash == runHash )
        {

                //旋转跟随到目标位置
            roStep = roSpeed * Time.deltaTime;
            lookRo = Quaternion.LookRotation(new Vector3(dtx, 0, dtz), Vector3.up);
            gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, lookRo, roStep);
            

            if (dtx * dtx + dtz * dtz <= sr2)
            {
                animator.SetBool(runHash, false);
            }

        }
        else if(stateInfo.shortNameHash == idleHash) //暂时骷髅兵只有两个状态，不是 Run 跑动 就是站立状态
        {
            if(dtx*dtx+dtz*dtz>sr2)
            {
                animator.SetBool(runHash, true);
            }
        }
    }

    //被摧毁----------------------------------------------------------------------
    public void onBreak(Transform trans)
    {
        //创建一个表现摧毁效果的 SkelePhyBreaker
        SktPhyCon phyCon = ObjPool.Instance.getObj("SktBreaker").GetComponent<SktPhyCon>();
        phyCon.transform.position = transform.position;
        phyCon.transform.rotation = transform.rotation;

        

        phyCon.gameObject.SetActive(true);

        phyCon.setBreakDir(trans.position);
        phyCon.animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
        phyCon.startMove();

        //挂起
        hangUp();
    }

    public void onBreak(Vector3 pos)
    {
        //创建一个表现摧毁效果的 SkelePhyBreaker
        SktPhyCon phyCon = ObjPool.Instance.getObj("SktBreaker").GetComponent<SktPhyCon>();
        phyCon.transform.position = transform.position;
        phyCon.transform.rotation = transform.rotation;

        phyCon.gameObject.SetActive(true);
        phyCon.setBreakDir(pos);
        phyCon.animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash);
        phyCon.startMove();


        //挂起
        hangUp();
    }

    //ObjPool---------------------------------------------------------------------

    ObjPool belongPool = null;
    public ObjPool BelongPool { get { return belongPool; } set { belongPool = value; } }

    string objName;
    public string ObjName { get { return objName; } set { objName = value; } }

    public GameObject Obj { get { return gameObject; } }

    public void reSetObj()
    {
        
    }

    public void startMove()
    {
        if (aim.x<-4900&&aim.z<-4900)
        {
            animator.SetBool(runHash, false);
            animator.Play(idleHash);
        }
        else
        {
            dtx = aim.x - transform.position.x;
            dtz = aim.z - transform.position.z;

            if (dtx * dtx + dtz * dtz <= sr2)
            {
                animator.SetBool(runHash, false);
                animator.Play(idleHash);
            }
            else
            {
                animator.SetBool(runHash, true);
                animator.Play(runHash);
            }
        }
    }

    public void hangUp()
    {
        SktCreater.Instance.SkeletonS.Remove(this);
        gameObject.SetActive(false);
        belongPool?.backtoPool(this);//返回对象池中
    }
}
