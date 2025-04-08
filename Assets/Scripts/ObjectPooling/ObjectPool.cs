using UnityEngine;
using System.Collections.Generic;

public class ObjectPool
{
	private PoolableObject prefab;
	private int size;
	private List<PoolableObject> available;
	private ObjectPool(PoolableObject prefab, int size)
	{
		this.prefab = prefab;
		this.size = size;
		available = new(size);
	}
	public static ObjectPool CreateInstance(PoolableObject prefab, int size)
	{
		ObjectPool pool = new(prefab, size);
		// put all the child under the game object pool, help with scene management
		GameObject gameObj = new(prefab.name + "Pool");
		pool.CreateObject(gameObj.transform, size);
		return pool;
	}

	private void CreateObject(Transform transform, int size)
	{
		for (int i = 0; i < size; i++)
		{
			PoolableObject poolObj = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
			poolObj.Parent = this;
			poolObj.gameObject.SetActive(false);
		}
	}

	public void ReturnObjectToPool(PoolableObject poolObj)
	{
		available.Add(poolObj);
	}
	public PoolableObject GetObject()
	{
		if (available.Count > 0)
		{
			PoolableObject first = available[0];
			available.RemoveAt(0);
			first.gameObject.SetActive(true);
			return first;
		}
		else
		{
			Debug.Log("Could not get an object from pool");
			return null; // could return null if you sure the pool never could be configure wrong, or your game is okay with it being null. But you have to validate the value before using.

			// or you could create new object expand the pool size then return new. This create new object at run time could introduce stutter
		}
	}
}
