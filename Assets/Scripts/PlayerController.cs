using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float baseJumpForce = 7f;

    [Header("Escala")]
    public float scaleStep = 0.2f;
    public float minScale = 0.5f;
    public float maxScale = 2f;
    public float scaleSmoothSpeed = 5f;

    [Header("Animaciones")]
    public float sleepDelay = 5f;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded;

    private float targetScale;
    private float currentScale;
    private float jumpForce;

    private float idleTimer = 0f;

    // Dirección del gato
    private bool facingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentScale = Mathf.Abs(transform.localScale.x);
        targetScale = currentScale;

        jumpForce = baseJumpForce;
    }

    void Update()
    {
        // =========================
        // MOVIMIENTO
        // =========================

        float move = 0f;

        bool movingLeft =
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.LeftArrow);

        bool movingRight =
            Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.RightArrow);

        if (movingLeft)
        {
            move = -1f;
        }

        if (movingRight)
        {
            move = 1f;
        }

        rb.velocity = new Vector2(move * speed, rb.velocity.y);

        // =========================
        // GIRAR PERSONAJE
        // =========================

        if (move < 0 && !facingRight)
        {
            Flip();
        }
        else if (move > 0 && facingRight)
        {
            Flip();
        }

        // =========================
        // SALTO
        // =========================

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // =========================
        // ESCALADO
        // =========================

        if (Input.GetKeyDown(KeyCode.X))
        {
            targetScale = Mathf.Clamp(
                targetScale + scaleStep,
                minScale,
                maxScale
            );
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            targetScale = Mathf.Clamp(
                targetScale - scaleStep,
                minScale,
                maxScale
            );
        }

        // =========================
        // ANIMACIONES
        // =========================

        bool isRunning = movingLeft || movingRight;
        bool isJumping = !isGrounded;
        bool isQuiet = !isRunning && isGrounded;

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isQuiet", isQuiet);

        // =========================
        // SLEEP
        // =========================

        if (Input.anyKey)
        {
            idleTimer = 0f;
            animator.SetBool("isSleeping", false);
        }
        else
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= sleepDelay)
            {
                animator.SetBool("isSleeping", true);
            }
        }
    }

    void LateUpdate()
    {
        // Mantener siempre recto
        transform.rotation = Quaternion.identity;

        // Escala suave
        currentScale = Mathf.Lerp(
            currentScale,
            targetScale,
            Time.deltaTime * scaleSmoothSpeed
        );

        float direction = facingRight ? 1f : -1f;

        transform.localScale = new Vector3(
            currentScale * direction,
            currentScale,
            1f
        );

        // Ajustar salto según tamaño
        jumpForce = baseJumpForce * currentScale;
    }

    void Flip()
    {
        facingRight = !facingRight;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Solo cuenta como suelo si viene desde abajo
                if (contact.normal.y > 0.5f)
                {
                    isGrounded = true;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // =========================
        // MUERTE
        // =========================

        if (collision.CompareTag("Thorn"))
        {
            // Reiniciar escena rápidamente
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // =========================
        // VICTORIA
        // =========================

        if (collision.CompareTag("Goal"))
        {
            Debug.Log("GANASTE");

            // Pausar el juego
            Time.timeScale = 0f;
        }
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}