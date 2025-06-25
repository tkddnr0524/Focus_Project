using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private bool isGrounded;
    private FocusSkillController focusSkillController;
    private SpriteRenderer spriteRenderer;

    [Header("이동/점프 설정")]
    public float moveSpeed = 12f;         // 이동 속도
    public float jumpForce = 40f;        // 점프 힘
    public Transform groundCheck;        // 바닥 체크 위치
    public LayerMask groundLayer;        // 일반 바닥 레이어
    public LayerMask thinPlatformLayer;  // 얇은 바닥 레이어

    [Header("잔상 이펙트")]
    [SerializeField] private AfterImageManager afterImageMgr;

    private Animator anim; // Animator 참조
    private float originalGravity;       // 추가하는 코드 입니다: 중력 스케일 원본 보관

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        focusSkillController = GetComponent<FocusSkillController>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        afterImageMgr = GetComponent<AfterImageManager>();

        originalGravity = rb.gravityScale; // 추가하는 코드 입니다
    }

    void Update()
    {
        // 포커스 모드 중엔 이동/점프 차단 및 중력 억제
        if (focusSkillController.isFocusActive)
        {
            // 공중에서 멈춤
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;

            // 애니메이션 정지 상태로
            anim.SetBool("IsRunning", false);
            anim.SetBool("IsGrounded", false);
            anim.SetFloat("yVelocity", 0f);
            return;

        }
        // 모드 해제 시 중력 복구
        rb.gravityScale = originalGravity; 

        // 좌우 이동
        float h = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);

        if (h != 0)
        {
            spriteRenderer.flipX = h < 0; // 왼쪽이면 true
        }

        // 바닥 체크
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            0.6f,
            groundLayer | thinPlatformLayer
        );

        // 애니메이터 파라미터 갱신
        anim.SetBool("IsRunning", h != 0);                // 좌우 입력 여부
        anim.SetBool("IsGrounded", isGrounded);           // 바닥에 닿았는지
        anim.SetFloat("yVelocity", rb.velocity.y);        // 점프/낙하 판단용

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
            0.4f,
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
