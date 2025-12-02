using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Input Settings")]
    public InputActionAsset inputActions;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    private float currentSpeed = 0f;

    [Header("Acceleration Settings")]
    public float accelerationRate = 3f;   // Speed increase per second
    public float decelerationRate = 5f;   // Speed decrease per second
    public float maxRunSpeed = 12f;       // Maximum running speed

    [Header("Stack Settings")]
    public int maxStackLimit = 3;
    public bool IsStackFull => itemStacker != null && itemStacker.preStackList.Count >= maxStackLimit;

    public ParticleSystem moneyParticles;

    [Header("Health")]
    public int Health;
    public int currentHealth;

    private BodyStacker itemStacker;
    private PlayerAnimatorController playerAnimatorController;
    private InputAction moveAction;
    private CharacterController characterController;
    private Camera mainCamera;

    private float attackDuration = 1.5f;
    private bool isAttacking = false;

    // movement variables
    Vector2 input;
    Vector3 move;
    float magnitude;

    void Awake()
    {
        playerAnimatorController = GetComponent<PlayerAnimatorController>();
        characterController = GetComponent<CharacterController>();
        itemStacker = GetComponent<BodyStacker>();
        mainCamera = Camera.main;
        EventsManager.playerCollectedMoney += CollectMoney;
        currentHealth = Health;

        var actionMap = inputActions.FindActionMap("Player");
        moveAction = actionMap.FindAction("Move");
    }

    private void CollectMoney()
    {
        moneyParticles.Play();
    }

    void OnEnable()
    {
        moveAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
    }

    void Update()
    {
        if (isAttacking)
            return;

        input = moveAction.ReadValue<Vector2>();
        move = new Vector3(input.x, 0, input.y);
        magnitude = move.magnitude;

        bool isMoving = magnitude > 0.1f;

        // ------------------------------
        // ACCELERATION / DECELERATION LOGIC
        // ------------------------------
        if (isMoving)
        {
            currentSpeed += accelerationRate * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, walkSpeed, maxRunSpeed);
        }
        else
        {
            currentSpeed -= decelerationRate * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, walkSpeed, maxRunSpeed);
        }

        // ------------------------------
        // MOVEMENT & ANIMATION
        // ------------------------------
        if (isMoving)
        {
            characterController.Move(move.normalized * currentSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (currentSpeed > walkSpeed + 0.5f)
                playerAnimatorController.Play(PlayerAnimations.Run);
            else
                playerAnimatorController.Play(PlayerAnimations.Walk);
        }
        else
        {
            playerAnimatorController.Play(PlayerAnimations.Idle);
        }
    }

    // ------------------------------
    // ATTACK SYSTEM
    // ------------------------------
    public void Attack()
    {
        if (isAttacking)
            return;

        StartCoroutine(StartAttack());
    }

    private IEnumerator StartAttack()
    {
        isAttacking = true;
        playerAnimatorController.Play(PlayerAnimations.Attack);
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }

    // ------------------------------
    // DAMAGE SYSTEM
    // ------------------------------
    public void TakeDamage(Transform hitPosition, int damage, float knockbackForce = 0)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("Player died");

        }
    }
}
