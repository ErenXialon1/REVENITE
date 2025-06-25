using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PlayerMovementController : MonoBehaviour
{
    // ====================================
    // 1. Movement Settings (Editor Adjustable)
    // ====================================
    [Header("Movement Settings")]
    [Tooltip("Maximum horizontal speed of the character.")]
    [SerializeField] private float maxSpeed = 8f;
    [Tooltip("Acceleration rate when input is active.")]
    [SerializeField] private float acceleration = 20f;
    [Tooltip("Deceleration rate when input stops.")]
    [SerializeField] private float deceleration = 25f;
    [SerializeField] private MonoBehaviour inputReaderRef; // Inspector üzerinden atanacak
    private IInputReader inputReader;
    public Vector2 LastMoveDirection { get; private set; } = Vector2.down;
    // ====================================
    // 4. Component References
    // ====================================
    [Header("Component References")]
    [Tooltip("Reference to the Rigidbody2D component.")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator; //  Animator referansı
    [SerializeField] private SpriteRenderer spriteRenderer; // Sağa/sola çevirmek için
    [Header("Animation Smoothing")]
    [Tooltip("Input kesildiğinde Idle'a geçmeden önceki saniye cinsinden bekleme süresi.")]
    [SerializeField] private float idleTransitionDelay = 0.1f;
    private float timeSinceNoInput = 0f;
    private float updateTick = 0.05f;
    // ====================================
    // 5. Input & State Variables
    // ====================================


    public Vector2 inputVector = Vector2.zero;
    private Coroutine customUpdate = null;
    public bool canMove, velocityCut;

    private void Awake()
    {
        // Ensure that the Rigidbody2D component is assigned.
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer==null) spriteRenderer = GetComponent<SpriteRenderer>(); // Genellikle karakterin sprite'ı bir alt nesnede olur.
    
    inputReader = inputReaderRef as IInputReader;

    }
    private void OnEnable()
    {
        if (inputReader != null)
        {
            inputReader.MovesEvent += OnMove;
        }
        StartCustomUpdate();
    }

    private void OnDisable()
    {
        if (inputReader != null)
        {
            inputReader.MovesEvent -= OnMove;
        }
        StopCustomUpdate();
    }
    private void OnMove(Vector2 input)
    {
        inputVector = input;
        if (inputVector != Vector2.zero)
        {
            LastMoveDirection = inputVector.normalized;
        }


    }
    private void StartCustomUpdate()
    {
        if (customUpdate == null)
            customUpdate = StartCoroutine(CustomUpdate());
    }

    private void StopCustomUpdate()
    {
        if (customUpdate != null)
        {
            StopCoroutine(customUpdate);
            customUpdate = null;
        }
    }

    public void ResetVelocity()
    {
        rb.linearVelocity = Vector2.zero;
        velocityCut = true;
    }

    // HandleMovement() breaks the overall movement logic into smaller, manageable methods.
    private void HandleMovement()
    {
        if (!canMove)
        {
            
        }
        // Calculate the desired target speed based on input.
        float targetSpeedX = inputVector.x * maxSpeed;
        float targetSpeedY = inputVector.y * maxSpeed;
        // Determine the difference between the current speed and target speed.
        float speedDifferenceX = targetSpeedX - rb.linearVelocity.x;
        float speedDifferenceY = targetSpeedY - rb.linearVelocity.y;

        // Calculate the force to be applied using a separate method.
        Vector2 movementForce = CalculateMovementForce(speedDifferenceX, speedDifferenceY, acceleration, deceleration);

        // Apply the calculated force on the horizontal axis.
        rb.AddForce(movementForce);

        // Ensure the character's horizontal speed does not exceed maxSpeed.
        CapHorizontalSpeed();
        UpdateAnimation();
    }
    private void UpdateAnimation()
    {
        // Oyuncunun bir tuşa basıp basmadığını kontrol et.
        bool isMoving = inputVector.sqrMagnitude > 0.01f;
       

        // Eğer hareket ediyorsak (hızımız çok küçük değilse)
        if (isMoving)
        {
            timeSinceNoInput = 0f;
            animator.SetFloat("Speed", 1f);
            // Hareket yönünü Animator'a gönder.
            animator.SetFloat("LastMoveX", inputVector.normalized.x);
            animator.SetFloat("LastMoveY", inputVector.normalized.y);

            // --- YATAY HAREKET İÇİN SPRITE'I ÇEVİRME ---
            if (inputVector.x > 0.01f && spriteRenderer.flipX== true)
            {
                spriteRenderer.flipX = false; // Sağa gidiyorsa normal
            }
            else if (inputVector.x < -0.01f && spriteRenderer.flipX == false)
            {
                spriteRenderer.flipX = true; // Sola gidiyorsa X ekseninde çevir
            }
        }
        else // Eğer duruyorsak
        {
            
            timeSinceNoInput += updateTick;
            // Zamanlayıcı, bizim belirlediğimiz küçük gecikme süresini geçtiyse,
            // bu demektir ki oyuncu gerçekten durdu.
            if (timeSinceNoInput >= idleTransitionDelay)
            {
                // Animator'a "durdu" bilgisini gönder.
                animator.SetFloat("Speed", 0f);
            }
            
        }
    }
    // CalculateMovementForce() returns the force needed based on whether there is active input.
    private Vector2 CalculateMovementForce(float speedDiffX, float speedDiffY, float currentAcceleration, float currentDeceleration)
    {
        // If there is significant horizontal input, use acceleration; otherwise, use deceleration.
        if (inputVector.sqrMagnitude > 0.01f)
        {
            return new Vector2(speedDiffX * currentAcceleration, speedDiffY * currentAcceleration);
        }
        else
        {
            return new Vector2(-rb.linearVelocity.x * currentDeceleration, -rb.linearVelocity.y * currentDeceleration);
        }
    }
    // CapHorizontalSpeed() ensures the character does not exceed the maximum allowed speed.
    private void CapHorizontalSpeed()
    {
        if (Mathf.Abs(rb.linearVelocity.x) > maxSpeed)
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * maxSpeed, rb.linearVelocity.y);
        }
        if (Mathf.Abs(rb.linearVelocity.y) > maxSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Sign(rb.linearVelocity.y) * maxSpeed);
        }
    }
    // ====================================
    // 11. Physics State & Coyote‑Time Routine
    // ====================================
    private IEnumerator CustomUpdate()
    {

        while (true)
        {




            HandleMovement();
            yield return new WaitForSeconds(updateTick);//sorun çıkarsa fixedupdate olarak değişebilir
        }
    }
    public void StopCanMove()
    {
        canMove = false;
    }
    public void StartCanMove()
    {
        canMove=true;
    }


}
