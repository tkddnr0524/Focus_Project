using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private bool isGrounded;
    private FocusSkillController focusSkillController;

    [Header("이동/점프 설정")]
    public float moveSpeed = 5f;         // 이동 속도
    public float jumpForce = 10f;        // 점프 힘
    public Transform groundCheck;        // 바닥 체크 위치
    public LayerMask groundLayer;        // 일반 바닥 레이어
    public LayerMask thinPlatformLayer;  // 얇은 바닥 레이어

    private float originalGravity;       // 추가하는 코드 입니다: 중력 스케일 원본 보관

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        focusSkillController = GetComponent<FocusSkillController>();

        originalGravity = rb.gravityScale; // 추가하는 코드 입니다
    }

    void Update()
    {
        // 포커스 모드 중엔 이동/점프 차단 및 중력 억제
        if (focusSkillController.isFocusActive)
        {
            // 추가하는 코드 입니다: 공중에서 멈춤
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            return;
        }
        // 모드 해제 시 중력 복구
        rb.gravityScale = originalGravity; // 추가하는 코드 입니다

        // 좌우 이동
        float h = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);

        // 바닥 체크
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            0.3f,
            groundLayer | thinPlatformLayer
        );

        // 점프
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // 얇은 바닥 통과
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            StartCoroutine(PassThroughPlatform());
        }
    }

    private IEnumerator PassThroughPlatform()
    {
        Collider2D thin = Physics2D.OverlapCircle(
            groundCheck.position,
            0.2f,
            thinPlatformLayer
        );

        if (thin != null)
        {
            Physics2D.IgnoreCollision(playerCollider, thin, true);
            yield return new WaitForSeconds(0.5f);
            Physics2D.IgnoreCollision(playerCollider, thin, false);
        }
    }
}
