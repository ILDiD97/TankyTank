using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FloorBehaviour : MonoBehaviour
{

    [SerializeField]
    private List<EntrancesBehevior> entranceBeheviour;

    [SerializeField]
    private Transform playerSpawn;

    [SerializeField]
    private int floorType;

    [SerializeField]
    private List<Vector3> obstaclePositions;

    public void SetEntrancesState(CellSpace currentCell, bool isEditor)
    {
        if (currentCell.nord) entranceBeheviour[0].SelectEntranceActive(true, isEditor);
        else entranceBeheviour[0].SelectEntranceActive(false, isEditor);

        if (currentCell.ovest) entranceBeheviour[1].SelectEntranceActive(true, isEditor);
        else entranceBeheviour[1].SelectEntranceActive(false, isEditor);

        if (currentCell.sud) entranceBeheviour[2].SelectEntranceActive(true, isEditor);
        else entranceBeheviour[2].SelectEntranceActive(false, isEditor);

        if (currentCell.est) entranceBeheviour[3].SelectEntranceActive(true, isEditor);
        else entranceBeheviour[3].SelectEntranceActive(false, isEditor);
    }

    public void DestroyWall(WallAxis boardAxis, bool isEditor)
    {
        switch (boardAxis)
        {
            case WallAxis.Nord:
                if(!isEditor) Destroy(entranceBeheviour[0].gameObject);
                else DestroyImmediate(entranceBeheviour[0].gameObject);
                break;
            case WallAxis.Ovest:
                if (!isEditor) Destroy(entranceBeheviour[1].gameObject);
                else DestroyImmediate(entranceBeheviour[1].gameObject);
                break;
            case WallAxis.Sud:
                if (!isEditor) Destroy(entranceBeheviour[2].gameObject);
                else DestroyImmediate(entranceBeheviour[2].gameObject);
                break;
            case WallAxis.Est:
                if (!isEditor) Destroy(entranceBeheviour[3].gameObject);
                else DestroyImmediate(entranceBeheviour[3].gameObject);
                break;
            case WallAxis.NordOvest:
                if (!isEditor)
                {
                    Destroy(entranceBeheviour[0].gameObject);
                    Destroy(entranceBeheviour[1].gameObject);
                }
                else
                {
                    DestroyImmediate(entranceBeheviour[0].gameObject);
                    DestroyImmediate(entranceBeheviour[1].gameObject);
                }
                break;
            case WallAxis.OvestSud:
                if (!isEditor)
                {
                    Destroy(entranceBeheviour[1].gameObject);
                    Destroy(entranceBeheviour[2].gameObject);
                }
                else
                {
                    DestroyImmediate(entranceBeheviour[1].gameObject);
                    DestroyImmediate(entranceBeheviour[2].gameObject);
                }
                break;
            case WallAxis.SudEst:
                if (!isEditor)
                {
                    DestroyImmediate(entranceBeheviour[2].gameObject);
                    DestroyImmediate(entranceBeheviour[3].gameObject);
                }
                else
                {
                    DestroyImmediate(entranceBeheviour[2].gameObject);
                    DestroyImmediate(entranceBeheviour[3].gameObject);
                }
                break;
            case WallAxis.EstNord:
                if (!isEditor)
                {
                    Destroy(entranceBeheviour[3].gameObject);
                    Destroy(entranceBeheviour[0].gameObject);
                }
                else
                {
                    DestroyImmediate(entranceBeheviour[3].gameObject);
                    DestroyImmediate(entranceBeheviour[0].gameObject);
                }
                break;
        }
    }


    public void SetPlayerStart()
    {
        GameStatus.Instance.SetPositionPlayer(playerSpawn.position);
    }
   
}
