using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow3 : MonoBehaviour
{
    #region VARIABLES
    [SerializeField]
    Vector3 targetPos;
    Vector3 startPos;

    // PROJECTILE
    float speed = 10f;

    // ARC
    float arcHeight = 1f;

    [SerializeField] LayerMask collidableLayers;


    // CALCULATIONS
    float timeElapsed = 0f;
    bool hasArrived = false;
    bool hasCollided = false;

    // RIGIDBODY
    Rigidbody rb;
    Vector3 cachedVelocity;
    Vector3 verifiedCachedVelocity;
    bool hasRB = false;

    // COLLISION
    [SerializeField]
    private float collisionRange = 0.05f;
    Vector3 lastPos;
    #endregion



    #region BUILTIN
    private void Start()
    {
        startPos = transform.position;
        timeElapsed = 0f;

        //Time.timeScale = 0.2f;
    }

    private void FixedUpdate()
    {
        if (!hasArrived)
        {
            Move();
        }

        timeElapsed += 1f * Time.deltaTime;

        // Glide as a RigidBody
        if (hasRB && !hasCollided)
        {
            Vector3 nextPos = (transform.position + rb.velocity);

            //Debug.Log("Velocity: " + (nextPos - transform.position) /** Time.deltaTime*/);
            transform.forward = rb.velocity;
            CheckCollisions();
        }
    }

    private void Update()
    {
        //timeElapsed += Time.deltaTime;
        //Debug.Log("COUNTING!");

    }


    #endregion

    #region CUSTOM METHODS
    public void Launch(Vector3 _targetPos, float _speed, float _arcHeight)
    {
        // Set up the variables
        speed = _speed;
        targetPos = _targetPos;
        arcHeight = _arcHeight;

        // Shoot 
        //Move();
    }

    private void Move()
    {
        // Establish Ground Directions
        Vector3 gStartPos = startPos;
        Vector3 gTargetPos = targetPos;
        gStartPos.y = 0f;
        gTargetPos.y = 0f;

        float gDist = (gTargetPos - gStartPos).magnitude;

        Vector3 groundPos = transform.position;
        groundPos.y = 0f;

        Vector3 nextGroundPos = Vector3.MoveTowards(groundPos, gTargetPos, speed * Time.deltaTime);

        float baseY = Mathf.Lerp(startPos.y, targetPos.y, (nextGroundPos - gStartPos).magnitude / gDist);

        float arc = -arcHeight * (nextGroundPos - gStartPos).magnitude * (nextGroundPos - gTargetPos).magnitude / (-0.25f * gDist * gDist);


        Vector3 nextPos = new Vector3(nextGroundPos.x, baseY + arc, nextGroundPos.z);

        cachedVelocity = (nextPos - transform.position) / Time.deltaTime;
        lastPos = transform.position; // For Collision Detection
        //Debug.Log("Velocity: " + cachedVelocity);


        // Rotate to face the next Position
        transform.LookAt(nextPos);
        // MOVE
        transform.position = nextPos;

        CheckCollisions();

        if (nextPos == targetPos)
        {
            Arrived();
        }
        else
        {
            // If the projectile doesn't stop yet, VERIFY the Velocity
            //
            //  --> Otherwise, the velocity will cut short
            verifiedCachedVelocity = cachedVelocity;
        }
    }

    private void CheckCollisions()
    {
        // FOR RAYCAST

        //Vector3 fromPos = transform.position;
        //Vector3 toPos = lastPos;

        //Vector3 direction = (fromPos - toPos).normalized;
        //float distance = (fromPos - toPos).magnitude;


        //Debug.DrawLine(fromPos, fromPos + Vector3.up * 3f, Color.red);
        //Debug.DrawRay(toPos, (direction * distance));
        //Debug.DrawLine(fromPos, (-transform.forward * collisionRange + fromPos)); // v2

        //RaycastHit hit;

        if (Physics.OverlapSphere(transform.position, collisionRange, collidableLayers).Length > 0)   // v1 OverlapSphere
                                                                                                      //if (Physics.Raycast(transform.position, -transform.forward, out hit, collisionRange, collidableLayers)) // v2 Raycast
            //if (Physics.Raycast(fromPos, -direction, out hit, distance, collidableLayers)) // v3
        {
            // Collides with something
            hasCollided = true;
            Arrived();

            //Remove possible RB
            if (hasRB)
            {
                //Debug.Log("RB DESTROYED!");
                Destroy(rb);
            }

            // Remove Collider too
            //Destroy(GetComponent<BoxCollider>());

            // STICK TO THE OBJECT

            // A) OVERLAPSPHERE VERSION
            transform.parent = Physics.OverlapSphere(transform.position, collisionRange, collidableLayers)[0].gameObject.transform; // v1 OverlapSphere

            // B) RAYCAST VERSION
            //Debug.Log("Ray Hit: " + hit.transform.name);
            //transform.parent = hit.transform;     // v2 Raycast

        }
    }

    private void Arrived()
    {
        hasArrived = true;

        if (!hasCollided)
        {
            ContinueAsRigidbody();
        }
        //Debug.Log("Total Time Elapsed: " + timeElapsed);
        //Debug.Log("Projectile has landed at: " + transform.position);
    }


    private void ContinueAsRigidbody()
    {
        hasRB = true;

        gameObject.AddComponent<Rigidbody>();
        rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = verifiedCachedVelocity;
        //Debug.Log("Velocity: " + cachedVelocity);
    }
    #endregion
}