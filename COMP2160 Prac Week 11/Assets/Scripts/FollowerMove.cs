using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerMove : MonoBehaviour
{
    // targets: marble, crosshair or targetUI
    [SerializeField] private Transform targetOne; 
    [SerializeField] private Transform targetTwo;

    [SerializeField] private bool followOneTarget = true;

    //Restrict between 0 and 100% of the distance between t1 and t2
    [Range(0, 100)]
    [SerializeField] private float percentage;

    void Update()
    {
        FollowTarget();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f); //See the gizmo clearly.
    }
    
    private void FollowTarget()
    {
        if (followOneTarget)
        {
            if (targetOne != null)
            {
                transform.position = targetOne.position;
            }
        }
        else
        {
            if (targetOne && targetTwo != null)
            {
                //Could add additions to set follower anywhere on the vector between targets
                //use lerp?
                //As it "finds a point some fraction of the way along a line between two endpoints" 

                float factor = percentage / 100f;
                Vector3 positionBetween = Vector3.Lerp(targetOne.position, targetTwo.position, factor);
                transform.position = positionBetween;

                //Vector3 midpoint = (targetOne.position + targetTwo.position) / 2;
                //transform.position = midpoint;
            }
        }
    }
}
