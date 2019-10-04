using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsWithRotation : MonoBehaviour
{
    public Transform target;
    public float maxRotation = 60f;
    public float remainingAngle;

    void Update()
    {
        Vector3 offset = target.position - transform.position;
        Vector3 moveBy = offset.normalized * Time.deltaTime;

        Quaternion lookRotation = Quaternion.LookRotation(offset);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, maxRotation * Time.deltaTime);

        remainingAngle = Quaternion.Angle(transform.rotation, lookRotation);
        if (remainingAngle < 30) {
            transform.position = Vector3.MoveTowards(transform.position, target.position, Mathf.Min(Time.deltaTime, offset.magnitude));
        }
    }
}
