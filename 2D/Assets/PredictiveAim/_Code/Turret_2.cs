using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_2 : MonoBehaviour
{
    #region Variables
    [SerializeField] Rigidbody2D projectile;
    [SerializeField] Rigidbody2D target;
    Vector2 prevPos;
    [Header("Projectile Stats")]
    [SerializeField] float projectileSpeed;
    [SerializeField] float delay = 0.1f;
    [SerializeField] float fireRate = 0.1f;
    #endregion

    #region Builtin Methods
    private void Start()
    {
        InvokeRepeating(nameof(Fire), .1f, .1f);
    }

    #endregion

    #region Custom Methods
    public void Fire()
    {
        var instance = Instantiate(projectile, transform.position, Quaternion.identity);

        if (InterceptionDirection(target.transform.position, transform.position, target.velocity, projectileSpeed, out var direction))
        {
            instance.velocity = direction * projectileSpeed;
        }
        else
        {
            instance.velocity = (target.transform.position - transform.position).normalized * projectileSpeed;
        }

        // Destroy the instances after a period of time
        Destroy(instance.gameObject, 5f);
    }

    public bool InterceptionDirection(Vector2 a, Vector2 b, Vector2 vA, float sB, out Vector2 result)
    {
        var aToB = b - a;
        var dC = aToB.magnitude;
        var alpha = Vector2.Angle(aToB, vA) * Mathf.Deg2Rad;
        var sA = vA.magnitude;
        var r = sA / sB;
        if (MyMath_2.SolveQuadratic(1 - r * r, 2 * r * dC * Mathf.Cos(alpha), -(dC * dC), out var root1, out var root2) == 0)
        {
            result = Vector2.zero;
            return false;
        }
        var dA = Mathf.Max(root1, root2);
        var t = dA / sB;
        var c = a + vA * t;
        result = (c - b).normalized;
        return true;
    }

    #endregion
}

public class MyMath_2
{
    public static int SolveQuadratic(float a, float b, float c, out float root1, out float root2)
    {
        var discriminant = b * b - 4 * a * c;
        if (discriminant < 0)
        {
            root1 = Mathf.Infinity;
            root2 = -root1;
            return 0;
        }

        root1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
        root2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
        return discriminant > 0 ? 2 : 1;

    }
}
