using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FocusSlowManager : MonoBehaviour
{
    private Transform player;
    private FocusSkillController focus;

    [Range(0f, 1f)]
    public float slowFactor = 0.2f;

    private IFocusAffectable affectable;

    private void Start()
    {
        affectable = GetComponent<IFocusAffectable>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        focus = player != null ? player.GetComponent<FocusSkillController>() : null;
    }

    private void Update()
    {
        if (player == null || focus == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                focus = player.GetComponent<FocusSkillController>();
            }
            else
            {
                return;
            }
        }

        float dist = Vector2.Distance(transform.position, player.position);
        bool inFocusRange = focus.IsFocusActive() &&
                            dist <= focus.GetEffectiveWorldRadius();

        float factor = inFocusRange ? slowFactor : 1f;

        if (affectable != null)
        {
            affectable.ApplyFocusSlow(factor);
        }
    }
}
