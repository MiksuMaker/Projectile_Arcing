using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] Rigidbody rb;
    [SerializeField] bool rigibBodyON = true;

    bool hit = false;
    [SerializeField] GameObject indicator;
    bool drawIndicator = true;
    Vector3 indicatorPos;
    float projectileLifetime = 10f;
    #endregion  


    #region BUILTIN
    private void Start()
    {
        //GameObject _indicator = Instantiate(indicator) as GameObject;
        //indicator = _indicator;
    }
    private void Update()
    {
        if (rigibBodyON)
        {
            HandleRigidHit();
        }
        else
        {

        }
    }
    #endregion


    #region CUSTOM
    #region RIGIDBODY
    private void HandleRigidHit()
    {
        if (!hit)
        {
            // TURN
            transform.forward = rb.velocity;
        }

        if (hit)
        {
            // Stop drawing Indicator
            drawIndicator = false;
            //Debug.Log("DrawIndicator Status: " + drawIndicator);

            projectileLifetime -= Time.deltaTime;
            if (projectileLifetime < 0)
            {
                //Destroy(indicator);
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hit)
        {
            // Stick to the Collider
            transform.parent = collision.collider.transform;

            //Reset Scale   # HOW TO CHANGE SCALE #
            //gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);


            hit = true;
            Destroy(rb);
            Destroy(GetComponent<BoxCollider>());
        }
    }

    public void MoveRigidbody(Vector3 speed, Vector3 targetPos)
    {
        rb.velocity = speed;
        indicatorPos = targetPos;


    }
    #endregion

    private void OnDrawGizmos()
    {
        if (drawIndicator)
        {
            //Debug.Log("This is not supposed to fire");
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(indicatorPos, 0.5f);
        }
    }
    #endregion
}