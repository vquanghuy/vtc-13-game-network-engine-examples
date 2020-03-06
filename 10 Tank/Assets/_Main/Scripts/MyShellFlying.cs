using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyShellFlying : MonoBehaviour
{
    public GameObject explosionPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col) {
        if (col.tag != "Player") {
            
            var exposionObject = GameObject.Instantiate(explosionPrefab, transform.position, transform.rotation);
            exposionObject.GetComponent<ParticleSystem>().Play();

            Destroy(gameObject);
            Destroy(exposionObject, 2.0f);
        }
    }
}
