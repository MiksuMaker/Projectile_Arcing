using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAttacker : MonoBehaviour
{
    Attacker2 a2;
    Attacker3 a3;
    [SerializeField] bool attacker2IsOn = true;

    [Header("Target")]
    Transform predictedPos;
    Transform stubTurret;

    [Header("Fire Control")]
    [SerializeField] float fireAngleLimit = 0.707f;

    [SerializeField] float minDistance = 10f;
    [SerializeField] float maxDistance = 40f;

    [SerializeField] bool allowedToFire = true;

    bool withinDistance;
    bool withingAngle;

    void Start()
    {
        // Determine the Script
        if (attacker2IsOn)
        {
            a2 = GetComponent<Attacker2>();
            stubTurret = a2.transform.Find("TURRET");
        }
        else
        {
            a3 = GetComponent<Attacker3>();
            stubTurret = a3.transform.Find("TURRET");
        }
    }

    void Update()
    {
        CheckShootingPermission();
        UpdateShootingPermission();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color32(150, 50, 100, 200);
        Gizmos.DrawWireSphere(transform.position, minDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    private void UpdateShootingPermission()
    {
        if (withinDistance && withingAngle)
        {
            if (attacker2IsOn)
            {
                a2.allowedToShoot = true;
            }
            else
            {
                a3.allowedToShoot = true;
            }
        }
        else
        {
            if (attacker2IsOn)
            {
                a2.allowedToShoot = false;
            }
            else
            {
                a3.allowedToShoot = false;
            }
        }
    }

    private void CheckShootingPermission()
    {
        CheckShootingAngle();
        CheckShootingDistance();
    }

    private void CheckShootingAngle()
    {
        Vector3 dirToTarget;
        if (attacker2IsOn) { dirToTarget = Vector3.Normalize(a2.predictedTargetPos - transform.position); }
        else               { dirToTarget = Vector3.Normalize(a3.predictedTargetPos - transform.position); }

        var Dot = Vector3.Dot(stubTurret.forward, dirToTarget);

        // Check if the Target is within firing angle
        if (Dot > fireAngleLimit)
        {
            withingAngle = true;
        }
        else
        {
            withingAngle = false;
        }
    }

    private void CheckShootingDistance()
    {
        float distance;
        if (attacker2IsOn) { distance = Vector3.Magnitude(a2.predictedTargetPos - transform.position); }
        else               { distance = Vector3.Magnitude(a3.predictedTargetPos - transform.position); }

        if (distance < maxDistance && distance > minDistance)
        {
            withinDistance = true;
        }
        else
        {
            withinDistance = false;
        }

    }
}
