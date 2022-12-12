using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayfarerAnimHelper : MonoBehaviour
{
    private Animator animator;

    public void SetHitboxActiveL(float val)
    {
        animator.SetFloat("MeleeL.active", val);
    }

    public void SetHitboxActiveR(float val)
    {
        animator.SetFloat("MeleeR.active", val);
    }
}
