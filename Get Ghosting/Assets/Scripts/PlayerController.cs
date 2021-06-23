using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 interimPosition;
    public Vector3 playPosition;
    private float playerPosZ;
    private float distanceFromCamera;
    private float mouseX;
    private float mouseY;
    private Vector3 mouseWorldPos;
    Transform center;
    Camera mainCam;
    Transform camTransform;
    float zPos;
    Vector2 velocity;
    public float moveRadiusMin = 0.5f;
    public float moveRadiusMax = 8f;
    public float speed = 1f;
    public float driftStrength = 1f;
    Vector2 faceDirection;
    [SerializeField] Animator bodyAnimator;
    [SerializeField] Animator armsAnimator;
    [SerializeField] Animator mouthAnimator;
    Rigidbody2D rb2D;
    public bool clickActive;
    public bool booActive;
    AudioSource booAudio;


    private void Awake()
    {
        center = transform.Find("Center").transform;
        mainCam = Camera.main;
        camTransform = mainCam.transform;
        zPos = transform.position.z;
        rb2D = GetComponent<Rigidbody2D>();
        booAudio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ResetPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.inPlay)
        {
            UpdateVelocity();
            UpdateFaceDirection();
            transform.position += (Vector3)velocity * Time.deltaTime;

            UpdateAction();
        }
    }

    public void ResetPlayer()
    {
        transform.position = interimPosition;
        bodyAnimator.SetBool("moving", false);
        StopBoo();
        armsAnimator.SetBool("waving", false);
    }

    void UpdateVelocity()
    {
        // Get direction from current position to cursor
        Vector2 mousePos = (Vector2)GetMousePosition();
        Vector2 moveVector = mousePos - (Vector2)center.position;

        // Get velocity
        if (moveVector.magnitude > moveRadiusMin && moveVector.magnitude < moveRadiusMax)
            velocity = moveVector.normalized * speed;
        else
            velocity = Vector2.zero;

        // velocity.x = Input.GetAxis("Horizontal") * speed;
        // velocity.y = Input.GetAxis("Vertical") * speed;

        if (velocity.x != 0)
            bodyAnimator.SetBool("moving", true);
        else
            bodyAnimator.SetBool("moving", false);
    }

    void UpdateFaceDirection()
    {
        if (transform.localScale.x < 0 && velocity.x > 0 || transform.localScale.x > 0 && velocity.x < 0)
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    void UpdateAction()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartBoo();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            StopBoo();
        }

        if (Input.GetMouseButtonDown(0))
        {
            armsAnimator.SetBool("waving", true);
            clickActive = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            armsAnimator.SetBool("waving", false);
            clickActive = false;
        }
    }

    void StartBoo()
    {
        mouthAnimator.SetBool("howling", true);
        booActive = true;
        booAudio.Play();
    }

    void StopBoo()
    {
        mouthAnimator.SetBool("howling", false);
        booActive = false;
        booAudio.Stop();
    }

    // Returns In-World Transform position of cursor on the Player's Z plane
    Vector3 GetMousePosition()
    {
        // Get cursor position in pixels and clamp to screen
        mouseX = Mathf.Clamp(Input.mousePosition.x, 0, Screen.width);
        mouseY = Mathf.Clamp(Input.mousePosition.y, 0, Screen.height);

        // Get Z axis position of player and distance from camera
        distanceFromCamera = zPos - camTransform.position.z;

        // Turn mouse position into world position
        mouseWorldPos = mainCam.ScreenToWorldPoint(new Vector3(mouseX, mouseY, distanceFromCamera));

        // Return world position
        return mouseWorldPos;
    }
}
