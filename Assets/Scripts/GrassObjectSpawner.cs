using System;
using System.Collections;
using System.Collections.Generic;
using MyPooler;
using UnityEngine;
using Random = UnityEngine.Random;


public class GrassObjectSpawner : MonoBehaviour
{
    
    private ObjectPooler Pooler;

    private List<KeyValuePair<string, GameObject>> _poolingObject;

    private List<int> pointSpawnX;

   

    private void Start()
    {
        _poolingObject = new List<KeyValuePair<string, GameObject>>();
        Pooler = ObjectPooler.Instance;
        pointSpawnX  = new List<int>();
        Spawn();
    }

    private void Spawn()
    {
        int countTrees = Random.Range(2, 5);


       

        for (int j = -6; j < 6; j++)
        {
            pointSpawnX.Add(j);
        }

        for (int i = 0; i < countTrees; i++)
        {
            string randTag = Random.Range(0, 4).ToString() + "tree";

            int randX = pointSpawnX[Random.Range(0, pointSpawnX.Count)];

            GameObject tree = Pooler.GetFromPool(randTag,
                new Vector3(randX, transform.position.y, transform.position.z),
                Quaternion.Euler(-90, 0, 0));

            _poolingObject.Add(new KeyValuePair<string, GameObject>(randTag, tree));

            pointSpawnX.Remove(randX);
        }

        int x = Random.Range(0, 2);
        if (x == 1)
        {
            var coin = Pooler.GetFromPool("coin", transform.position, Quaternion.identity);
            coin.transform.position =
                new Vector3(pointSpawnX[Random.Range(0, pointSpawnX.Count)], 0.6f, transform.position.z);
            _poolingObject.Add(new KeyValuePair<string, GameObject>("coin", coin));
        }
    }

    private void OnDisable()
    {
        if (!GameController.IsFirstLaunch)
        {
            foreach (var obj in _poolingObject)
            {
                if (obj.Value != null)
                {
                    Pooler.ReturnToPool(obj.Key, obj.Value);
                }
            }
        }
    }
}