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
    public GameObject leftArm;
    public GameObject rightArm;
    public GameObject upperBody;
    public GameObject lowerBody;
    public GameObject intact;

    Rigidbody larb;
    Rigidbody rarb;
    Rigidbody ubrb;
    Rigidbody lbrb;
    public float hoverHeight = 0.75f;
    public float hoverHeightTolerance = 0.1f;
    public float moveSpeed = 10f;
    public float turnSpeed = 60f;
    public float flipSpeed = 90f;
    public float enemyWidth = 0.75f;
    public int damage = 10;
    public int health = 3;
    public float noiseScale = 0.05f;
    public Vector2 attackNoiseScale = new Vector2(0.2f, 0.2f);
    float noiseTimer;
    float originalY;
    public float attackTimer;
    public float attackTime = 1f;
    float stunTime = .5f;
    float stunTimer;
    public float dirDetectionDelta = 0.1f;

    public bool botKilled;

    Vector3 targetVector;

    Vector3 prevPos;
    Vector3 targetPos;
    Quaternion prevRot;
    Quaternion targetRot;

    Rigidbody rb;

    Transform player;


    void Awake() {
        originalY = intact.transform.localPosition.y;
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("VRCamera").transform;
        //player = GameObject.Find("FollowHead").transform;
        stunTimer = stunTime;
        attackTimer = attackTime;
        gm = FindObjectOfType<GameManager>();
        larb = leftArm.GetComponent<Rigidbody>();
        rarb = rightArm.GetComponent<Rigidbody>();
        ubrb = upperBody.GetComponent<Rigidbody>();
        lbrb = lowerBody.GetComponent<Rigidbody>();
    }

    private void Start() {
        Enemy.PlayOneShot(SpawnSound, 0.4f);
    }

    void FixedUpdate() {

        if(!Stunned() && !gm.hasGameEnded) {

            if(CheckOrientation()) {
                botmode = BotMode.Flipping;
                Flip();
            } else if(CheckHoverHeight()) {
                botmode = BotMode.AdjustingHoverHeight;
                AdjustHoverHeight();
            } else if(CheckDirection()) {
                botmode = BotMode.Turning;
                Turn();
            } else if(CheckAttackDistance()) {
                if(botmode != BotMode.Hunting) {
                    noiseTimer = 0;
                }
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
            UnStun();
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
        noiseTimer += Time.deltaTime;
        var heightShift = Mathf.PerlinNoise(0, noiseTimer);
        intact.transform.localPosition = Vector3.up * (heightShift * noiseScale + originalY);
        targetPos = transform.position + targetVector.normalized * moveSpeed * Time.deltaTime;
        rb.MovePosition(targetPos);
    }

    void UnStun() {
        rb.angularVelocity = Vector3.zero;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        botmode = BotMode.Flipping;
    }

    void Attack() {
        attackTimer -= Time.deltaTime;
        var t = attackTime - attackTimer;
        //var heightShift = Mathf.PerlinNoise(0, t);
        var sideShift = Mathf.PerlinNoise(0, t);
        intact.transform.localPosition = Vector3.right * sideShift * attackNoiseScale.x + 
            Vector3.up * (Mathf.Sin(t * 6) * attackNoiseScale.y + originalY);
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
        }
        return false;
    }

    bool CheckDirection() {
        targetRot = Quaternion.LookRotation(Vector3.ProjectOnPlane((player.transform.position - transform.position), Vector3.up), Vector3.up);
        return Vector3.Distance(transform.forward, Vector3.Normalize(Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up))) > dirDetectionDelta;
    }

    bool CheckAttackDistance() {
        targetVector = Vector3.ProjectOnPlane(player.position - transform.position, Vector3.up);
        return targetVector.magnitude > enemyWidth;
    }

    void BreakBot() {
        leftArm.SetActive(true);
        larb.isKinematic = false;
        leftArm.transform.SetParent(null);
        Destroy(leftArm, 3f);

        rightArm.SetActive(true);
        rarb.isKinematic = false;
        rightArm.transform.SetParent(null);
        Destroy(rightArm, 3f);

        upperBody.SetActive(true);
        ubrb.isKinematic = false;
        upperBody.transform.SetParent(null);
        Destroy(upperBody, 3f);

        lowerBody.SetActive(true);
        lbrb.isKinematic = false;
        lowerBody.transform.SetParent(null);
        Destroy(lowerBody, 3f);

        intact.SetActive(false);
    }

    public void BotHit() {
        if(!botKilled && !Stunned()) {
            stunTimer = stunTime;
            rb.useGravity = true;
            botmode = BotMode.Stunned;
            health -= 1;
            Enemy.PlayOneShot(HurtSound);

            if(health < 1) {
                botKilled = true;
                gm.SetHealth(1);
                gm.enemiesKilled++;
                gm.SetKillText();
                if(gm.IsLastRound() && gm.enemiesSpawned == gm.enemiesKilled) {
                    UnStun();
                    attackTimer = Mathf.Infinity;
                } else {
                    BreakBot();
                    gm.resetTimer = gm.resetTime;
                    Destroy(gameObject, .75f);
                }
            }
        }
    }
}