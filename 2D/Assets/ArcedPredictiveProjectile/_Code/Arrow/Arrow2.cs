using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow2 : MonoBehaviour
{
    #region VARIABLES
    // TARGET
    [SerializeField] Transform targetT;

    [SerializeField]
    Vector3 targetPos;

    // PROJECTILE
    [SerializeField] float speed = 10f;

    // ARC
    [SerializeField]
    float arcHeight = 1f;

    Vector3 startPos;

    // CALCULATIONS
    float timeElapsed = 0f;
    bool hasArrived = false;
    #endregion



    #region BUILTIN
    private void Start()
    {
        startPos = transform.position;

        targetPos = targetT.position;

        Time.timeScale = 0.2f;
    }

    private void FixedUpdate()
    {
        if (!hasArrived)
        {
            Move();
        }

        timeElapsed = +1f / Time.deltaTime;
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

        Debug.Log("Velocity: " + (transform.position - nextPos).magnitude / Time.deltaTime);

        // Rotate to face the next Position
        transform.LookAt(nextPos);
        // MOVE
        transform.position = nextPos;


        if (nextPos == targetPos)
        {
            Arrived();
        }
    }

    private void Arrived()
    {
        hasArrived = true;
        Debug.Log("Total Time Elapsed: " + timeElapsed);
        //Debug.Log("Projectile has landed at: " + transform.position);
    }
    #endregion
}
