using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaussianRandomizer : MonoBehaviour
{
    [SerializeField] float MIN;
    [SerializeField] float MAX;
    [Range(1, 100)]
    [SerializeField] int GaussLoops = 1;
    [Space]
    [SerializeField] bool spam = true;
    [SerializeField] bool testForDebugValues = true;
    [SerializeField] bool givePercentage = true;
    [SerializeField] float debugLowerLimit;
    [SerializeField] float debugHigherLimit;

    float countAll = 0;
    float testedCount = 0;

    void Update()
    {
        countAll++;

        // Test the Gauss Randomizer
        float number = MyRandomizer.GaussianRandomizer(MIN, MAX, GaussLoops);

        if (spam)
        {
            Debug.Log("Spam: " + number);
        }


        if (testForDebugValues)
        {
            if (number > debugLowerLimit && number < debugHigherLimit)
            {
                testedCount++;

                Debug.Log("Success: " + number);
            }
        }

        if (givePercentage)
        {
            float percentage = testedCount / countAll * 100f;

            Debug.Log("Percentage: " + Mathf.RoundToInt(percentage));
            Debug.Log("Tested: " + testedCount + " All: " + countAll);
        }
    }

}

public class MyRandomizer
{
    static public float GaussianRandomizer(float minRange, float maxRange, int gaussLoops)
    {
        float sum = 0f;

        for (int i = 0; i < gaussLoops; i++)
        {
            sum += Random.Range(minRange, maxRange);
        }

        float value = sum / gaussLoops;
        return value;
    }
}

#region OLD STUFF
//private Vector3 ModifyAccuracy(Vector3 targetPos, float distance, float inaccuracyMod)
//{
//    // Getting the amount of Inaccuracy
//    distance = distance - 10f;
//    distance = Mathf.Max(distance, 1f);
//    float inaccuracy = distance / 2f * inaccuracyMod;

//    // Randomizing the direction of inaccuracy  # feat. GAUS RANDOMIZATION #
//    float maxInaccuracy = 5f;
//    float minRange = -maxInaccuracy; // -3f;

//    #region GAUSS Randomization
//    float xMod = ((Random.Range(minRange, maxInaccuracy)
//                    + Random.Range(minRange, maxInaccuracy)                       // Center it at ZERO
//                    + Random.Range(minRange, maxInaccuracy)
//                    + Random.Range(minRange, maxInaccuracy)
//                        + Random.Range(minRange, maxInaccuracy)) / 5f) /*- (maxInaccuracy)*/;

//    float zMod = ((Random.Range(minRange, maxInaccuracy)
//                    + Random.Range(minRange, maxInaccuracy)                       // Center it at ZERO
//                    + Random.Range(minRange, maxInaccuracy)
//                    + Random.Range(minRange, maxInaccuracy)
//                        + Random.Range(minRange, maxInaccuracy)) / 5f) /*- (maxInaccuracy)*/;
//    #endregion

//    Vector3 direction = new Vector3(xMod, 0f, zMod);
//    //Debug.Log("Direction: " + direction);
//    Debug.Log("X: " + xMod + " Z: " + zMod);

//    // Calculating the final inaccuracy
//    Vector3 interpretedPos = targetPos + direction * inaccuracy;
//    //Debug.Log("Distance From TargetPos: " + (interpretedPos - targetPos).magnitude);

//    return interpretedPos;
//}
#endregion
