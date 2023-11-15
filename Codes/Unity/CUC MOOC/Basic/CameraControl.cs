using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour{
    public Transform target;

    void Start () {

    }

    void Update () {
        transform.lookAt(target);
    }
}