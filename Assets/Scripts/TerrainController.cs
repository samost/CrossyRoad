using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MyPooler;

public class TerrainController : MonoBehaviour
{
    public int minZ = 1;
    public int lineAhead = 40;
    public int lineBehind = 20;

    public GameObject[] linePrefabs;

    private Dictionary<int, GameObject> lines;

    private GameObject player;

    private ObjectPooler Pooler;

    private List<string> _tags;

    private Queue<KeyValuePair<string, GameObject>> _poolingObject;
    

    public void Start()
    {
        _tags = new List<string> {"grass", "water", "road"};


        player = GameObject.FindGameObjectWithTag("Player");
        lines = new Dictionary<int, GameObject>();
        Pooler = ObjectPooler.Instance;
        _poolingObject = new Queue<KeyValuePair<string, GameObject>>();
        GameController.IsFirstLaunch = false;
    }

    public void Update()
    {
        var playerZ = (int) player.transform.position.z;

        for (var z = Mathf.Max(minZ, playerZ - lineBehind); z <= playerZ + lineAhead; z += 1)
        {

            if (!lines.ContainsKey(z))
            {
                // var line = (GameObject) Instantiate(
                //     linePrefabs[Random.Range(0, linePrefabs.Length)],
                //     new Vector3(0, 0, z),
                //     Quaternion.Euler(0, 0, 0)  );
                
                var randTag = _tags[Random.Range(0, _tags.Count)];
                
                var line = Pooler.GetFromPool(randTag, new Vector3(0, 0, z),
                    Quaternion.Euler(0, 0, 0));
              

                lines.Add(z, line);
                _poolingObject.Enqueue(new KeyValuePair<string, GameObject>(randTag, line));


            }
            
        }

        // Remove lines based on player position.
        foreach (var line in new List<GameObject>(lines.Values))
        {
            var lineZ = line.transform.position.z;
            if (lineZ < playerZ - lineBehind)
            {
                lines.Remove((int) lineZ);
                Pooler.ReturnToPool(_poolingObject.Peek().Key, _poolingObject.Peek().Value);
                _poolingObject.Dequeue();
            }
        }
    }

    public void Reset()
    {
        // TODO This kind of reset is dirty, refactor might be needed.
        if (lines != null)
        {
            foreach (var line in new List<GameObject>(lines.Values))
            {
                Destroy(line);
            }

            Start();
        }
    }
}