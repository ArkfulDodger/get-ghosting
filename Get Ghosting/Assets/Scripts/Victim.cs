using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victim : MonoBehaviour
{
    public bool activeScare;
    public bool activeCalming;
    public bool stabilized;
    public float panic = 1f;
    public float score;
    float minPanic = 1f;
    float maxPanic = 5f;
    public float panicSpeed = 0.5f;
    public float cooldownSpeed = 0.3f;
    [SerializeField] Animator breathingAnimator;
    [SerializeField] Animator handsAnimator;
    [SerializeField] Animator feetAnimator;
    [SerializeField] Animator faceAnimator;
    [SerializeField] SpriteRenderer bodyRenderer;
    [SerializeField] SpriteRenderer headRenderer;
    [SerializeField] SpriteRenderer hairRenderer;
    [SerializeField] SpriteRenderer frontHandRenderer;
    [SerializeField] SpriteRenderer rearHandRenderer;
    [SerializeField] List<Color> shirtColors;
    [SerializeField] List<Color> skinColors;
    [SerializeField] List<Color> hairColors;
    Rigidbody2D rb2D;
    Vector2 velocity;
    Vector2 direction;
    public float speed = 1f;
    public float panicSpeedMod = 0.2f;


    private void Awake() {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        StartCoroutine("DirectionChange");
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.inPlay)
        {
            UpdatePanicValue();
            UpdatePanicAnimation(panic);
            UpdateVelocity();
            UpdateFaceDirection();
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.inPlay)
            Move();
    }

    void LevelOver()
    {
        StopAllCoroutines();
    }

    IEnumerator DirectionChange()
    {
        while (GameManager.instance.inPlay)
        {
            float nextChangeTime = Random.Range(2f, 4f) / panic;

            yield return new WaitForSeconds(nextChangeTime);

            ChangeDirection();
        }
    }

    void ChangeDirection()
    {
        int directionIndex = Random.Range(0,5);
        if (panic > 4)
            directionIndex = Mathf.Max(1, directionIndex);

        switch (directionIndex)
        {
            case 0:
            {
                direction = Vector2.zero;
                break;
            }
            case 1:
            {
                direction = Vector2.up;
                break;
            }
            case 2:
            {
                direction = Vector2.down;
                break;
            }
            case 3:
            {
                direction = Vector2.left;
                break;
            }
            case 4:
            {
                direction = Vector2.right;
                break;
            }

            default:
            {
                direction = Vector2.zero;
                break;
            }
        }
    }

    void UpdateVelocity()
    {
        velocity = direction * (speed + (panicSpeedMod * panic));

        if (velocity != Vector2.zero)
            feetAnimator.SetBool("moving", true);
        else
            feetAnimator.SetBool("moving", false);
    }

    void UpdateFaceDirection()
    {
        if (transform.localScale.x < 0 && velocity.x > 0 || transform.localScale.x > 0 && velocity.x < 0)
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    void Move()
    {
        rb2D.MovePosition(rb2D.position + (velocity * Time.deltaTime));
    }

    void UpdatePanicValue()
    {
        if (activeScare)
        {
            panic = Mathf.Min(maxPanic, panic + (panicSpeed * Time.deltaTime));
            StopCoroutine("StabilizeTimer");
            activeCalming = false;
            stabilized = false;
        }
        else if (!activeScare && !stabilized)
        {
            stabilized = true;
            StartCoroutine("StabilizeTimer");
        }
        else if (activeCalming)
            panic = Mathf.Max(minPanic, panic - (cooldownSpeed * Time.deltaTime));

        score = (panic - minPanic)/(maxPanic - minPanic);
    }

    void UpdatePanicAnimation(float panic)
    {
        breathingAnimator.SetFloat("panic", panic);
        handsAnimator.SetFloat("panic", panic);
        feetAnimator.SetFloat("panic", panic);
        faceAnimator.SetFloat("panic", panic);
    }

    public void RefreshLook()
    {
        Color hairColor = hairColors[Random.Range(0, hairColors.Count)];
        Color skinColor = skinColors[Random.Range(0, skinColors.Count)];
        Color shirtColor = shirtColors[Random.Range(0, shirtColors.Count)];

        bodyRenderer.color = shirtColor;
        hairRenderer.color = hairColor;
        headRenderer.color = skinColor;
        frontHandRenderer.color = skinColor;
        rearHandRenderer.color = skinColor;
    }

    public void ResetVictim()
    {
        activeScare = false;
        activeCalming = false;
        stabilized = false;
        panic = 1f;
    }

    IEnumerator StabilizeTimer()
    {
        yield return new WaitForSeconds(GameManager.instance.stableTime);
        activeCalming = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        direction *= -1;
    }
}
