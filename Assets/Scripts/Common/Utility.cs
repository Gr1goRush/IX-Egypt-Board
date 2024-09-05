using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Utility
{
    public static IEnumerable<T> GetRandomEnumerable<T>(IEnumerable<T> arr)
    {
        System.Random random = new System.Random();
        return arr.OrderBy(x => random.Next());
    }

    public static T[] GetRandomArray<T>(T[] arr)
    {
        return GetRandomEnumerable(arr).ToArray();
    }

    public static void OnAnimation(this MonoBehaviour monoBehaviour, Animator animator, string name, GameAction call, float targetTime)
    {
        monoBehaviour.StartCoroutine(AnimationWatch(animator, name, call, targetTime));
    }

    static IEnumerator AnimationWatch(Animator animator, string name, GameAction call, float targetTime)
    {
        bool isAnimation = false;
        while (!isAnimation)
        {
            isAnimation = animator.GetCurrentAnimatorStateInfo(0).IsName(name);
            yield return null;
        }

        isAnimation = true;
        while (isAnimation)
        {
            AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            isAnimation = animatorStateInfo.IsName(name);
            if (animatorStateInfo.normalizedTime >= targetTime)
            {
                break;
            }
            yield return null;
        }

        if (call != null)
        {
            call.Invoke();
        }
    }
}