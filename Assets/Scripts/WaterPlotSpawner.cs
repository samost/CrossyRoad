using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using DG.Tweening;
using MyPooler;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaterPlotSpawner : MonoBehaviour
{

    [SerializeField] private int leftSpawn = -12;
    [SerializeField] private int rightSpawn = 12;

    private float _plotSpeed;
    private float _spawnInterval;
    private float _plotScaleX;

    private int direction;

    

    private ObjectPooler Pooler;
    
    private List<KeyValuePair<string, GameObject>> _poolingObject;
    
   
    
    private void Start()
    {
        Pooler = ObjectPooler.Instance;
        _poolingObject = new List<KeyValuePair<string, GameObject>>();
        Spawn();
    }

    private void Spawn()
    {

        _spawnInterval = Random.Range(2.5f, 4f);

        direction = Random.Range(0, 2);
        direction = direction == 0 ? leftSpawn : rightSpawn;

        InvokeRepeating("SpawnPlot", 0f, _spawnInterval);


        int x = Random.Range(0, 2);
        if (x == 1)
        {
            var coin = Pooler.GetFromPool("coin", transform.position, Quaternion.identity); //spawn coin 
            coin.transform.position = new Vector3(Random.Range(-5, 5), 0.6f, transform.position.z);
            _poolingObject.Add(new KeyValuePair<string, GameObject>("coin", coin));
        }
    }


    private void SpawnPlot()
    {
        _plotSpeed = Random.Range(10f, 15f);
        _plotScaleX = Random.Range(10f, 60f);


        GameObject plot = Pooler.GetFromPool("plot", transform.position, Quaternion.identity);
        _poolingObject.Add(new KeyValuePair<string, GameObject>("plot", plot));

        plot.transform.position = new Vector3(direction, 0.45f, transform.position.z);
        if (direction == leftSpawn)
        {
            plot.transform.rotation = Quaternion.Inverse(plot.transform.rotation);
        }

        plot.transform.DOScaleX(_plotScaleX, 0f);
        plot.transform.DOMoveX(-direction, _plotSpeed).OnComplete(() => Pooler.ReturnToPool("plot", plot));
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