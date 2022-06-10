using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#region STORAGE
#region Variables

#endregion

#region Builtin Methods

#endregion

#region Custom Methods

#endregion
#endregion



public class Move3DPlayer : MonoBehaviour
{
    #region Variables
    [Header("Move")]
    [SerializeField] bool doMove = true;
    [SerializeField] bool doRigidMove = false;
    Rigidbody rb;
    [Header("Speed")]
    [Range(0.01f, 10f)]
    public float speed = 10f;
    [Header("Direction")]
    [Range(-5f, 5f)]
    [SerializeField] float x = 1f;
    [Range(-5f, 5f)]
    [SerializeField] float y = 0.5f;
    [Range(-5f, 5f)]
    [SerializeField] float z = 0.5f;

    #endregion

    #region Builtin Methods
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (doMove)
        {
            Move();

        }
    }

    private void FixedUpdate()
    {
        if (doRigidMove && rb)
        {
            MoveWithRigidBody();
        }
    }
    #endregion

    #region Custom Methods
    private void Move()
    {
        float newX = transform.position.x + x * speed * Time.deltaTime;
        float newY = transform.position.y + y * speed * Time.deltaTime;
        float newZ = transform.position.z + z * speed * Time.deltaTime;
        transform.position = new Vector3(newX, newY, newZ);
    }

    private void MoveWithRigidBody()
    {
        float newX = transform.position.x + x * speed * Time.deltaTime;
        float newY = transform.position.y + y * speed * Time.deltaTime;
        float newZ = transform.position.z + z * speed * Time.deltaTime;
        var dir = new Vector3(newX, newY, newZ);
        //rb.AddForce(dir, ForceMode2D.Force);

        rb.MovePosition(dir);
    }

    #endregion
}

