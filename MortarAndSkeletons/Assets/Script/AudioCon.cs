using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCon : MonoBehaviour
{
    //����
    public static AudioCon Instance = null;


    //clip��¼
    [System.Serializable]
    public struct ADClip{
        public string adName;
        public AudioClip audioClip;
        public int idx;
    }

    public List<ADClip> clips = new List<ADClip>();

    Dictionary<string, ADClip> dic;

    //clip����
    AudioSource[] audioSource;

   

    private void Awake()
    {
        //����
        if (!Instance) Instance = this;
        else if (Instance != this)
        {
            Destroy(this);
            return;
        }

        //clipsչ��
        dic = new Dictionary<string, ADClip>();
        foreach (var adc in clips) dic.Add(adc.adName, adc);
        audioSource = gameObject.GetComponents<AudioSource>();
    }


    int pidx;
    public void  playClip(string clipName)
    {
        if (!dic.ContainsKey(clipName)) return;
        pidx = dic[clipName].idx;

        audioSource[pidx].Stop();
        audioSource[pidx].clip = dic[clipName].audioClip;
        audioSource[pidx].Play();
    }



}
