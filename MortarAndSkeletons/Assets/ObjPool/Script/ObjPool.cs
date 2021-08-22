using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PoolObj {
	void reSetObj();//重置Obj
	void startMove();//开始运作
	void hangUp();//停止运作并挂起
	ObjPool BelongPool { set; get; }//所属对象池
	string ObjName { set; get; }//对象名称
	GameObject Obj { get; }//返回所属gameObject
}


public class ObjPool:MonoBehaviour{

	public static ObjPool Instance = null;

	Dictionary<string, List<GameObject>> pool;

    private void Awake()
    {
		if (!Instance) Instance = this;
		else if (Instance != this) Destroy(gameObject);
		

	}

    private void Start()
	{
		pool = new Dictionary<string, List<GameObject>>();
	}
	
	public GameObject getObj(string objname)
	{
		GameObject outobj;
		if (pool.ContainsKey(objname) && pool[objname].Count > 0)
		{
			outobj = pool[objname][0];
			pool[objname].Remove(outobj);
			outobj.GetComponent<PoolObj>().reSetObj();
			
		}
		else
		{
			outobj = Instantiate(Resources.Load(objname) as GameObject);
			outobj.GetComponent<PoolObj>().BelongPool = this;
			outobj.GetComponent<PoolObj>().ObjName = objname;
		}
		return outobj;
	}
	public void backtoPool(PoolObj backobj)
	{
		if (pool.ContainsKey(backobj.ObjName))
		{
			if (!pool[backobj.ObjName].Contains(backobj.Obj)) //这里要防止重复入池  一些包含子弹的射击类游戏常有此状况
			{
				pool[backobj.ObjName].Add(backobj.Obj);
			}
		}
		else
		{
			pool.Add(backobj.ObjName, new List<GameObject>());
			pool[backobj.ObjName].Add(backobj.Obj);
		}

		backobj.Obj.SetActive(false);
	}
	
	
}
