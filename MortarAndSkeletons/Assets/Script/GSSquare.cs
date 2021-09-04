using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSSquare : MonoBehaviour
{

    public int edg = 70;

    public int outRange = 26;

    public static GSSquare Instance = null;

    [HideInInspector]
    public SpriteRenderer sprite;

    [HideInInspector]
    public List<GStoneCon> gStones;

    private void Awake()
    {
        if (!Instance) Instance = this;//���ﲻ����Awake���� Awake�׶β�Ӧ����ͨ��
        else if (Instance != this)
        {
            Destroy(gameObject);//����ظ����볡����DontDestroyOnLoad����Ψһ�Ի�
            return;
        }

        transform.localScale = new Vector3(edg * 0.1f, edg * 0.1f, 1);

        sprite = gameObject.GetComponent<SpriteRenderer>();

        sprite.enabled = false;

        sprite.material.SetFloat("uvRange", Mathf.Pow(((float)outRange / (float)edg), 2));

        var gsArr = GameObject.FindObjectsOfType<GStoneCon>();

        gStones = new List<GStoneCon>();

    }

}
