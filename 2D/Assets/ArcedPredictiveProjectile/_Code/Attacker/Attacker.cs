using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] Transform targetT;
    Target targetScript;

    [SerializeField] GameObject arrowPrefab;


    [Header("Projectile Stats")]
    [SerializeField] float arrowSpeed = 10f;

    [Header("Attacker Stats")]
    [SerializeField] float attackRange = 10f;
    [SerializeField] float attackDelay = 2f;
    [SerializeField] float gravity = -Physics.gravity.y;

    private float curAttackDelay = 0f;

    [Header("Coroutine Shooting")]
    [SerializeField] LayerMask collidableLayers;
    #endregion  


    #region BUILTIN
    private void Start()
    {
        targetScript = targetT.gameObject.GetComponent<Target>();
    }

    private void Update()
    {
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

    #region BASIC SHOOTING
    private void HandleShooting()
    {
        curAttackDelay -= Time.deltaTime;
        Vector3 aim = targetT.position - transform.position;
        aim.y = 0;
        transform.forward = aim;
        if (curAttackDelay < 0 && aim.magnitude < attackRange)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // Reset AttackDelay
        curAttackDelay = attackDelay;

        // Instantiate Arrow
        GameObject arrow = Instantiate(arrowPrefab) as GameObject;
        arrow.transform.position = transform.position;
        float time = 0f;
        Vector3 hitPoint = GetHitPoint(targetT.position, targetScript.velocity, transform.position, arrowSpeed, out time);
        hitPoint.y += 1f;
        Vector3 aim = hitPoint - transform.position;

        //

        float antiGravity = gravity * time / 2f;
        float deltaY = (hitPoint.y - arrow.transform.position.y) / time;

        Vector3 projectileSpeed = aim.normalized * arrowSpeed;
        projectileSpeed.y = antiGravity + deltaY;

        

        arrow.GetComponent<Arrow>().MoveRigidbody(projectileSpeed, hitPoint);
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


    #endregion
}
