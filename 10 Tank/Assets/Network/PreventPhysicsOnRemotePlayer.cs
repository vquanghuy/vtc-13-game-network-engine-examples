using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PreventPhysicsOnRemotePlayer : MonoBehaviour
{
    private Rigidbody rigidbody;
    bool isCollidingWithRemote = false;
    Vector3 position, velocity, angularVelocity;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!isCollidingWithRemote)
        {
            position = rigidbody.position;
            velocity = rigidbody.velocity;
            angularVelocity = rigidbody.angularVelocity;
        }
    }

    void LateUpdate()
    {
        if (isCollidingWithRemote)
        {
            rigidbody.position = position;
            rigidbody.velocity = velocity;
            rigidbody.angularVelocity = angularVelocity;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Remote Player")
        {
            isCollidingWithRemote = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Remote Player")
        {
            isCollidingWithRemote = false;
        }
    }
}
