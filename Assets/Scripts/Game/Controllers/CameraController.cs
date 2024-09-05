using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private Camera _camera;

    public Vector3 ScreenToWorld(Vector3 screenPos)
    {
        return _camera.ScreenToWorldPoint(screenPos);
    }

    public void SetSize(Vector3 boardPosition, Vector3 boardSize)
    {
        float screenWorldHeight = boardSize.x / _camera.aspect;
        _camera.orthographicSize = screenWorldHeight / 2f;

        float y = boardPosition.y - (boardSize.y / 2f) + (screenWorldHeight / 2f);
        transform.position = new Vector3(0, y, transform.position.z);
    }
}
