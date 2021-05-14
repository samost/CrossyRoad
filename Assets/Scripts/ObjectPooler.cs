using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPooler
{
	public class ObjectPooler : MonoBehaviour
	{
		[System.Serializable]
		public class Pool
		{
			public string tag;
			public GameObject prefab;
			public int amount;
			public bool shouldExpandPool = true;
			public int extensionLimit;
		}

		private static ObjectPooler _instance;
		public static ObjectPooler Instance
		{
			get
			{
				if (_instance == null)
				{
					GameObject go = new GameObject("ObjectPooler");
					go.AddComponent<ObjectPooler>();
				}
				return _instance;
			}
		}

		void Awake()
		{
			_instance = this;
			if(!shouldDestroyOnLoad)
				DontDestroyOnLoad(this);
		}
		

		public bool shouldDestroyOnLoad = false;
		private int extensionSize;
		public List<Pool> pools;
		public Dictionary<string, Transform> parents;
		public Dictionary<string, Queue<GameObject>> poolDictionary;

		void Start()
		{
			CreatePool();
		}

		public GameObject GetFromPool(string tag, Vector3 position, Quaternion rotation)
		{
			if (!poolDictionary.ContainsKey(tag))
			{
				Debug.Log("Pool tag not found!");
				return null;
			}

			GameObject o = null;
			if (poolDictionary[tag].Count > 0)
			{
				o = poolDictionary[tag].Dequeue();
			}
			else
			{
				Pool currentPool = null;
				float extensionLimit = 0f;
				bool shouldExpandPool = false;

				foreach (Pool p in pools)
				{
					if (p.tag == tag)
					{
						currentPool = p;
						extensionLimit = p.extensionLimit;
						shouldExpandPool = p.shouldExpandPool;
						break;
					}
				}
				if (shouldExpandPool)
				{
					if(extensionLimit > 0)
                    {
						if (extensionSize < extensionLimit)
						{
							o = IncrementPool(currentPool);
							Debug.Log(tag + " pool incremented!");
							extensionSize++;
						}
						else
						{
							Debug.Log("You have no room left for extension on your pool: " + tag + ".");
							return null;
						}			
					}
                    else
                    {
						o = IncrementPool(currentPool);
						Debug.Log(tag + " pool incremented!");
						extensionSize++;
					}		
				}
				else
				{
					Debug.Log("You have no object left on your pool: " + tag + ".");
					return null;
				}		
			}
			o.SetActive(true);
			o.transform.position = position;
			o.transform.rotation = rotation;
			PooledObjInterface pooledObj = o.GetComponent<PooledObjInterface>();

			if(pooledObj != null)
			{
				pooledObj.OnObjectPooled();
			}
			return o;
		}

		public void ReturnToPool(string tag, GameObject o)
		{
			if (!poolDictionary.ContainsKey(tag))
			{
				Debug.Log("Pool tag not found!");
				return;
			}
			poolDictionary[tag].Enqueue(o);
			o.SetActive(false);
		}

		void CreatePool()
		{
			poolDictionary = new Dictionary<string, Queue<GameObject>>();
			parents = new Dictionary<string, Transform>();

			foreach (Pool pool in pools)
			{
				GameObject poolObject = new GameObject(pool.tag + "_Pool");
				poolObject.transform.SetParent(this.transform);
				parents.Add(pool.tag, poolObject.transform);
				Queue<GameObject> objectPool = new Queue<GameObject>();
				for (int i = 0; i < pool.amount; i++)
				{
					GameObject o = Instantiate(pool.prefab);
					o.SetActive(false);
					objectPool.Enqueue(o);
					o.transform.SetParent(poolObject.transform);
					o.transform.position = new Vector3(0, -100, 0);
				}
				poolDictionary.Add(pool.tag, objectPool);
			}
		}

		GameObject IncrementPool(Pool p)
		{
			GameObject objectToIncrement = p.prefab;
			GameObject obj = Instantiate(objectToIncrement);
			obj.transform.SetParent(parents[p.tag]);
			return obj;
		}
		
		
	}
}
