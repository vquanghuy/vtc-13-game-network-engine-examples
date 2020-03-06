using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTankShoot : MonoBehaviour
{
    public GameObject shellPrefab;
    public float shellSpeed = 30;

    Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) {
            Shoot();
        }
    }

    void Shoot() {
        Vector3 shellVelocity = transform.forward * shellSpeed;
        
        GameObject shellObject = GameObject.Instantiate(shellPrefab,
                                    transform.position + new Vector3(0, 1.6f, 0), 
                                    transform.rotation);

        shellObject.GetComponent<Rigidbody>().velocity = shellVelocity;
    }
}
