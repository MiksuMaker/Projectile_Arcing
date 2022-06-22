using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker2 : MonoBehaviour
{
    #region VARIABLES
    // TARGET
    [SerializeField] Transform targetT;
    Target targetScript;
    Vector3 predictedTargetPos;

    [SerializeField] GameObject arrowPrefab;


    [Header("Projectile Stats")]
    [SerializeField] float arrowSpeed = 10f;
    [SerializeField] float arcHeight = 5f;  // 5f; without Modifier
    private float curArrowSpeed;
    private float curArcHeight;

    [Header("Attacker Stats")]
    [SerializeField] float attackRange = 10f;
    [SerializeField] float attackDelay = 2f;
    [SerializeField] float gravity = -Physics.gravity.y;

    private float curAttackDelay = 0f;

    [Header("Rotation")]
    [SerializeField] GameObject turret;
    [SerializeField] GameObject gun;

    [Header("Fire Control")]
    [SerializeField] Transform shootingPos;
    [SerializeField] bool testFire = false;
    bool firingInProcess = false;

    // MODIFIERS
    [Range(0.1f,1.5f)]
    [SerializeField] float speedModifier = 0.1f;
    [Range(0.1f,1.5f)]
    [SerializeField] float heightModifier = 0.9f;
    [Range(0.5f,4f)]
    [SerializeField] float maxHeightModifier = 2f;
    #endregion  


    #region BUILTIN
    private void Start()
    {
        targetScript = targetT.gameObject.GetComponent<Target>();

        shootingPos = transform;
    }

    private void Update()
    {
        HandleRotation();

        HandleShooting();

        HandleTimeScale();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    #endregion

    #region CUSTOM
    private void HandleTimeScale()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = 1f;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = 0.5f;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Time.timeScale = 0.25f;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Time.timeScale = 0.1f;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Time.timeScale = 2f;
        }
    }

    #region SHOOTING
    private void HandleShooting()
    {
        // IF firing, start and keep up coroutine
        if (testFire)
        {
            if (!firingInProcess)
            {
                firingInProcess = true;
                StartCoroutine(Firing_Coroutine());
            }
        }
        else
        {
            if (firingInProcess)
            {
                firingInProcess = false;
                StopCoroutine(Firing_Coroutine());
            }
        }

        // IF not, stop and keep it off
    }

    IEnumerator Firing_Coroutine()
    {
        while (true && testFire)
        {
            // SHOOT
            Shoot();

            // WAIT FOR DELAY
            yield return new WaitForSeconds(attackDelay);
        }
    }


    private void Shoot()
    {
        GameObject arrow = Instantiate(arrowPrefab, shootingPos) as GameObject;


        // ADJUST FIRE

        // Get Distance
        float distance = (transform.position - targetT.position).magnitude;

        // Adjust the CURRENT SPEED and ARC-HEIGHT according to Magnitude

        // HEIGHT
        //curArcHeight = arcHeight - (distance * heightModifier);   // v1
        curArcHeight = arcHeight - (distance * heightModifier) + (distance * heightModifier * heightModifier / 2f);
        curArcHeight = Mathf.Max(maxHeightModifier, curArcHeight);  // Capping the Height
        Debug.Log("Height: " + curArcHeight);
        
        // SPEED
        //curArrowSpeed = arrowSpeed + (distance * speedModifier);  // v1
        //curArrowSpeed = (arrowSpeed + distance) / 2f;             // v2
        curArrowSpeed = (arrowSpeed + distance - (curArcHeight * 0.5f)) / 2f;   // v3




        float time = 0f;
        //Vector3 hitPoint = GetHitPoint(targetT.position,        // ORIGINAL
        //                               targetScript.velocity,
        //                               transform.position,
        //                               arrowSpeed,
        //                               out time);

        Vector3 hitPoint = GetHitPoint(targetT.position,        // ADJUSTED
                                       targetScript.velocity,
                                       transform.position,
                                       curArrowSpeed,
                                       out time);

        // This is for Cannon Rotation
        predictedTargetPos = hitPoint;

        //arrow.GetComponent<Arrow3>().Launch(hitPoint, arrowSpeed, arcHeight); // ORIGINAL
                                            //        //          // 5f;
        arrow.GetComponent<Arrow3>().Launch(hitPoint, curArrowSpeed, curArcHeight);
    }

    private Vector3 GetHitPoint(Vector3 targetPos, Vector3 targetVelocity, Vector3 attackerPos, float bulletSpeed, out float time)
    {
        Vector3 q = targetPos - attackerPos;
        //Ignoring Y for now. Add gravity compensation later, for more simple formula and clean game design around it
        q.y = 0f;
        targetVelocity.y = 0f;

        //solving quadratic ecuation from t*t(Vx*Vx + Vy*Vy - S*S) + 2*t*(Vx*Qx)(Vy*Qy) + Qx*Qx + Qy*Qy = 0

        float a = Vector3.Dot(targetVelocity, targetVelocity) - (bulletSpeed * bulletSpeed); //Dot is basicly (targetSpeed.x * targetSpeed.x) + (targetSpeed.y * targetSpeed.y)
        float b = 2 * Vector3.Dot(targetVelocity, q); //Dot is basicly (targetSpeed.x * q.x) + (targetSpeed.y * q.y)
        float c = Vector3.Dot(q, q); //Dot is basicly (q.x * q.x) + (q.y * q.y)

        //Discriminant
        float D = Mathf.Sqrt((b * b) - 4 * a * c);

        float root1 = (-b + D) / (2 * a);
        float root2 = (-b - D) / (2 * a);

        //Debug.Log("t1: " + t1 + " t2: " + t2);

        time = Mathf.Max(root1, root2);

        Vector3 ret = targetPos + targetVelocity * time;
        return ret;
    }
    #endregion


    #region ROTATION
    private void HandleRotation()
    {
        RotateTurret();
        PitchTurretGun();
    }

    private void RotateTurret()
    {
        // Point the turret into correct direction
        Vector3 pointDirection = predictedTargetPos - transform.position;
        Vector3 newDirection = Vector3.Lerp(turret.transform.forward, pointDirection, 0.1f * Time.deltaTime);

        turret.transform.rotation = Quaternion.LookRotation(newDirection);
    }

    private void PitchTurretGun()
    {

    }
    #endregion


    #endregion
}
