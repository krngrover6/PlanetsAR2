using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    public GameObject PlanetObject;
    public Vector3 RotationVector;

    void Update()
    {
        if (PlanetObject == null) return;

        PlanetObject.transform.Rotate(RotationVector * Time.deltaTime);
    }
}