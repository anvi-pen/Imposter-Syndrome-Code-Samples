using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
This script is attached to the Player game object. This script manages the player's
suspicion level, and adjusts the suspicion meter UI display and the player's controls
as necessary when the player's suspicion level changes.
*/

public class PlayerSusManager : MonoBehaviour
{
    // Declare necessary variables
    private int susLevel;
    private int maxSus = 9;
    private int sus0 = 0;
    private int sus1 = 3;
    private int sus2 = 6;
    private Color sus0Color = new Color(0.016f, 0.651f, 0.243f);
    private Color sus1Color = new Color(0.965f, 0.867f, 0);
    private Color sus2Color = new Color(0.812f, 0.2f, 0);
    private Slider susMeter;
    private Image fill;

    // Start is called before the first frame update
    void Start()
    {
        // Load the saved suspicion level of the player and adjust the meter UI 
        // as needed to indicate the suspicion level to the player
        susLevel = SavePlayerInfo.susLevel;
        susMeter = GameObject.Find("SusMeter").GetComponent<Slider>();
        fill = susMeter.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        maxSus = (int)susMeter.maxValue;
        sus1 = maxSus / 3;
        sus2 = sus1 * 2;

        susMeter.value = susLevel;
        if (susLevel >= sus2)
        {
            fill.color = sus2Color;
            // Adjust the player's controls depending on their suspicion level
            gameObject.GetComponent<PlayerMovement>().setSus2();
        }
        else if (susLevel >= sus1)
        {
            fill.color = sus1Color;
            // Adjust the player's controls depending on their suspicion level
            gameObject.GetComponent<PlayerMovement>().setSus1();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Save the player's current suspicion level
    public void setSus(int val)
    {
        SavePlayerInfo.susLevel = val;
    }

    // Increment the player's current suspicion level
    public void incrementSus()
    {
        if (susLevel < maxSus)
        {
            susLevel += 1;
            setSus(susLevel);
            susMeter.value = susLevel;

            if (susLevel == sus1)
            {
                fill.color = sus1Color;
                // Adjust the player's controls depending on their suspicion level
                gameObject.GetComponent<PlayerMovement>().setSus1();
            }
            else if (susLevel == sus2)
            {
                fill.color = sus2Color;
                // Adjust the player's controls depending on their suspicion level
                gameObject.GetComponent<PlayerMovement>().setSus2();
            }
        }
    }

    // Decrement the player's current suspicion level
    public void decrementSus()
    {
        if (susLevel > 0)
        {
            susLevel -= 1;
            setSus(susLevel);
            susMeter.value = susLevel;

            if (susLevel == sus0)
            {
                fill.color = sus0Color;
                // Adjust the player's controls depending on their suspicion level
                gameObject.GetComponent<PlayerMovement>().setSus0();
            }
            else if (susLevel == sus1)
            {
                fill.color = sus1Color;
                // Adjust the player's controls depending on their suspicion level
                gameObject.GetComponent<PlayerMovement>().setSus1();
            }
        }
    }
}
