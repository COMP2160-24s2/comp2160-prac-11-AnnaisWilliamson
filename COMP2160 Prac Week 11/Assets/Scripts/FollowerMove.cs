using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerMove : MonoBehaviour
{
    [SerializeField] private Transform target; //marble

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
        } 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f); //See the gizmo clearly.
    }
}
