using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MyPooler;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarSpawner : MonoBehaviour
{
    

    [SerializeField] private int leftSpawn = -12;
    [SerializeField] private int rightSpawn = 12;

    private float _carSpeed;
    private float _spawnInterval;

    private int direction;
    private string _randTag;
    
    private List<KeyValuePair<string, GameObject>> _poolingObject;
    private ObjectPooler Pooler;

    
    
    private void Start()
    {
        Pooler = ObjectPooler.Instance;
        _poolingObject = new List<KeyValuePair<string, GameObject>>();
        Spawn();
    }

    private void Spawn()
    {
        _carSpeed = Random.Range(10f, 13f);
        _spawnInterval = Random.Range(4f, 5f);
        

        direction = Random.Range(0, 2);
        direction = direction == 0 ? leftSpawn : rightSpawn;

        _randTag = Random.Range(0, 13).ToString() + "car";
        InvokeRepeating("SpawnCar", 0f, _spawnInterval);

        int x = Random.Range(0, 2);
        if (x == 1)
        {
            var coin = Pooler.GetFromPool("coin", transform.position, Quaternion.identity); //spawn coin 
            coin.transform.position = new Vector3(Random.Range(-5, 5), 0.6f, transform.position.z);
            _poolingObject.Add(new KeyValuePair<string, GameObject>("coin", coin));
        }
    }

    private void SpawnCar()
    {
        GameObject car = Pooler.GetFromPool(_randTag, transform.position, Quaternion.Euler(0,90,0));
        
        _poolingObject.Add(new KeyValuePair<string, GameObject>(_randTag, car));
        
        car.transform.position = new Vector3(direction, 0.45f, transform.position.z);
        if (direction == leftSpawn)
        {
            car.transform.rotation = Quaternion.Inverse(car.transform.rotation);
        }
        
        
        car.transform.DOMoveX(-direction, _carSpeed).OnComplete(() => Pooler.ReturnToPool(_randTag, car));
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
