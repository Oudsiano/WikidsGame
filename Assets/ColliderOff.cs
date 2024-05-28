using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderOff : MonoBehaviour
{
    Collider collider;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ColliderOffed()
    {
        _ = collider.isTrigger;
    }
}
