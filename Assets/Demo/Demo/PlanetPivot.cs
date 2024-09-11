using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanetPivot : MonoBehaviour
{
    public float rotationSpeed;

    void Start()
    {
        rotationSpeed += Random.value;
    }

    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
