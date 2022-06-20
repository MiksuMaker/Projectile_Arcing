using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret3D : MonoBehaviour
{
    #region Variables
    [SerializeField] Rigidbody projectile;
    [SerializeField] Rigidbody target;
    [SerializeField] Transform targetT;
    Vector3 prevPos;
    [Header("Projectile Stats")]
    [SerializeField] float projectileSpeed;
    [SerializeField] float delay = 0.1f;
    [SerializeField] float fireRate = 0.1f;

    Vector3 predictedPosition;

    #endregion

    #region Builtin Methods
    private void Start()
    {
        prevPos = targetT.position;
        predictedPosition = targetT.position;

        InvokeRepeating(nameof(Fire), delay, fireRate);
    }

    #endregion

    #region Custom Methods
    public void Fire()
    {
        //Debug.Log("POSITION: " + transform.position);
        var instance = Instantiate(projectile, transform.position, Quaternion.identity);
        ////  INSERT Interception
        //Debug.Log("--REAL--  Velocity: " + target.velocity);
        //Debug.Log("Simulated Velocity: " + TargetVelocity());
        if (InterceptionDirection(target.transform.position,
                                  transform.position,
                                  //target.velocity,
                                  TargetVelocity(),
                                  projectileSpeed,
                                  out var direction))
        {
            // Predictive Shot
            instance.velocity = direction * projectileSpeed;
            //Debug.Log("SUCCEEDED");
        }
        else
        {
            // Straight Shot
            instance.velocity = (target.transform.position - transform.position).normalized * projectileSpeed;
            Debug.Log("FAILED to Intercept");

        }

        prevPos = targetT.position;

        // Destroy the instances after a period of time
        Destroy(instance.gameObject, 5f);
    }

    public Vector3 TargetVelocity()
    {
        //float speed = targetGO.GetComponent<PlayerMove>().speed;

        Vector3 pos = targetT.position;
        Vector3 dir = pos - prevPos;

        Vector3 finalDir = dir * 10f;

        return finalDir;
        //return dirVel;
    }

    // BELOW EXPLAINED
    //
    // a = Position of Player
    // b = Position of Turret
    // 
    // vA = velocity of Player
    // sB = Speed of Projectile
    // 
    // result = gives the direction we need to shoot 
    //
    // 
    public bool InterceptionDirection(Vector3 a, Vector3 b, Vector3 vA, float sB, out Vector3 result)
    {
        var aToB = b - a;   // Direction from A to B
        var dC = aToB.magnitude;
        var alpha = Vector3.Angle(aToB, vA) * Mathf.Deg2Rad;
        var sA = vA.magnitude;
        var r = sA / sB;
        if (MyMath.SolveQuadratic(1 - r * r, 2 * r * dC * Mathf.Cos(alpha), -(dC * dC), out var root1, out var root2) == 0)
        {
            result = Vector3.zero;
            return false;
        }

        // Of those TWO Roots, one will be positive, and the another negative
        // --> We need to return the positive one

        var dA = Mathf.Max(root1, root2);
        var t = dA / sB;
        var c = a + vA * t;

        predictedPosition = c;  // THIS IS FOR OnDrawGizmos ONLY

        // INTERCEPTION DIRECTION
        result = (c - b).normalized;
        return true;

    }
    #endregion

    // COMMENT ON/OFF
    //
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.magenta;
    //    Gizmos.DrawSphere(predictedPosition, 0.5f);
    //}
}

//public class MyMath
//{
//    public static int SolveQuadratic(float a, float b, float c, out float root1, out float root2)
//    {
//        var discriminant = b * b - 4 * a * c;

//        // Check if there are any VALID answers for DISCRIMINANT
//        if (discriminant < 0)
//        {
//            root1 = Mathf.Infinity;
//            root2 = -root1;
//            return 0;
//        }

//        root1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
//        root2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);

//        // If DISCRIMINANT is greater than 0; --> choose ROOT2, else, choose ROOT1
//        return discriminant > 0 ? 2 : 1;
//    }
//}
