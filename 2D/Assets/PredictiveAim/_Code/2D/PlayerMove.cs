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



public class PlayerMove : MonoBehaviour
{
    #region Variables
    [Header("Move")]
    [SerializeField] bool doMove = true;
    [SerializeField] bool doRigidMove = false;
    Rigidbody2D rb;
    [Header("Speed")]
    [Range(0.01f, 10f)]
    public float speed = 10f;
    [Header("Direction")]
    [Range(-5f, 5f)]
    [SerializeField] float x = 1f;
    [Range(-5f, 5f)]
    [SerializeField] float y = 0.5f;

    #endregion

    #region Builtin Methods
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (doMove)
        {
            Move();
            //MoveWithRigidBody();

        }
    }

    private void FixedUpdate()
    {
        if (doRigidMove)
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
        transform.position = new Vector2(newX, newY);
    }

    private void MoveWithRigidBody()
    {
        float newX = transform.position.x + x * speed * Time.deltaTime;
        float newY = transform.position.y + y * speed * Time.deltaTime;
        var dir = new Vector2(newX, newY);
        //rb.AddForce(dir, ForceMode2D.Force);

        rb.MovePosition(dir);
    }

    #endregion
}
