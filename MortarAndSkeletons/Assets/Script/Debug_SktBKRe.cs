using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_SktBKRe : MonoBehaviour
{
    public SktPhyCon skt;
    public SktCon skeleton;
    // Update is called once per frame
    void Update()
    {
        //Test
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //skt.reSetObj();
            //skt.gameObject.SetActive(true);
            //skt.setBreakDir(transform.position);
            //skt.startMove();

            skeleton.onBreak(gameObject.transform);

        }
    }
}
