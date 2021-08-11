using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MortarRoState //�Ȼ�����ת��Ϊ
{
    onRotate, //���ڸ���
    Catch,    //���񵽵�
    Lock      //�����޷���ת��
}


public class MortarCon : MonoBehaviour
{

    //Range��Χ��ʾ���

    public float maxRange=30.0f; //��������(m)
    public float minRange=10.0f; //��С�������(m)

    public float dmRange = 3.0f;//�˺���Χ

    [SerializeField]
    GameObject cirRGSP;

    [SerializeField]
    GameObject cirDMSP;

    SpriteRenderer cirRGRend;
    SpriteRenderer cirDMRend;

    bool rangeVis = false;//�Ƿ�����Χ�鿴


    //��ײ���
    RaycastHit raycast;
    int layerMask;
    float maxR2;
    float minR2;
    float rcx;
    float rcz;
    float lcx;
    float lcz;
    float rcdis;//����
    Vector2 rcnormal;


    //��ת��Ϊ
    MortarRoState roState = MortarRoState.onRotate;
    public float roSpeed = 480.0f;//��ת�����ٶ�(�Ȼ���LookAt �� �ڹܹ�����һ�ٶ�)
    public float catchDeg = 3.0f;//�����������ƫ��
    public float launchLock = 1.5f;//��������ʱ��
    float nlTime;//��ǰ����ʱ��
    float roStep;
    Vector3 lookVec;
    Quaternion lookRo;



    //�ڵ��������

    public float gravity = 9.8f;//�������ٶ�
    public float loadTime = 3.0f;//װ��ʱ��
    public Color hasbeenLoad;
    public Color notload;



    float nlodtime;//��ǰװ��ʱ��
    Transform shellPoint; //�ڵ�λ��
    public GameObject shell;//�ڵ�

    float baseVelocity;//���ٶ�
    float bv2;
    float deg;

    Transform cannon;//�ڹ� --- ���ýǶ�

    bool isLoad = true;//�ڵ��Ƿ���װ��


    private void Awake()
    {

        //���÷��䷶ΧȦSprite
        cirRGSP.transform.localScale = new Vector3(maxRange * 0.2f,maxRange * 0.2f,1);

        cirRGRend = cirRGSP.GetComponent<SpriteRenderer>();

        cirRGRend.material.SetFloat("uvRange", Mathf.Pow((minRange/maxRange)/2.0f,2)); //

        //�����˺���ΧȦSprite

        cirDMSP.transform.localScale = new Vector3(dmRange * 0.2f, dmRange * 0.2f, 1);

        cirDMRend = cirDMSP.GetComponent<SpriteRenderer>();
        cirDMRend.material.SetColor("_Color", hasbeenLoad);

        cirRGSP.SetActive(false);
        cirDMSP.SetActive(false);

        //��ײ���
        maxR2 = maxRange * maxRange;
        minR2 = minRange * minRange;
        layerMask = 1<<LayerMask.NameToLayer("mcollider");

        //�ڵ��������
        cannon = transform.GetChild(0);
        shellPoint = cannon.GetChild(0);
        baseVelocity = Mathf.Sqrt(gravity * maxRange);
        bv2 = baseVelocity * baseVelocity;

        shell.SetActive(false);

    }

    

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            rangeVis = !rangeVis;
            cirRGSP.SetActive(rangeVis);
            cirDMSP.SetActive(rangeVis);

            if(!rangeVis&&roState==MortarRoState.Catch) roState = MortarRoState.onRotate; //����Χ���ɼ�ʱ�����Ѳ�׽��תΪ��ת
        }

        if(roState == MortarRoState.Lock)
        {
            nlTime += Time.deltaTime;
            if (nlTime >= launchLock) roState = MortarRoState.onRotate;
        }

        if (!isLoad)
        { //װ����ȴ
            nlodtime += Time.deltaTime;
            if (nlodtime >= loadTime)
            {
                isLoad = true;
                cirDMRend.material.SetColor("_Color", hasbeenLoad);
                cirDMRend.material.SetFloat("load", 1);
            }
            else cirDMRend.material.SetFloat("load", nlodtime / loadTime);
        }

        if (rangeVis)
        {

            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out raycast,float.MaxValue,layerMask)) {//������ײ���

                //λ�ü���
                rcx = raycast.point.x;
                rcz = raycast.point.z;
                lcx = rcx - transform.position.x;
                lcz = rcz - transform.position.z;

                rcdis = lcx*lcx+lcz*lcz;

                if (rcdis > maxR2)//��ȡ�����Χ
                {
                    rcnormal = new Vector2(lcx, lcz).normalized;
                    rcx = maxRange * rcnormal.x + transform.position.x;
                    rcz = maxRange * rcnormal.y + transform.position.z;
                    rcdis = maxRange;

                }
                else if (rcdis < minR2)
                {
                    rcnormal = new Vector2(lcx, lcz).normalized;
                    rcx = minRange * rcnormal.x + transform.position.x;
                    rcz = minRange * rcnormal.y + transform.position.z;
                    rcdis = minRange;
                }
                else rcdis = Mathf.Sqrt(rcdis);

                //�����Ȼ��ڳ���&�Ƕ�

                if(roState == MortarRoState.onRotate) { //������ת ���Խ��� Catch

                    roStep = roSpeed * Time.deltaTime;
                    lookVec = new Vector3(rcx, gameObject.transform.position.y, rcz) - gameObject.transform.position;
                    //��ת�Ȼ���
                    lookRo = Quaternion.LookRotation(lookVec, Vector3.up);
                    gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, lookRo, roStep);

                    //��ת�ڹ�
                    deg = Mathf.Asin(rcdis * gravity / bv2) * Mathf.Rad2Deg / 2.0f;
                    cannon.eulerAngles = new Vector3(cannon.eulerAngles.x, cannon.eulerAngles.y,
                        Mathf.MoveTowards(cannon.eulerAngles.z,deg, roStep));

                    if (Vector3.Angle(transform.forward, lookVec) < catchDeg && Mathf.Abs(cannon.eulerAngles.z - deg) < catchDeg) roState = MortarRoState.Catch;



                }
                else if(roState == MortarRoState.Catch) //�Ѳ��� 
                {
                    gameObject.transform.LookAt(new Vector3(rcx, gameObject.transform.position.y, rcz), Vector3.up);

                    //�����ڹܽǶ�
                    deg = Mathf.Asin(rcdis * gravity / bv2) * Mathf.Rad2Deg / 2.0f;
                    cannon.eulerAngles = new Vector3(cannon.eulerAngles.x, cannon.eulerAngles.y, deg);

                }//else if(roState == MortarRoState.Lock) �����²��ı��Ȼ��ں��ڹܵ���ת

                //���� dmRange worldPositon Rotate
                cirDMSP.transform.position = new Vector3(rcx, cirDMSP.transform.position.y, rcz);
                cirDMSP.transform.eulerAngles = new Vector3(90, 0, 0);

            }
        }


        if(Input.GetMouseButtonDown(0)&&rangeVis&&isLoad)
        {

            //���볯��  ---  �����ڿ���Range�����̷��䣬��δ��ɸ���

            if(roState == MortarRoState.onRotate)
            {
                gameObject.transform.LookAt(new Vector3(rcx, gameObject.transform.position.y, rcz), Vector3.up);

                //�����ڹܽǶ�
                deg = Mathf.Asin(rcdis * gravity / bv2) * Mathf.Rad2Deg / 2.0f;
                cannon.eulerAngles = new Vector3(cannon.eulerAngles.x, cannon.eulerAngles.y, deg);

                //���� dmRange worldPositon Rotate
                cirDMSP.transform.position = new Vector3(rcx, cirDMSP.transform.position.y, rcz);
                cirDMSP.transform.eulerAngles = new Vector3(90, 0, 0);
            }

            //�����ڵ�
            //var shellCon = Instantiate(shell).GetComponent<ShellCon>();
            //shellCon.gameObject.SetActive(true);
            //shellCon.shellInit(shellPoint.position, 
            //    new Vector3(cirDMSP.transform.position.x,transform.position.y, cirDMSP.transform.position.z), 
            //    baseVelocity * Mathf.Cos(deg * Mathf.Deg2Rad), 
            //    gravity);

            var shellCon = ObjPool.Instance.getObj("Shell").GetComponent<ShellCon>();
            shellCon.shellInit(shellPoint.position,
                new Vector3(cirDMSP.transform.position.x, transform.position.y, cirDMSP.transform.position.z),
                baseVelocity * Mathf.Cos(deg * Mathf.Deg2Rad),
                gravity);
            shellCon.startMove();


            //��ת����
            roState = MortarRoState.Lock;//��������״̬
            nlTime = 0f;

            //��Ч

            AudioCon.Instance.playClip("launch");

            //��ȴ
            rangeVis = false;
            cirRGSP.SetActive(false);
            cirDMSP.SetActive(false);
            isLoad = false;
            nlodtime = 0;

            cirDMRend.material.SetColor("_Color", notload);
            cirDMRend.material.SetFloat("load", 0);

        }

        
    }



}
