using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardHouse
{
    Beauty, Ba, IsisNephthys, Resurrection, Sun, ThreeTruths, Water
}

public class BoardPoint : MonoBehaviour
{
    public Dancer Dancer => dancer;

    [HideInInspector] public int index;

    [SerializeField] private Transform dancerSocket;

    public bool IsHouse => isHouse;
    public BoardHouse House => house;

    [Header("House:")]
    [SerializeField] private bool isHouse;
    [SerializeField] private BoardHouse house;
    [SerializeField] private Animator houseAnimator;

    [SerializeField] private Dancer dancer = null;

    public void SetDancer(Dancer _dancer)
    {
        if(dancer != null)
        {
            dancer.point = null;
        }

        dancer = _dancer;
        if(dancer == null)
        {
            UpdateHighlight();

            return;
        }

        if(dancer.point != null)
        {
            dancer.point.SetDancer(null);
        }

        dancer.point = this;
        dancer.transform.position = dancerSocket.transform.position;
        UpdateHighlight();
    }

    public Vector3 GetSockerPosition()
    {
        return dancerSocket.transform.position;
    }

    public bool HasDancer()
    {
        return dancer != null;
    }

    private void UpdateHighlight()
    {
        if (houseAnimator != null)
        {
            houseAnimator.SetBool("Highlight", HasDancer());
        }
    }
}
