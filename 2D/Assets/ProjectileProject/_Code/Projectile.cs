using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankShooter.Projectile
{
    public class Projectile : MonoBehaviour
    {
        #region Variables
        //[SerializeField] float _InitialVelocity;
        //[SerializeField] float _Angle;
        [Header("Line")]
        [SerializeField] LineRenderer _Line;
        [SerializeField] float _Step;
        [SerializeField] LayerMask collidableLayer;
        [Header("Fire Point")]
        [SerializeField] Transform _FirePoint;

        [Header("Gravity")]
        [SerializeField] float gravity = 18f; // Default: -Physics.gravity.y


        // Positions
        Vector3 nextPos;
        Vector3 oldPos;

        private Camera _cam;

        private void Start()
        {
            _cam = Camera.main;
        }


        private void Update()
        {
            // Raycast to find hitpoint
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, collidableLayer))
            {
                Vector3 direction = hit.point - _FirePoint.position;
                Vector3 groundDirection = new Vector3(direction.x, 0, direction.z);

                Vector3 targetPos = new Vector3(groundDirection.magnitude, direction.y, 0);

                // Create the variables for Calculation
                float height = targetPos.y + targetPos.magnitude / 2f;
                height = Mathf.Max(0.01f, height);  // Clamping the Height, it MUST be over 0
                float angle;
                float v0;
                float time;
                //CalculatePath(targetPos, angle, out v0, out time);
                CalculatePathWithHeight(targetPos, height, out v0, out angle, out time);

                // Draw the path
                //DrawPath(groundDirection.normalized, v0, angle, time, _Step);
                //DrawSmartPath(groundDirection.normalized, v0, angle, time, _Step);
                //DrawSmartPath(groundDirection.normalized, v0, angle, time, _Step);
                DrawSmarterPath(groundDirection.normalized, v0, angle, time, _Step);

                // Check for user input
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StopAllCoroutines();
                    StartCoroutine(Coroutine_Movement(groundDirection.normalized, v0, angle, time));
                }

            }


        }
        #endregion


        #region Line Renderer
        private void DrawPath(Vector3 direction, float v0, float angle, float time, float step)
        {
            step = Mathf.Max(0.01f, step);

            _Line.positionCount = (int)(time / step) + 2;
            int count = 0;

            for (float i = 0; i < time; i += step)
            {
                // For each POINT on the line at i time, calculate its position
                float x = v0 * i * Mathf.Cos(angle);
                float y = v0 * i * Mathf.Sin(angle) - 0.5f * gravity * Mathf.Pow(i, 2);
                _Line.SetPosition(count, _FirePoint.position + direction * x + Vector3.up * y);
                count++;
            }
            // Set up the last TWO points on the Line
            float xfinal = v0 * time * Mathf.Cos(angle);
            float yfinal = v0 * time * Mathf.Sin(angle) - 0.5f * gravity * Mathf.Pow(time, 2);
            _Line.SetPosition(count, _FirePoint.position + direction * xfinal + Vector3.up * yfinal);
        }

        private void DrawSmarterPath(Vector3 direction, float v0, float angle, float time, float step)
        {
            step = Mathf.Max(0.01f, step);

            _Line.positionCount = (int)(time / step) + 2;
            int count = 0;
            bool interruptedLine = false;

            for (float i = 0; i < time; i += step)
            {
                // For each POINT on the line at i time, calculate its position
                float x = v0 * i * Mathf.Cos(angle);
                float y = v0 * i * Mathf.Sin(angle) - 0.5f * gravity * Mathf.Pow(i, 2);

                Vector3 newPos = _FirePoint.position + direction * x + Vector3.up * y;

                _Line.SetPosition(count, newPos);
                count++;


                // If the line hits a collider, stop drawing
                if (Physics.OverlapSphere(newPos, 0.01f, collidableLayer).Length > 0)
                {
                    _Line.positionCount = count;
                    interruptedLine = true;
                    break;
                }
            }

            // Set up the last TWO points on the Line IF LINE IS NOT INTERRUPTED
            if (!interruptedLine)
            {
                float xfinal = v0 * time * Mathf.Cos(angle);
                float yfinal = v0 * time * Mathf.Sin(angle) - 0.5f * gravity * Mathf.Pow(time, 2);
                _Line.SetPosition(count, _FirePoint.position + direction * xfinal + Vector3.up * yfinal);
            }
        }

        private void DrawSmartPath(Vector3 direction, float v0, float angle, float time, float step)
        {
            step = Mathf.Max(0.01f, step);

            _Line.positionCount = (int)(time / step) + 2;
            int count = 0;

            for (float i = 0; i < time; i += step)
            {
                // For each POINT on the line at i time, calculate its position
                float x = v0 * i * Mathf.Cos(angle);
                float y = v0 * i * Mathf.Sin(angle) - 0.5f * gravity * Mathf.Pow(i, 2);


                Vector3 newPos = _FirePoint.position + direction * x + Vector3.up * y;


                //_Line.SetPosition(count, _FirePoint.position + direction * x + Vector3.up * y);
                _Line.SetPosition(count, newPos);

                count++;

                // If the line hits a collider, stop drawing
                if (Physics.OverlapSphere(newPos, 0.01f, collidableLayer).Length > 0)
                {
                    //_Line.positionCount = count;
                    //_Line.SetPosition(count, newPos);
                    i = time;
                    break;
                }

                //Debug.Log("TIME: " + time);

            }

            // Set up the last TWO points on the Line
            float xfinal = v0 * time * Mathf.Cos(angle);
            float yfinal = v0 * time * Mathf.Sin(angle) - 0.5f * gravity * Mathf.Pow(time, 2);
            _Line.SetPosition(count, _FirePoint.position + direction * xfinal + Vector3.up * yfinal);

            Debug.Log("FinalPOS X: " + xfinal + ", Y:  " + yfinal);

            #region First Attempt
            //step = Mathf.Max(0.01f, step);

            //_Line.positionCount = (int)(time / step) + 2; // How many points does the line have
            //float numPoints = _Line.positionCount;
            //List<Vector3> points = new List<Vector3>();

            //Vector3 startingPosition = _FirePoint.position;
            ////float startingVelocity = v0;
            //Vector3 startingVelocity = direction * v0;

            //// Draw the line
            //for (float t = 0; t < numPoints; t += step)
            //{
            //    Vector3 newPoint = startingPosition + t * startingVelocity;
            //    newPoint.y = startingPosition.y + startingVelocity.y * t + gravity / 2f * t * t;
            //    points.Add(newPoint);

            //    // If _Line hits a collider, stop it
            //    if (Physics.OverlapSphere(newPoint, 0.1f, collidableLayer).Length > 0)
            //    {
            //        _Line.positionCount = points.Count;
            //        break;
            //    }
            //}

            //_Line.SetPositions(points.ToArray());
            #endregion
        }

        private float QuadraticEquation(float a, float b, float c, float sign)
        {
            return (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
        }


        //              (~ 9:00 in video)
        private void CalculatePathWithHeight(Vector3 targetPos, float h, out float v0, out float angle, out float time)
        {
            float xt = targetPos.x;
            float yt = targetPos.y;
            float g = gravity;

            float b = Mathf.Sqrt(2 * g * h);
            float a = (-0.5f * g);
            float c = -yt;

            float tplus = QuadraticEquation(a, b, c, 1);
            float tmin = QuadraticEquation(a, b, c, -1);
            // BELOW: time = (is tplus bigger than tmin)?
            //                  -> If true, time = tplus
            //                  -> If untrue, time = tmin
            time = tplus > tmin ? tplus : tmin;

            angle = Mathf.Atan(b * time / xt);

            v0 = b / Mathf.Sin(angle);
        }


        // OUT KEYWORD
        //
        // Below, the keyword OUT is used with 'v0' & 'time'. It will use
        // those variables, but also change them
        //

        private void CalculatePath(Vector3 targetPos, float angle, out float v0, out float time)
        {
            float xt = targetPos.x;
            float yt = targetPos.y;
            float g = gravity;

            // Calculating Initial Velocity         (~ 6:00 in video https://www.youtube.com/watch?v=Qxs3GrhcZI8)
            float v1 = Mathf.Pow(xt, 2) * g;
            float v2 = 2 * xt * Mathf.Sin(angle) * Mathf.Cos(angle);
            float v3 = 2 * yt * Mathf.Pow(Mathf.Cos(angle), 2);
            v0 = Mathf.Sqrt(v1 / (v2 - v3));

            // -> Initial Velocity is needed to calculate the TIME it takes for proj. to travel

            time = xt / (v0 * Mathf.Cos(angle));
        }
        #endregion



        // Below is the formula used for calculating the trajectory of the projectile
        IEnumerator Coroutine_Movement(Vector3 direction, float v0, float angle, float time)
        {
            // While loop, that will go from 0 to 100 seconds
            float t = 0;
            while (t < time)
            {
                // MATH

                float x = v0 * t * Mathf.Cos(angle);
                float y = v0 * t * Mathf.Sin(angle) - 0.5f * gravity * Mathf.Pow(t, 2);

                // TESTING ###
                //oldPos = transform.position;
                // ### ### ###

                // Next, Set the position of the Object to the new XY coordinates
                //transform.position = _FirePoint.position + direction * x + Vector3.up * y;
                nextPos = _FirePoint.position + direction * x + Vector3.up * y;

                // Set Rotation right
                transform.LookAt(nextPos);
                // Move the projectile
                transform.position = nextPos;

                // TESTING ###
                //Vector3 difference = transform.position - oldPos;
                //Debug.Log("Magnitude: " + difference.magnitude * 100);
                // ### ### ###

                // Stop the projectile if it hits something

                // ---> THIS is so that if something MOVING collides,
                // it will finish the current trajectory right there
                if (Physics.OverlapSphere(transform.position, 0.01f, collidableLayer).Length > 0)
                {
                    // Stop
                    Debug.Log("STOPPING!");
                    break;
                }

                t += Time.deltaTime;
                yield return null;
            }
        }
    }
}
