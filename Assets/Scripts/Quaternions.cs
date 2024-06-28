using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quaternions : MonoBehaviour
{
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        //transform.LookAt(target);
        LookAtTarget();
    }

    // Update is called once per frame
    void Update()
    {
        LookAtTarget();
    }

    void LookAtTarget()
    {
        Vector3 offest = target.position - transform.position;
        offest.Normalize();
        Vector3 axis = Vector3.Cross(transform.forward, offest);
        axis.Normalize();
        float angle = Mathf.Acos(Vector3.Dot(transform.forward, offest));

        Quaternion q = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axis);
        transform.rotation *= q;
    }
}
