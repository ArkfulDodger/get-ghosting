using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGhost : MonoBehaviour
{
    public Animator skeletonAnimator;
    public Animator bodyAnimator;
    public Animator handsAnimator;
    public Animator mouthAnimator;

    // Start is called before the first frame update
    void Start()
    {
        bodyAnimator.SetBool("moving", true);
        handsAnimator.SetBool("waving", true);
        mouthAnimator.SetBool("howling", true);
    }
}
