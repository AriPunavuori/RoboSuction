using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BotMode { AdjustingHoverHeight, Attacking, Flipping, Hunting, Stunned, Turning };

public class EnemyController : MonoBehaviour {

    public AudioClip AttackSound;
    public AudioClip HurtSound;
    public AudioClip SpawnSound;
    public AudioClip DieSound;
    public AudioSource Enemy;

    GameManager gm;
    public BotMode botmode;
    public float hoverHeight = 0.75f;
    public float hoverHeightTolerance = 0.1f;
    public float moveSpeed = 10f;
    public float turnSpeed = 60f;
    public float flipSpeed = 90f;
    public float enemyWidth = 0.75f;
    public int damage = 10;
    public int health = 3;

    public float attackTimer;
    public float attackTime = 1f;
    float stunTime = .5f;
    float stunTimer;
    public float dirDetectionDelta = 0.1f;

    bool botKilled;

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
        player = GameObject.Find("VRCamera").transform;
        //player = GameObject.Find("FollowHead").transform;
        stunTimer = stunTime;
        attackTimer = attackTime;
        gm = FindObjectOfType<GameManager>();
    }

    private void Start() {
        Enemy.PlayOneShot(SpawnSound, 0.01f);
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
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
            botmode = BotMode.Flipping;
            return false;
        } else {
            return true;
        }
    }

    void Flip() {
        targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, Vector3.up), Vector3.up);
        rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRot, flipSpeed * Time.deltaTime);
        if(Vector3.Angle(transform.up, Vector3.up) < 1) {
            rb.rotation = targetRot;
        }
    }

    void AdjustHoverHeight() {
        targetPos = transform.position + Vector3.up * Time.deltaTime * moveSpeed * (transform.position.y < hoverHeight ? 1 : -1);
        rb.MovePosition(targetPos);
    }

    void Turn() {
        rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRot, turnSpeed * Time.deltaTime);
    }

    void Hunt() {
        targetPos = transform.position + targetVector.normalized * moveSpeed * Time.deltaTime;
        rb.MovePosition(targetPos);
    }

    void Attack() {
        attackTimer -= Time.deltaTime;
        if(attackTimer < 0) {
            gm.SetHealth(-damage);
            attackTimer = attackTime;
            Enemy.PlayOneShot(AttackSound, 0.3f);
        }
    }

    bool CheckOrientation() {
        return Vector3.Angle(transform.up, Vector3.up) > 1;
    }

    bool CheckHoverHeight() {
        if(transform.position.y < hoverHeight - hoverHeightTolerance || transform.position.y > hoverHeight + hoverHeightTolerance) {
            return true;
        } else {
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
        if (!botKilled&&!Stunned())
        {
            if (other.gameObject.layer == batLayer)
            {
                stunTimer = stunTime;
                rb.useGravity = true;
                botmode = BotMode.Stunned;
                health -= 1;
                Enemy.PlayOneShot(HurtSound);
                if (health < 1)
                {
                    botKilled = true;
                    gm.SetHealth(1);
                    Enemy.PlayOneShot(DieSound, 0.4f);
                    
                    gm.enemiesKilled++;
                    gm.SetKillText();
                    //if(gm.waveNumber == gm.waveInfo.Length && gm.enemiesSpawned == gm.enemiesKilled-1) {
                    //    rb.velocity = Vector3.zero;
                    //    rb.freezeRotation = true;
                    //    rb.isKinematic = true;
                    //    stunTimer = Mathf.Infinity;
                    //    bot
                    //} else {
                        Destroy(gameObject, .5f);
                    //}
                }
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision) {

    }
}