using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] float speed = 3f;
    Camera cam;
    [SerializeField] Transform destination;

    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public Transform enemyAim;
    #endregion  

    #region BUILTIN
    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        HandleMovement();
    }
    #endregion


    #region CUSTOM
    private void HandleMovement()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 worldMousePoint = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane));

            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            bool rez = Physics.Raycast(ray, out hit, Mathf.Infinity);

            if (rez)
            {
                destination.position = hit.point;
            }
        }

        Vector3 moveDistance = destination.position - transform.position;
        moveDistance.y = 0f;

        if (moveDistance.magnitude > 0.5f)
        {
            velocity = moveDistance.normalized * speed;
            MoveTarget(velocity);
        }
        else
        {
            velocity = Vector3.zero;
        }
    }

    private void MoveTarget(Vector3 _velocity)
    {
        // MOVE
        transform.position += _velocity * Time.deltaTime;
        // TURN
        transform.forward = _velocity;
    }
    #endregion
}