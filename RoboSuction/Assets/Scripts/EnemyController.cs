using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BotMode { AdjustingHoverHeight, Attacking, Flipping, Hunting, Stunned, Turning };

public class EnemyController : MonoBehaviour {

    public BotMode botmode;
    public float hoverHeight = 0.75f;
    public float hoverHeightTolerance = 0.1f;
    public float moveSpeed = 10f;
    public float turnSpeed = 60f;
    public float flipSpeed = 90f;
    public float enemyWidth = 0.75f;
    int health = 5;

    public float stunDist = 0.1f;
    float stunTime = 2f;
    public float stunTimer;
    public float dirDetectionDelta = 0.1f;

    Vector3 targetVector;

    Vector3 prevPos;
    Vector3 targetPos;
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
        stunTimer = stunTime;
    }

    void FixedUpdate() {

        if(!Stunned()) {
            if(CheckOrientation()) {
                botmode = BotMode.Flipping;
                Flip();
                return;
            }
            if(CheckHoverHeight()) {
                botmode = BotMode.AdjustingHoverHeight;
                AdjustHoverHeight();
                return;
            }
            if(CheckDirection()) {
                botmode = BotMode.Turning;
                Turn();
                return;
            }
            if(CheckAttackDistance()) {
                botmode = BotMode.Hunting;
                Hunt();
            } else {
                botmode = BotMode.Attacking;
                Attack();
            }
        }
    }

    bool Stunned() {
        stunTimer -= Time.deltaTime;

        if(stunTimer < 0) {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.isKinematic = false;
            rb.useGravity = false;
            botmode = BotMode.Flipping;
            return false;
        } else {
            return true;
        }
        //var hasMoved = Vector3.Distance(prevPos, transform.position);
        //var hasRotated = 

        //if(hasMoved > stunDist) {
        //    print("I am still stunned!");
        //} else {
        //    botmode = BotMode.Flipping;
        //}

        //prevPos = transform.position;
        //prevRot = transform.rotation;
    }

    void Flip() {
        targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, Vector3.up), Vector3.up);
        rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRot, flipSpeed * Time.deltaTime);
        if(Vector3.Distance(transform.up, Vector3.up) < dirDetectionDelta) {
            rb.rotation = targetRot;
        }
    }

    void AdjustHoverHeight() {
        print("Adjusting hover height");
        targetPos = transform.position + Vector3.up * Time.deltaTime * (transform.position.y < hoverHeight ? 1:-1);
        rb.MovePosition(targetPos);
    }

    void Turn() {
        print("Turning towards my nemesis!");
        rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRot, turnSpeed * Time.deltaTime);
    }

    void Hunt() {
        print("Hunt mode, ON!");
        targetPos = transform.position + targetVector.normalized * moveSpeed * Time.deltaTime;
        rb.MovePosition(targetPos);
    }

    void Attack() {
        print("I am attacking now!");
    }

    bool CheckOrientation() {
        return Vector3.Angle(transform.up, Vector3.up) > Mathf.Epsilon;
    }

    bool CheckHoverHeight() {
        if(transform.position.y < hoverHeight - hoverHeightTolerance || transform.position.y > hoverHeight + hoverHeightTolerance) {
            print("Hover height should be adjusted");
            return true;
        } else {
            print("Hover height fine");
            return false;
        }
    }

    bool CheckDirection() {
        targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane((player.transform.position - transform.position), Vector3.up), Vector3.up);
        return Vector3.Distance(transform.forward, Vector3.Normalize(Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up))) > dirDetectionDelta;
    }

    bool CheckAttackDistance() {
        targetVector = Vector3.ProjectOnPlane(player.position - transform.position, Vector3.up);
        return targetVector.magnitude > enemyWidth;
    }



    private void OnTriggerEnter(Collider other) {

        if(other.gameObject.layer == batLayer) {
            rb.useGravity = true;
            botmode = BotMode.Stunned;
            health -= 1;
            print("Got hit!");
            if(health < 1)
                Destroy(gameObject, 2f);
        }
    }

    private void OnCollisionEnter(Collision collision) {

    }
}