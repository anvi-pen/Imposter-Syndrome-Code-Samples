using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
This script manages the effect that occurs if the player walks into
a water vapor game object, which would be to increase the player's
suspicion level. This script is attached to the Player game object.
*/

public class PlayerCollisionManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "WaterVapor")
        {
            gameObject.GetComponent<PlayerSusManager>().incrementSus();
        }
    }
}
