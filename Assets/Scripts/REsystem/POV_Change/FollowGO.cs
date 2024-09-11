using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGO : MonoBehaviour
{

    //public Vector3 offset {get; private set; }
    [SerializeField] private GameObject goToFollow;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        transform.position = goToFollow.transform.position;
        transform.rotation = goToFollow.transform.rotation;
    }
}
