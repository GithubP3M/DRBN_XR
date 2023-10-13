using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomizer : MonoBehaviour
{
    public GameObject[] GOList;
    public int repeat;
    public float xmin = -1;
    public float xmax = 1;
    public float ymin = -1;
    public float ymax = 1;
    public float zmin = -1;
    public float zmax = 1;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject go in GOList)
        {
            for (int i =0; i<=repeat; i++)
            {
                var randomPosition = new Vector3(Random.Range(xmin,xmax), Random.Range(ymin,ymax), Random.Range(zmin,zmax));
                var randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                Instantiate(go, randomPosition, randomRotation);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
