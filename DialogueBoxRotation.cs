using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This script makes the floating dialogue box positioned above an NPC to always
face the player in the 3D environment. This script should be attached to the
dialogue box game object.
*/

public class DialogueBoxRotation : MonoBehaviour
{
    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerTransform = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(playerTransform);
    }
}
