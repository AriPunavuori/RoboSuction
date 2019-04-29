using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patonki : MonoBehaviour {
    public Transform attachmentPoint;
    Rigidbody rb;
    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate() {
        rb.rotation = attachmentPoint.rotation;
        rb.position = attachmentPoint.position;
    }
}
