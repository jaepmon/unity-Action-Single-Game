using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject target;
    public Vector3 cameraOffset;

    //void Start()
    //{
    //    cameraOffset = transform.position - target.transform.position;
    //}
    //void LateUpdate()
    //{
    //    transform.position = target.transform.position + cameraOffset;
    //}
    void LateUpdate()
    {
        transform.position = target.transform.position + cameraOffset;
    }
}
