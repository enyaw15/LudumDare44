using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grave : MonoBehaviour
{

    public static List<Grave> allGraves;
    // Start is called before the first frame update
    void Start()
    {
        if(allGraves == null)
        {
            allGraves = new List<Grave>();
        }
        allGraves.Add(this);
    }

    public void useGrave()
    {
        allGraves.Remove(this);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
