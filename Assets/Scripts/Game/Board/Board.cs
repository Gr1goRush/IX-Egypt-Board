using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using UnityEngine;
using static System.Net.WebRequestMethods;

public struct BoardPlayerDancers
{
    public Dancer[] dancers;
}

[System.Serializable]
public struct BoardDancerPlacement
{
    public int playerIndex, pointIndex;
}

public class Board : MonoBehaviour
{
    [SerializeField] private float dancerMoveSpeed;

    [SerializeField] private Dancer dancerOriginal;
    [SerializeField] private Transform finishPoint;
    [SerializeField] private Animator boardAnimator;
    [SerializeField] private SpriteRenderer mainBoard;

    [SerializeField] private BoardPoint[] points;
    [SerializeField] private BoardDancerPlacement[] tutorialPlacements;

    private int moveDistance = 0, movingPlayerIndex = -1;
    private bool dancerSelecting = false;
    private Dancer selectedDancer = null;
    private Vector3 lastTouchWorldPosition = Vector3.zero;

    private BoardPlayerDancers[] playersDancers;
    private Dictionary<BoardHouse, BoardPoint> housesPoints;

    public void Initialize(int playersCount)
    {
        housesPoints = new Dictionary<BoardHouse, BoardPoint>();
        for (int i = 0; i < points.Length; i++)
        {
            BoardPoint point = points[i];
            point.index = i;

            if (point.IsHouse)
            {
                housesPoints.Add(point.House, point);
            }
        }

        RuntimeAnimatorController[] dancerAnimatorControllers = new RuntimeAnimatorController[playersCount];
        playersDancers = new BoardPlayerDancers[playersCount];
        for (int i = 0; i < playersCount; i++)
        {
            dancerAnimatorControllers[i] = PlayersManager.Instance.LoadDancerAnimatorController(i);

            Dancer[] dancers = new Dancer[5];
            BoardPlayerDancers boardPlayerDancers = new BoardPlayerDancers();
            boardPlayerDancers.dancers = dancers;
            playersDancers[i] = boardPlayerDancers;
        }

        if (GameModeManager.Instance.Tutorial)
        {
            CreateTutorialPlacement(dancerAnimatorControllers);
        }
        else
        {
            CreateDefaultPlacement(dancerAnimatorControllers);
        }

        Destroy(dancerOriginal.gameObject);
        dancerOriginal = null;
    }

    private void CreateDancer(int playerIndex, int pointIndex, int dancerIndex, RuntimeAnimatorController dancerAnimatorController)
    {
        Dancer newDancer = Instantiate(dancerOriginal, dancerOriginal.transform.parent);
        newDancer.playerIndex = playerIndex;
        newDancer.SetAnimatorController(dancerAnimatorController);
        newDancer.name = "Dancer " + playerIndex + " " + dancerIndex;

        SetDancerOnPoint(newDancer, pointIndex);

        playersDancers[playerIndex].dancers[dancerIndex] = newDancer;
    }

    private void CreateTutorialPlacement(RuntimeAnimatorController[] dancerAnimatorControllers)
    {
        int[] dancersIndexes = new int[dancerAnimatorControllers.Length];
        for (int i = 0; i < dancersIndexes.Length; i++)
        {
            dancersIndexes[i] = 0;
        }

        for (int i = 0; i < tutorialPlacements.Length; i++)
        {
            BoardDancerPlacement boardDancerPlacement = tutorialPlacements[i];
            int playerIndex = boardDancerPlacement.playerIndex;

            CreateDancer(playerIndex, boardDancerPlacement.pointIndex, dancersIndexes[playerIndex], dancerAnimatorControllers[playerIndex]);

            dancersIndexes[playerIndex]++;
        }
    }

    private void CreateDefaultPlacement(RuntimeAnimatorController[] dancerAnimatorControllers)
    {
        int playersCount = dancerAnimatorControllers.Length;

        for (int playerIndex = 0; playerIndex < playersCount; playerIndex++)
        {
            RuntimeAnimatorController dancerAnimatorController = dancerAnimatorControllers[playerIndex];

            int pointIndex = playerIndex;

            for (int dancerIndex = 0; dancerIndex < 5; dancerIndex++)
            {
                CreateDancer(playerIndex, pointIndex, dancerIndex, dancerAnimatorController);

                pointIndex += playersCount;
            }
        }
    }

    private void SetDancerOnPoint(Dancer dancer, int pointIndex)
    {
        BoardPoint boardPoint = points[pointIndex];
        SetDancerOnPoint(dancer, boardPoint);

        if (boardPoint.IsHouse)
        {
            AudioController.Instance.Sounds.PlayOneShot("house");
        }
    }

    private void SetDancerOnPoint(Dancer dancer, BoardPoint boardPoint)
    {
        boardPoint.SetDancer(dancer);
    }

    private void MoveDancerOnPoint(Dancer dancer, BoardPoint boardPoint, GameAction onCompleted = null)
    {
        GameAction action = () => SetDancerOnPoint(dancer, boardPoint);
        if (onCompleted != null)
        {
            action += onCompleted;
        }
        dancer.MoveDancer(boardPoint.GetSockerPosition(), action);
    }

    public void ProcessPlayerMove(int playerIndex, int distance, bool isAI)
    {
        if(isAI && TutorialController.Instance.IsActive)
        {
            distance = 1;
        }

        selectedDancer = null;
        moveDistance = distance;
        movingPlayerIndex = playerIndex;

        List<Dancer> movableDancers = new List<Dancer>();
        BoardPlayerDancers boardPlayerDancers = playersDancers[playerIndex];
        for (int i = 0; i < boardPlayerDancers.dancers.Length; i++)
        {
            Dancer dancer = boardPlayerDancers.dancers[i];
            if (dancer.Completed || !DancerCanMove(dancer, distance))
            {
                dancer.SetMovable(false);
                continue;
            }

            dancer.SetMovable(!isAI);
            movableDancers.Add(dancer);
        }

        if (movableDancers.Count == 0)
        {
            OnMoveCompleted();
            return;
        }

        if (isAI)
        {
            if (TutorialController.Instance.IsActive)
            {
                selectedDancer = boardPlayerDancers.dancers[1];
            }
            else
            {
                selectedDancer = SelectDancerForAIMove(movableDancers);
            }

            BoardPoint targetPoint = points[selectedDancer.point.index + distance];

            Dancer anotherDancer = targetPoint.Dancer;
            BoardPoint defaultPoint = selectedDancer.point;

            defaultPoint.SetDancer(null);
            targetPoint.SetDancer(null);

            if (anotherDancer != null)
            {
                MoveDancerOnPoint(anotherDancer, defaultPoint);
            }

            Dancer _dancer = selectedDancer;
            MoveDancerOnPoint(selectedDancer, targetPoint, () => OnDancerMoved(_dancer, true));

            AudioController.Instance.Sounds.PlayOneShot("dancer_move");
        }
        else
        {
            dancerSelecting = true;
        }

        TutorialController.Instance.MakeAction("process_move_start");
    }

    private Dancer SelectDancerForAIMove(List<Dancer> _dancers)
    {
        List<Dancer> preferableDancers = new List<Dancer>();
        foreach (Dancer _dancer in _dancers)
        {
           BoardPoint targetPoint = points[_dancer.point.index + moveDistance];
            if (targetPoint.HasDancer())
            {
                if(targetPoint.Dancer.playerIndex != movingPlayerIndex)
                {
                    preferableDancers.Add(_dancer);
                    continue;
                }
            }

            if (!targetPoint.IsHouse)
            {
                preferableDancers.Add(_dancer);
                continue;
            }

            if(IsHouseToComplete(targetPoint.House))
            {
                return _dancer;
            }

            if (!IsHouseToResurrect(targetPoint.House, moveDistance))
            {
                preferableDancers.Add(_dancer);
            }
        }

        if(preferableDancers.Count > 0)
        {
            return GetDancerFurther(preferableDancers);
        }

        return GetDancerFurther(_dancers);
    }

    private Dancer GetDancerFurther(List<Dancer> _dancers)
    {
        return _dancers.OrderByDescending((dancer) => dancer.point.index).ToList()[0];
    }

    private void Update()
    {
        if (!dancerSelecting)
        {
            return;
        }

        if (InputUtility.TouchStopped())
        {
            Vector3 screenPosition = InputUtility.GetTouchPosition();
            Vector3 worldPosition = CameraController.Instance.ScreenToWorld(screenPosition);

            Collider2D collider2D = Physics2D.OverlapPoint(worldPosition, LayerMask.GetMask("Dancer"));
            if (collider2D != null && collider2D.TryGetComponent(out Dancer dancer) && dancer.Movable && !dancer.IsMoving)
            {
                selectedDancer = dancer;

                MoveSelectedDancerOnMoveDistance();
            }
            else
            {
                selectedDancer = null;
            }
        }
    }

    private bool PathContainsBeautyHouse(int startIndex, int targetIndex)
    {
        BoardPoint beautyHousePoint = housesPoints[BoardHouse.Beauty];
        if (beautyHousePoint.index > startIndex && beautyHousePoint.index <= targetIndex)
        {
            return true;
        }

        return false;
    }

    private void MoveSelectedDancerOnMoveDistance()
    {
        Dancer _dancer = selectedDancer;

        if (selectedDancer.point.IsHouse && IsHouseToComplete(selectedDancer.point.House))
        {
            OnDancerMoved(_dancer, false);
            return;
        }

        BoardPoint targetPoint = points[selectedDancer.point.index + moveDistance];

        if(PathContainsBeautyHouse(selectedDancer.point.index, targetPoint.index))
        {
            targetPoint = housesPoints[BoardHouse.Beauty];
        }

        Dancer anotherDancer = targetPoint.Dancer;
        BoardPoint defaultPoint = selectedDancer.point;

        defaultPoint.SetDancer(null);
        targetPoint.SetDancer(null);

        if (anotherDancer != null)
        {
            MoveDancerOnPoint(anotherDancer, defaultPoint);
        }

        MoveDancerOnPoint(selectedDancer, targetPoint, () => OnDancerMoved(_dancer, true));

        AudioController.Instance.Sounds.PlayOneShot("dancer_move");

        dancerSelecting = false;
    }

    private bool PointHasDancerOfPlayer(BoardPoint boardPoint, int playerIndex) 
    {
        if (boardPoint.HasDancer())
        {
            if (boardPoint.Dancer.playerIndex == playerIndex)
            {
                return true;
            }
        }

        return false;
    }

    private void SetDefaultDancersState(int playerIndex)
    {
        BoardPlayerDancers boardPlayerDancers = playersDancers[playerIndex];
        for (int i = 0; i < boardPlayerDancers.dancers.Length; i++)
        {
            Dancer dancer = boardPlayerDancers.dancers[i];
            dancer.SetMovable(false);
        }
    }

    public void HighlightOnlyOneDancer(int playerIndex, int dancerIndex)
    {
        BoardPlayerDancers boardPlayerDancers = playersDancers[playerIndex];
        for (int i = 0; i < boardPlayerDancers.dancers.Length; i++)
        {
            Dancer dancer = boardPlayerDancers.dancers[i];
            dancer.SetHighlight(dancerIndex == i);
        }
    }

    private bool DancerCanMove(Dancer dancer, int distance)
    {
        return DancerCanMove(dancer, dancer.point, distance);
    }

    private bool DancerCanMove(Dancer dancer, BoardPoint startPoint, int distance)
    {
        if (startPoint.IsHouse)
        {
            if (startPoint.House == BoardHouse.Sun)
            {
                if (distance == 1)
                {
                    return true;
                }

                return false;
            }

            if (startPoint.House == BoardHouse.ThreeTruths)
            {
                if (distance == 3)
                {
                    return true;
                }

                return false;
            }

            if (startPoint.House == BoardHouse.IsisNephthys)
            {
                if (distance == 2)
                {
                    return true;
                }

                return false;
            }
        }

        int targetPointIndex = startPoint.index + distance;
        if (targetPointIndex >= points.Length)
        {
            return false;
        }

        BoardPoint targetPoint = points[targetPointIndex];
        if (targetPoint.index <= startPoint.index)
        {
            return false;
        }
        if ((targetPoint.index - startPoint.index) != distance)
        {
            return false;
        }

        if (distance >= 2)
        {
            int nextPointIndex = startPoint.index + 1;
            int filledPointsCount = 0;
            for (int i = nextPointIndex; i < points.Length; i++)
            {
                if (points[i].HasDancer())
                {
                    filledPointsCount++;
                    if (filledPointsCount >= 2)
                    {
                        return false;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        if (PathContainsBeautyHouse(startPoint.index, targetPoint.index))
        {
            if (housesPoints[BoardHouse.Beauty].HasDancer())
            {
                return false;
            }
        }

        bool targetPointIsHouse = targetPoint.IsHouse;
        if (!targetPoint.HasDancer())
        {
            if (targetPointIsHouse)
            {
                if (targetPoint.House == BoardHouse.Ba)
                {
                    if (!DancerCanMove(dancer, points[targetPointIndex], distance))
                    {
                        return false;
                    }
                }
                else if (IsHouseToResurrect(targetPoint.House, distance) && !CanResurrect())
                {
                    return false;
                }
            }

            return true;
        }

        Dancer anotherDancer = targetPoint.Dancer;
        if (targetPointIsHouse)
        {
            return false;
        }

        if (anotherDancer.playerIndex == dancer.playerIndex)
        {
            return false;
        }

        if (targetPoint.index >= 0)
        {
            BoardPoint previousPoint = points[targetPoint.index - 1];
            if (PointHasDancerOfPlayer(previousPoint, anotherDancer.playerIndex))
            {
                return false;
            }
        }

        if (targetPoint.index < (points.Length - 1))
        {
            BoardPoint nextPoint = points[targetPoint.index + 1];
            if (PointHasDancerOfPlayer(nextPoint, anotherDancer.playerIndex))
            {
                return false;
            }
        }

        return true;
    }

    private void ResurrectDancer(Dancer dancer, GameAction onCompleted = null)
    {
        MoveDancerOnPoint(dancer, housesPoints[BoardHouse.Resurrection], onCompleted);
    }

    private bool CanResurrect()
    {
        return !housesPoints[BoardHouse.Resurrection].HasDancer();
    }

    private bool IsHouseToComplete(BoardHouse house)
    {
        if (house == BoardHouse.Sun)
        {
            //if (distance == 1)
            //{
            //    return true;
            //}

            return true;
        }

        if (house == BoardHouse.ThreeTruths)
        {
            //if (distance == 3)
            //{
            //    return true;
            //}

            return true;
        }

        if (house == BoardHouse.IsisNephthys)
        {
            //if (distance == 2)
            //{
            //    return true;
            //}

            return true;
        }

        return false;
    }

        private bool IsHouseToResurrect(BoardHouse house, int distance)
    {
        if (house == BoardHouse.Water)
        {
            return true;
        }
        
        return false;
    }

    private void CompleteDancer(Dancer dancer, GameAction onCompleted = null)
    {
        GameAction action = dancer.SetCompleted;
        if (onCompleted != null)
        {
            action += onCompleted;
        }

        dancer.MoveDancer(finishPoint.transform.position, action);
    }

    private void OnDancerMoved(Dancer dancer, bool pointChanged)
    {
        VibrationManager.Instance.Vibrate();

        BoardPoint boardPoint = dancer.point;
        if (boardPoint.IsHouse)
        {
            BoardHouse house = boardPoint.House;

            if(house == BoardHouse.Ba)
            {
                BoardPoint nextPoint = points[boardPoint.index + moveDistance];
                if (nextPoint.HasDancer())
                {
                    Dancer anotherDancer = nextPoint.Dancer;
                    nextPoint.SetDancer(null);

                    MoveDancerOnPoint(anotherDancer, points[boardPoint.index - moveDistance]);
                }

                MoveDancerOnPoint(dancer, nextPoint, OnMoveCompleted);
                return;
            }
            
            if (IsHouseToResurrect(house, moveDistance))
            {
                ResurrectDancer(dancer, OnMoveCompleted);
                return;
            }

            if (IsHouseToComplete(house) && !pointChanged)
            {
                CompleteDancer(dancer, OnMoveCompleted);
                return;
            }
        }

        OnMoveCompleted();
    }

    private void OnMoveCompleted()
    {
        SetDefaultDancersState(movingPlayerIndex);

        GameController.Instance.OnMoveCompleted();
    }

    public Dictionary<BoardHouse, int> GetHousesPlayers()
    {
        Dictionary<BoardHouse, int> housesPlayersDictionary = new Dictionary<BoardHouse, int>();
        foreach (KeyValuePair<BoardHouse, BoardPoint> item in housesPoints)
        {
            if (item.Value.HasDancer())
            {
                housesPlayersDictionary.Add(item.Key, item.Value.Dancer.playerIndex);
            }
        }

        return housesPlayersDictionary;
    } 

    public int GetActiveDancersCount(int playerIndex)
    {
        int count = 0;

        BoardPlayerDancers boardPlayerDancers = playersDancers[playerIndex];
        for (int i = 0; i < boardPlayerDancers.dancers.Length; i++)
        {
            Dancer dancer = boardPlayerDancers.dancers[i];
            if (!dancer.Completed)
            {
                count++;
            }
        }

        return count;
    }

    public void ShowDestroyAnimation(GameAction action)
    {
        playersDancers[0].dancers[0].transform.parent.gameObject.SetActive(false);
        points[0].transform.parent.gameObject.SetActive(false);

        mainBoard.gameObject.SetActive(false);
        boardAnimator.gameObject.SetActive(true);

        this.OnAnimation(boardAnimator, "Destroy", action, 1f);
        boardAnimator.SetTrigger("Destroy");
    }

    public Vector3 GetBoardPosition()
    {
        return mainBoard.transform.position;
    }

    public Vector3 GetBoardSize()
    {
        return mainBoard.bounds.size;
    }

    //private void Reset()
    //{
    //    Transform pointsParent = transform.GetChild(1);
    //    points = new BoardPoint[pointsParent.childCount];
    //    for (int i = 0; i < points.Length; i++)
    //    {
    //        points[i] = pointsParent.GetChild(i).GetComponent<BoardPoint>();
    //        points[i].name = "Point " + i;
    //    }
    //}
}
