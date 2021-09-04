using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Text3DCon : MonoBehaviour
{
    public static Text3DCon Instance=null;

    public Color baseColor = Color.red;

    [HideInInspector]
    public MeshRenderer meshRenderer;
    [HideInInspector]
    public TextMesh txtMesh;

    Transform ctag;

    private void Awake()
    {
        if (!Instance) Instance = this;//不考虑Awake竞速，Awake阶段不应通信
        else if(Instance!=this)
        {
            Destroy(gameObject); //针对重复进入场景+DontDestroyOnLoad 保唯一自毁
            return;
        }

        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        txtMesh = gameObject.GetComponent<TextMesh>();
        meshRenderer.enabled = false;
        ctag = transform.GetChild(0);
        ctag.gameObject.SetActive(false);
    }

    
    public void setTxt(Vector3 pos, string str, int overHeigh = 6)
    {
        txtMesh.text = str;
        txtMesh.color = baseColor;
        meshRenderer.enabled = true;
        gameObject.transform.position = pos + Vector3.up * overHeigh;
        //transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z));
        transform.forward = Camera.main.transform.forward;

        ctag.gameObject.SetActive(true);
        ctag.LookAt(pos, Vector3.forward);
    }

    public void disVisable()
    {
        meshRenderer.enabled = false;
        ctag.gameObject.SetActive(false);
    }

}
