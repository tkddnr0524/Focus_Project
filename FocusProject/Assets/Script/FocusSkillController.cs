using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusSkillController : MonoBehaviour
{
    [Header("��Ŀ�� ��ų ����")]
    [SerializeField] private KeyCode focusKey = KeyCode.F;               // ��ų �ߵ� Ű
    [SerializeField] private Transform focusRangeCircle;                // ��ü ���� �� (������ ��ҵ�)
    [SerializeField] private Transform aimIndicator;                    // ���� ���� �� (����Ű�� �̵�)

    [SerializeField] private float maxFocusRange = 5f;                  // �ִ� ���� (���� ����)
    [SerializeField] private float focusRangeSpriteRadius = 0.5f; // ��������Ʈ ������
    [SerializeField] private float maxFocusTime = 5f;                   // �ִ� ���� �ð� (��)
    [SerializeField] private float rechargeDelay = 2f;                  // ������ ���� �� ��� �ð�
    [SerializeField] private float recoverSpeed = 1f;                   // �⺻ ȸ�� �ӵ� (�ʴ� ȸ����)
    [SerializeField] private float recoverSpeedBoost = 2f;              // 0 ������ �� ȸ�� �ӵ� ����

    [SerializeField] private float aimMoveSpeed = 3f;                   // ���� �� �̵� �ӵ� (����/��)

    private float currentFocusTime;                                     // ���� ��Ŀ�� �ð�
    public bool isFocusActive = false;                                 // ���� ��ų �ߵ� ����
    private bool canUseFocus = true;                                    // ��� ���� ����
    private Vector2 aimOffset = Vector2.zero;                           // �÷��̾� ���� ���� ��ġ

    private bool isRecoveryBoosted = false;                             // 0 ������ �� ȸ�� �ӵ� �ν�Ʈ ����


    private void Start()
    {
        currentFocusTime = maxFocusTime;
        focusRangeCircle.gameObject.SetActive(true);
        aimIndicator.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleInput();
        if (isFocusActive)
        {
            HandleAimMovement();
        }

        UpdateFocusGauge();
    }

    // ��ų �ߵ�/���� ó��
    private void HandleInput()
    {
        if (Input.GetKeyDown(focusKey) && canUseFocus)
        {
            ActivateFocus();
        }

        if (Input.GetKeyUp(focusKey) && isFocusActive)
        {
            TeleportToAim();
            DeactivateFocus();
        }
    }

    // ��Ŀ�� ��� ����
    private void ActivateFocus()
    {
        isFocusActive = true;
        aimIndicator.gameObject.SetActive(true);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    // ��Ŀ�� ��� ����
    private void DeactivateFocus()
    {
        isFocusActive = false;
        aimIndicator.gameObject.SetActive(false);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // ��ġ �ʱ�ȭ
        aimOffset = Vector2.zero;
        aimIndicator.localPosition = Vector2.zero;
    }

    // ���� ���� �� �̵�
    private void HandleAimMovement()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (input.sqrMagnitude > 0f)
        {
            aimOffset += input.normalized * aimMoveSpeed * Time.unscaledDeltaTime;
            aimOffset = Vector2.ClampMagnitude(aimOffset, GetCurrentFocusRange());
            aimIndicator.localPosition = aimOffset;
        }
    }

    // ������ ���� / ȸ�� �� ��ü �� ũ�� ����
    private void UpdateFocusGauge()
    {
        if (isFocusActive)
        {
            currentFocusTime -= Time.unscaledDeltaTime;
            if (currentFocusTime <= 0f)
            {
                currentFocusTime = 0f;
                canUseFocus = false;

                TeleportToAim();
                DeactivateFocus();
                StartCoroutine(StartRechargeAfterDelay());
            }
        }
        else
        {
            // ��ų ��Ȱ�� ������ �� ȸ�� ó��
            if (!canUseFocus)
            {
                // ������ 0 ���¿��� ��� �� ȸ�� �ν�Ʈ Ȱ��ȭ ��� ��
                // ȸ���� �ڷ�ƾ���� ���۵� (��� ��)
            }
            else
            {
                // ������ ���� �ְ� ȸ�� ���� �� �⺻ ȸ�� �ӵ��� ������
                currentFocusTime += recoverSpeed * Time.unscaledDeltaTime;
                currentFocusTime = Mathf.Min(currentFocusTime, maxFocusTime);
            }
        }

        // ��ü ���� �� ������ ����
        float scaledRange = maxFocusRange * (currentFocusTime / maxFocusTime);
        focusRangeCircle.localScale = Vector3.one * scaledRange;
    }

    // ���� ������ �����̵� ���� ���
    private float GetCurrentFocusRange()
    {
        return maxFocusRange * (currentFocusTime / maxFocusTime) * focusRangeSpriteRadius;
    }

    // ���� ��ġ�� �����̵�
    private void TeleportToAim()
    {
        Vector2 target = (Vector2)transform.position + aimOffset;
        transform.position = target;
    }

    // ������ ������ ��� �� �ٽ� ��� ����
    private IEnumerator StartRechargeAfterDelay()
    {
        yield return new WaitForSecondsRealtime(rechargeDelay);
        isRecoveryBoosted = true;
        canUseFocus = true;

        // �ν�Ʈ ȸ�� �ӵ��� ������ ä���
        while (currentFocusTime < maxFocusTime)
        {
            currentFocusTime += recoverSpeed * recoverSpeedBoost * Time.unscaledDeltaTime;
            yield return null;
        }
        currentFocusTime = maxFocusTime;
        isRecoveryBoosted = false;
    }

    public bool IsFocusActive()
    {
        return isFocusActive;
    }


    // ���� ��ü ������ ���� ���� ������ ��ȯ
    public float GetEffectiveWorldRadius()
    {
        return (maxFocusRange * (currentFocusTime / maxFocusTime)) / 2f ;
    }

    // ���ο� ��� (0.2 = 20% �ӵ�)
    public float GetSlowFactor()
    {
        return 0.1f;
    }
}
