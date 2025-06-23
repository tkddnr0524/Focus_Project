using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusSkillController : MonoBehaviour
{
    [Header("포커스 스킬 설정")]
    [SerializeField] private KeyCode focusKey = KeyCode.F;               // 스킬 발동 키
    [SerializeField] private Transform focusRangeCircle;                // 전체 범위 원 (스케일 축소됨)
    [SerializeField] private Transform aimIndicator;                    // 조준 에임 원 (방향키로 이동)

    [SerializeField] private float maxFocusRange = 5f;                  // 최대 범위 (월드 단위)
    [SerializeField] private float focusRangeSpriteRadius = 0.5f; // 스프라이트 반지름
    [SerializeField] private float maxFocusTime = 5f;                   // 최대 지속 시간 (초)
    [SerializeField] private float rechargeDelay = 2f;                  // 게이지 소진 후 대기 시간
    [SerializeField] private float recoverSpeed = 1f;                   // 기본 회복 속도 (초당 회복량)
    [SerializeField] private float recoverSpeedBoost = 2f;              // 0 게이지 후 회복 속도 배율

    [SerializeField] private float aimMoveSpeed = 3f;                   // 조준 원 이동 속도 (유닛/초)

    private float currentFocusTime;                                     // 남은 포커스 시간
    public bool isFocusActive = false;                                 // 현재 스킬 발동 여부
    private bool canUseFocus = true;                                    // 사용 가능 상태
    private Vector2 aimOffset = Vector2.zero;                           // 플레이어 기준 조준 위치

    private bool isRecoveryBoosted = false;                             // 0 게이지 후 회복 속도 부스트 여부


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

    // 스킬 발동/해제 처리
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

    // 포커스 모드 진입
    private void ActivateFocus()
    {
        isFocusActive = true;
        aimIndicator.gameObject.SetActive(true);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    // 포커스 모드 해제
    private void DeactivateFocus()
    {
        isFocusActive = false;
        aimIndicator.gameObject.SetActive(false);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // 위치 초기화
        aimOffset = Vector2.zero;
        aimIndicator.localPosition = Vector2.zero;
    }

    // 조준 에임 원 이동
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

    // 게이지 감소 / 회복 및 전체 원 크기 갱신
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
            // 스킬 비활성 상태일 때 회복 처리
            if (!canUseFocus)
            {
                // 게이지 0 상태에서 대기 후 회복 부스트 활성화 대기 중
                // 회복은 코루틴에서 시작됨 (대기 후)
            }
            else
            {
                // 게이지 남아 있고 회복 중일 때 기본 회복 속도로 차오름
                currentFocusTime += recoverSpeed * Time.unscaledDeltaTime;
                currentFocusTime = Mathf.Min(currentFocusTime, maxFocusTime);
            }
        }

        // 전체 범위 원 스케일 조정
        float scaledRange = maxFocusRange * (currentFocusTime / maxFocusTime);
        focusRangeCircle.localScale = Vector3.one * scaledRange;
    }

    // 현재 가능한 순간이동 범위 계산
    private float GetCurrentFocusRange()
    {
        return maxFocusRange * (currentFocusTime / maxFocusTime) * focusRangeSpriteRadius;
    }

    // 조준 위치로 순간이동
    private void TeleportToAim()
    {
        Vector2 target = (Vector2)transform.position + aimOffset;
        transform.position = target;
    }

    // 게이지 재충전 대기 후 다시 사용 가능
    private IEnumerator StartRechargeAfterDelay()
    {
        yield return new WaitForSecondsRealtime(rechargeDelay);
        isRecoveryBoosted = true;
        canUseFocus = true;

        // 부스트 회복 속도로 게이지 채우기
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


    // 현재 전체 범위의 월드 기준 반지름 반환
    public float GetEffectiveWorldRadius()
    {
        return (maxFocusRange * (currentFocusTime / maxFocusTime)) / 2f ;
    }

    // 슬로우 계수 (0.2 = 20% 속도)
    public float GetSlowFactor()
    {
        return 0.1f;
    }
}
