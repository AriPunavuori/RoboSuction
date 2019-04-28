using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BotMode { Hunting, Stunned, Flipping, Turning, Attacking };

public class EnemyController : MonoBehaviour {

    public BotMode botmode;
    public float hoverHeight = 0.75f;
    public float moveSpeed = 10f;
    public float turnSpeed = 60f;
    public float flipSpeed = 90f;
    public float enemyWidth = 0.75f;
    public int health = 3;

    public float stunDist = 0.1f;
    public float dirDetectionDelta = 0.1f;


    Vector3 prevPos;
    Vector3 targetVector;
    Quaternion prevRot;
    Quaternion targetRot;

    Rigidbody rb;

    Transform player;

    int batLayer;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        batLayer = LayerMask.NameToLayer("Bat");
        //player = GameObject.Find("VRCamera").transform;
        player = GameObject.Find("FollowHead").transform;
        botmode = BotMode.Stunned;
    }

    void FixedUpdate() {



        if(botmode == BotMode.Hunting) {
            Hunt();
        }
        if(botmode == BotMode.Flipping) {
            Flip();
        }
        if(botmode == BotMode.Attacking) {
            Attack();
        }
        if(botmode == BotMode.Turning) {
            Turn();
        }
        if(botmode == BotMode.Stunned) {
            Stunned();
        }
    }

    void Stunned() {
        var hasMoved = Vector3.Distance(prevPos, transform.position);
        //var hasRotated = 

        if(hasMoved > stunDist) {
            print("I am still stunned!");
        } else {
            botmode = BotMode.Flipping;
        }

        prevPos = transform.position;
        prevRot = transform.rotation;
    }

    void Flip() {
        //if(Vector3.Distance(transform.forward, Vector3.up) > dirDetectionDelta) {
        //    targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.up, Vector3.up), Vector3.up);
        //} else if(Vector3.Distance(transform.forward, Vector3.down) > dirDetectionDelta) {
        //    targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(-transform.up, Vector3.up), Vector3.up);
        //} else {
            targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, Vector3.up), Vector3.up);
        //}
        rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRot, flipSpeed * Time.deltaTime);
        if(Vector3.Distance(transform.up, Vector3.up) < dirDetectionDelta) {
            rb.rotation = targetRot;
            botmode = BotMode.Turning;
            //rb.isKinematic = true;
            //rb.useGravity = false;
        }
    }

    void Turn() {
        targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane((player.transform.position - transform.position), Vector3.up), Vector3.up);
        if(transform.position.y < hoverHeight) {
            var targetPos = transform.position + Vector3.up * hoverHeight * moveSpeed * Time.deltaTime;
            rb.MovePosition(targetPos);
        } else if(Vector3.Distance(transform.forward, Vector3.Normalize(Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up))) > dirDetectionDelta) {
            rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRot, turnSpeed * Time.deltaTime);
        } else {
            botmode = BotMode.Hunting;
        }
    }

    void Hunt() {
        if(!CheckAttackDistance()) {
            var targetPos = transform.position + targetVector.normalized * moveSpeed * Time.deltaTime;
            rb.MovePosition(targetPos);
        } else {
            botmode = BotMode.Attacking;
        }
    }

    void Attack() {
        if(CheckAttackDistance()) {
            print("I am attacking now!");
        } else {
            botmode = BotMode.Hunting;
        }
    }

    bool CheckAttackDistance() {
        targetVector = Vector3.ProjectOnPlane(player.position - transform.position, Vector3.up);
        if(targetVector.magnitude > enemyWidth) {
            return false;
        } else {
            return true;
        }
    }

    //bool CheckDirection() {
    //    if() {
    //        return true;
    //    } else {
    //        return false;
    //    }
    //}

    private void OnTriggerEnter(Collider other) {

        if(other.gameObject.layer == batLayer) {
            //rb.isKinematic = false;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        //rb.useGravity = true;
        botmode = BotMode.Stunned;
        health -= 1;
        //if(health < 1)
            //Destroy(gameObject, 2f);
    }
}