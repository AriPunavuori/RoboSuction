using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public float moveSpeed = 10f;
    public float hoverHeight = 0.75f;
    public float turnSpeed = 10f;
    public float flipSpeed = 90f;
    public float enemyWidth = 0.75f;
    public float stunDist = 0.1f;
    public float upDetection;
    public bool huntMode;
    public bool stunned;
    public int health = 3;
    public Transform player;
    Vector3 prevPos;
    Quaternion prevRot;
    Quaternion targetRot;

    int batLayer;

    Rigidbody rb;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        batLayer = LayerMask.NameToLayer("Bat");
        player = GameObject.Find("Player").transform;
    }

    void FixedUpdate() {
        if(huntMode) {
            Move();
        } else {
            if(Vector3.Distance(prevPos, transform.position) < stunDist && Quaternion.Angle(transform.rotation, prevRot) < 1f) {
                rb.useGravity = false;
                stunned = false;
            }
            if(!stunned) {
                Flip();
            }
            prevPos = transform.position;
            prevRot = transform.rotation;
        }
    }

    void Flip() {
        if(Vector3.Distance(transform.forward, Vector3.up) < upDetection) {
            targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.up, Vector3.up), Vector3.up);
        } else if (Vector3.Distance(transform.forward, Vector3.down) < upDetection) {
            targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(-transform.up, Vector3.up), Vector3.up);
        } else {
            targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, Vector3.up), Vector3.up);
        }
        rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRot, flipSpeed * Time.deltaTime);
        if(Vector3.Distance(transform.up, Vector3.up) < upDetection) {
            rb.rotation = targetRot;
            huntMode = true;
        }
    }

    void Move() {
        var targetVector = player.position - transform.position;
        if(targetVector.magnitude > enemyWidth) {
            var targetPos = transform.position + targetVector.normalized * moveSpeed * Time.deltaTime;
            rb.MovePosition(targetPos);
        }
    }

    private void OnTriggerEnter(Collider other) {

        if(other.gameObject.layer == batLayer) {
            huntMode = false;
            stunned = true;
            rb.useGravity = true;
            health -= 1;
            if(health < 1)
                Destroy(gameObject, 1f);
        }
    }
}