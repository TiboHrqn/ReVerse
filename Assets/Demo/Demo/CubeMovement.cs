using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    public float speed = 5f; // Vitesse de déplacement

    void Update()
    {
        // Déplacement horizontal et vertical
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calcul du vecteur de déplacement
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f) * speed * Time.deltaTime;

        // Déplacement de l'objet
        transform.Translate(movement);
    }
}
