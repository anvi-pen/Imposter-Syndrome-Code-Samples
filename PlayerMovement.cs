using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/*
This script should be attached to the Player game object, as it controls the movement of
the player. Depending on the suspicion level of the player, the movement controls
are changed by this script as follows:

When the player's suspicion level is at 0, the player uses normal controls to move:
--> Use W to move forward, S to move backward, A to move left, and D to move right
--> Move mouse to the left to move your view (camera) to the left
--> Move mouse to the right to move your view (camera) to the right

When the player's suspicion level is at 1, the player uses modified controls to move:
--> Use A to move forward, D to move backward, W to move left, and S to move right
--> Move mouse to the right to move your view (camera) to the left
--> Move mouse to the left to move your view (camera) to the right

When the player's suspicion level is at 2, the player uses the same modified controls as
at level 1, in addition to split screen cameras being activated to create the illusion
that the player's left eye is looking to the right and the player's right eye is looking
to the left.
*/

public class PlayerMovement : MonoBehaviour
{
    // Declare necessary variables
    private float xMove = 0;
    private float zMove = 0;
    private float rotate;

    private Rigidbody rb = null;

    private bool setNeg = false;
    private bool setPos = true;
    private bool setSplit = false;

    [SerializeField] private float speed = 4;
    [SerializeField] private float rotationSensitivity = 4;
    private bool sus1 = false;
    private bool sus2 = false;

    [SerializeField] private float bobSpeed = 14f;
    [SerializeField] private float bobAmount = 0.05f;

    private float defaultY = 0;
    private float timer;

    private Camera[] playerCameras;

    // Identify objects that can trigger a minigame (need to load a minigame scene)
    enum TriggerObject
    {
        NONE,
        PIANO,
        TEETH
    }

    private TriggerObject triggerObject = TriggerObject.NONE;

    // Functions that are called depending on the player's suspicion level
    // The player's suspicion level affects the player's movement controls
    public void setSus0()
    {
        sus1 = false;
        sus2 = false;
    }

    public void setSus1()
    {
        sus1 = true;
        sus2 = false;
    }

    public void setSus2()
    {
        sus1 = true;
        sus2 = true;
    }

    void PlayWalkSound()
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.Play();
    }

    // Creates a head bob effect while the player is moving (to create the illusion of walking)
    private void HeadBob()
    {
        playerCameras = gameObject.GetComponentsInChildren<Camera>();
        if (Mathf.Abs(xMove) > 0.1f || Mathf.Abs(zMove) > 0.1f)
        {
            timer += Time.deltaTime * bobSpeed;
            foreach (Camera playerCamera in playerCameras)
                playerCamera.transform.localPosition = new Vector3(
                    playerCamera.transform.localPosition.x,
                    defaultY + (Mathf.Sin(timer) * bobAmount),
                    playerCamera.transform.localPosition.z);
        }
        else
        {
            foreach (Camera playerCamera in playerCameras)
                playerCamera.transform.localPosition = new Vector3(
                    playerCamera.transform.localPosition.x,
                    defaultY,
                    playerCamera.transform.localPosition.z);
        }
    }

    void Awake()
    {
        playerCameras = gameObject.GetComponentsInChildren<Camera>();
        defaultY = playerCameras[0].transform.localPosition.y;

        // Load any previously saved data on the player's position and rotation in the current 3D scene
        gameObject.transform.position = SavePlayerInfo.position;
        gameObject.transform.localRotation = Quaternion.Euler(0, SavePlayerInfo.rotationY, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        rotate = SavePlayerInfo.rotationY;

        Cursor.lockState = CursorLockMode.Locked;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = (gameObject.transform.forward * zMove) + (gameObject.transform.right * xMove);
        Vector3 moveAmount = moveDirection * Time.deltaTime * speed;
        rb.MovePosition(rb.position + moveAmount);

        // If the player's suspicion level is low enough,
        // the camera controls are reverted to the normal controls
        // Moving the mouse to the left will move the camera to the left
        // Moving the mouse to the right will move the camera to the right
        if (!sus1 && !setPos)
        {
            rotationSensitivity *= (-1f);
            setNeg = false;
            setPos = true;
        }
        // If the player's suspicion level is at least a certain value,
        // the controls for camera movement are reversed
        // Moving the mouse to the left will move the camera to the right
        // Moving the mouse to the right will move the camera to the left
        if (sus1 && !setNeg)
        {
            rotationSensitivity *= (-1f);
            setNeg = true;
            setPos = false;
        }
        rotate += Mouse.current.delta.x.ReadValue() * rotationSensitivity;
        transform.localRotation = Quaternion.Euler(0, rotate, 0);

        // If the player's suspicion level is at least a certain value,
        // the split screen cameras are activated
        // This creates the illusion that the player's left eye is looking to the right
        // and the player's right eye is looking to the left
        if (sus2 && !setSplit)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
            gameObject.transform.GetChild(2).gameObject.SetActive(true);
            setSplit = true;
        }
        // If the player's suspicion level is low enough,
        // the split screen cameras are deactivated and the normal camera is reactivated
        else if (!sus2 && setSplit)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
            gameObject.transform.GetChild(2).gameObject.SetActive(false);
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            setSplit = false;
        }

        HeadBob();
    }

    // Player can move using normal controls if their suspicion level is low enough
    // W is forward, S is backward, A is left, and D is right
    private void OnMove(InputValue movementValue)
    {
        if (!sus1)
        {
            Vector2 movementVector = movementValue.Get<Vector2>();

            xMove = movementVector.x;
            zMove = movementVector.y;
            PlayWalkSound();
        }
    }

    // Player moves using modified controls if their suspicion level is at least a certain value
    // A is forward, D is backward, W is left, and S is right
    private void OnJankMove(InputValue movementValue)
    {
        if (sus1)
        {
            Vector2 movementVector = movementValue.Get<Vector2>();

            xMove = movementVector.x;
            zMove = movementVector.y;
            PlayWalkSound();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter");
        if (other.gameObject.tag == "Piano")
            triggerObject = TriggerObject.PIANO;
        if (other.gameObject.tag == "Teeth")
            triggerObject = TriggerObject.TEETH;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger Exit");
        triggerObject = TriggerObject.NONE;
    }

    // If the player presses Q, this function will be called
    // If the player is also near an object that can trigger a minigame,
    // the player's current position and rotation in the current
    // scene will be saved and the scene with the minigame will be loaded
    private void OnInteract()
    {
        if (triggerObject == TriggerObject.PIANO)
        {
            SavePlayerInfo.position = gameObject.transform.position;
            SavePlayerInfo.rotationY = gameObject.transform.eulerAngles.y;
            SceneManager.LoadScene("Piano");
        }
        if (triggerObject == TriggerObject.TEETH)
        {
            SavePlayerInfo.position = gameObject.transform.position;
            SavePlayerInfo.rotationY = gameObject.transform.eulerAngles.y;
            SceneManager.LoadScene("brushing_mini_game");
        }
    }
}
