using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Floater>())
        {
            other.gameObject.GetComponent<Floater>().SetIsInWater(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Floater>())
        {
            other.gameObject.GetComponent<Floater>().SetIsInWater(false);
        }
    }
}
