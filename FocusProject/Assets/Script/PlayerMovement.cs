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

    [Header("�̵�/���� ����")]
    public float moveSpeed = 12f;         // �̵� �ӵ�
    public float jumpForce = 40f;        // ���� ��
    public Transform groundCheck;        // �ٴ� üũ ��ġ
    public LayerMask groundLayer;        // �Ϲ� �ٴ� ���̾�
    public LayerMask thinPlatformLayer;  // ���� �ٴ� ���̾�

    [Header("�ܻ� ����Ʈ")]
    [SerializeField] private AfterImageManager afterImageMgr;

    private Animator anim; // Animator ����
    private float originalGravity;       // �߰��ϴ� �ڵ� �Դϴ�: �߷� ������ ���� ����

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        focusSkillController = GetComponent<FocusSkillController>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        afterImageMgr = GetComponent<AfterImageManager>();

        originalGravity = rb.gravityScale; // �߰��ϴ� �ڵ� �Դϴ�
    }

    void Update()
    {
        // ��Ŀ�� ��� �߿� �̵�/���� ���� �� �߷� ����
        if (focusSkillController.isFocusActive)
        {
            // ���߿��� ����
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;

            // �ִϸ��̼� ���� ���·�
            anim.SetBool("IsRunning", false);
            anim.SetBool("IsGrounded", false);
            anim.SetFloat("yVelocity", 0f);
            return;

        }
        // ��� ���� �� �߷� ����
        rb.gravityScale = originalGravity; 

        // �¿� �̵�
        float h = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);

        if (h != 0)
        {
            spriteRenderer.flipX = h < 0; // �����̸� true
        }

        // �ٴ� üũ
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            0.6f,
            groundLayer | thinPlatformLayer
        );

        // �ִϸ����� �Ķ���� ����
        anim.SetBool("IsRunning", h != 0);                // �¿� �Է� ����
        anim.SetBool("IsGrounded", isGrounded);           // �ٴڿ� ��Ҵ���
        anim.SetFloat("yVelocity", rb.velocity.y);        // ����/���� �Ǵܿ�

        // ����
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // ���� �ٴ� ���
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
