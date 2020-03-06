using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTankMovement : MonoBehaviour
{
    public float moveSpeed = 5;
    public float turnSpeed = 40;

    public AudioClip idleClip;
    public AudioClip drivingClip;

    Rigidbody rigidbody;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        EngineAudio();
    }

    void FixedUpdate() {
        Move();
        Turn();
    }

    void Move() {
        float v = Input.GetAxis("Vertical1");
        Vector3 offset = transform.forward * moveSpeed * Time.deltaTime * v;

        rigidbody.MovePosition(transform.position + offset);
    }

    void Turn() {
        float h = Input.GetAxis("Horizontal1");
        
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y += turnSpeed * Time.deltaTime * h;

        rigidbody.MoveRotation(Quaternion.Euler(currentRotation));
    }

    void EngineAudio() {
        if (Mathf.Abs(Input.GetAxis("Vertical1")) < 0.1f) {
            if (audioSource.clip == drivingClip) {
                audioSource.clip = idleClip;
                audioSource.Play();
            }
        } else {
            if (audioSource.clip == idleClip) {
                audioSource.clip = drivingClip;
                audioSource.Play();
            }
        }
    }
}
