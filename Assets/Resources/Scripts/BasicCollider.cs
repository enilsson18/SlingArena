using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCollider : MonoBehaviour
{
    public bool colPlayer;

    // Start is called before the first frame update
    public void Start()
    {
        colPlayer = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            colPlayer = true;
        }
    }
}
