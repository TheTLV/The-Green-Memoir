using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 4f;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 moveInput;
    private Vector2 lastMoveDir;
    private bool isMoving;
    private bool isRunning;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // --- Nhận input ---
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Normalize để đi chéo không nhanh hơn
        moveInput.Normalize();

        // --- Kiểm tra có đang chạy không ---
        isRunning = Input.GetKey(KeyCode.LeftShift); // giữ Shift để chạy
        isMoving = moveInput != Vector2.zero;

        // --- Cập nhật hướng di chuyển ---
        if (isMoving)
        {
            lastMoveDir = moveInput; // ghi nhớ hướng cuối cùng
        }

        // --- Cập nhật Animator ---
        animator.SetFloat("CurrentMoveX", moveInput.x);
        animator.SetFloat("CurrentMoveY", moveInput.y);
        animator.SetFloat("LastMoveX", lastMoveDir.x);
        animator.SetFloat("LastMoveY", lastMoveDir.y);
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", isRunning);
    }

    void FixedUpdate()
    {
        // --- Tính tốc độ ---
        float speed = isRunning ? runSpeed : walkSpeed;

        // --- Di chuyển ---
        rb.linearVelocity = moveInput * speed;
    }
}
