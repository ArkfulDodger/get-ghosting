using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuVictim : MonoBehaviour
{
    public Animator skeletonAnimator;
    public Animator faceAnimator;
    public Animator handsAnimator;
    public Animator feetAnimator;

    // Start is called before the first frame update
    void Start()
    {
        skeletonAnimator.SetFloat("panic", 5f);
        faceAnimator.SetFloat("panic", 5f);
        handsAnimator.SetFloat("panic", 5f);
        feetAnimator.SetFloat("panic", 5f);
        feetAnimator.SetBool("moving", true);
    }
}
