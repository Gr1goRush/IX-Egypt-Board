using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;

public class Dancer : MonoBehaviour
{
    public BoardPoint point;
    [HideInInspector] public int playerIndex = 0;

    public bool Movable { get; private set; }
    public bool IsMoving { get; private set; }
    public bool Completed { get; private set; }
    public bool Highlight { get; private set; }

    [SerializeField] private Animator animator;

    [SerializeField] private float moveTime = 1f;

    private void Awake()
    {
        Movable = false;
        IsMoving = false;
        Completed = false;
    }

    public void SetAnimatorController(RuntimeAnimatorController runtimeAnimatorController)
    {
        animator.runtimeAnimatorController = runtimeAnimatorController;
    }

    public void SetMovable(bool movable)
    {
        Movable = movable;
        SetHighlight(movable);
    }

    public void SetHighlight(bool highlight)
    {
        Highlight = highlight;

        animator.SetBool("Highlight", highlight);
    }

    public void SetCompleted()
    {
        Completed = true;
        if(point != null)
        {
            point.SetDancer(null);
            point = null;
        }
        gameObject.SetActive(false);
    }

    public void MoveDancer(Vector3 destination, GameAction action)
    {
        StartCoroutine(MovingDancerOnPoint(destination, action));
    }

    private IEnumerator MovingDancerOnPoint(Vector3 destination, GameAction action)
    {
        IsMoving = true;

        Vector3 origin = transform.position;
        float oneStepOffset = 1 / moveTime;
        for (float t = 0; t <= 1f; t += Time.deltaTime * oneStepOffset)
        {
            transform.position = Vector3.Lerp(origin, destination, t);

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        IsMoving = false;

        action?.Invoke();
    }

}
