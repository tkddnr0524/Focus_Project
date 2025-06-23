using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private bool isGrounded;
    private FocusSkillController focusSkillController;

    [Header("�̵�/���� ����")]
    public float moveSpeed = 5f;         // �̵� �ӵ�
    public float jumpForce = 10f;        // ���� ��
    public Transform groundCheck;        // �ٴ� üũ ��ġ
    public LayerMask groundLayer;        // �Ϲ� �ٴ� ���̾�
    public LayerMask thinPlatformLayer;  // ���� �ٴ� ���̾�

    private float originalGravity;       // �߰��ϴ� �ڵ� �Դϴ�: �߷� ������ ���� ����

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        focusSkillController = GetComponent<FocusSkillController>();

        originalGravity = rb.gravityScale; // �߰��ϴ� �ڵ� �Դϴ�
    }

    void Update()
    {
        // ��Ŀ�� ��� �߿� �̵�/���� ���� �� �߷� ����
        if (focusSkillController.isFocusActive)
        {
            // �߰��ϴ� �ڵ� �Դϴ�: ���߿��� ����
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            return;
        }
        // ��� ���� �� �߷� ����
        rb.gravityScale = originalGravity; // �߰��ϴ� �ڵ� �Դϴ�

        // �¿� �̵�
        float h = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(h * moveSpeed, rb.velocity.y);

        // �ٴ� üũ
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            0.3f,
            groundLayer | thinPlatformLayer
        );

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
