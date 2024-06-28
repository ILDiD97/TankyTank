using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueRandomInteger;

public class CellSpace
{
    public Vector3 worldPos;
    public WallAxis destroyableWallAxis;
    public FloorBehaviour floorBehaviour;
    public bool visited;
    public bool hasFactory;
    public bool nord;
    public bool ovest;
    public bool sud;
    public bool est;
    public int floorType;
    public int selectionX;
    public int selectionZ;
    public List<Vector3> obstaclePositions;
    public List<CellSpace> neighbors = new List<CellSpace>();

}

public class BoardGenerator : MonoBehaviour
{
    private CellSpace[,] board = new CellSpace[100, 100];

    private List<CellSpace> cellUsed;

    private List<FloorBehaviour> floorBehaviours;

    private List<GameObject> obstacles;

    [SerializeField]
    private List<GameObject> floorsPrefab;

    [SerializeField]
    private int floorType;

    [SerializeField]
    private int floorMaxNumber;

    [SerializeField]
    private List<GameObject> obstaclesPrefab;

    [SerializeField]
    private int obstacleMultiplier = 6;

    [SerializeField]
    private List<GameObject> enemiesTankPrefab;

    private int cellSelectionX = 50;

    private int cellSelectionZ = 50;

    private int startXPos = -2500;

    private int startZPos = -2500;

    public List<CellSpace> CellUsed { get => cellUsed; }

    private void Awake()
    {
        //print(board.Length);
        StartCells(false);
    }

    public void StartCells(bool isEditor)
    {
        cellUsed = new List<CellSpace>();
        floorBehaviours = new List<FloorBehaviour>();
        obstacles = new List<GameObject>();
        HQManager.Instance.FactoryLimit = floorMaxNumber / HQManager.Instance.LimitFactoryParam;

        //Generate Board of cells
        UIManager.Instance.FadeSetVisibility(true);
        CellBoardGenerator(isEditor);
    }

    private void CellBoardGenerator(bool isEditor)
    {
        for (int x = 0; x < 100; x++)
        {
            for (int z = 0; z < 100; z++)
            {

                //Generate Cell in Listo Board
                board[x, z] = new CellSpace();

                board[x, z].obstaclePositions = new List<Vector3>(); 

                //Detailing Cell
                board[x, z].worldPos = new Vector3(startXPos + (50 * x), -0.5f, startZPos + (50 * z));
                board[x, z].selectionX = x;
                board[x, z].selectionZ = z;

                //Add previous cell in vertical and on prevoius add this cell
                if (z != 0 && board[x, z - 1] != null)
                {
                    board[x, z].neighbors.Add(board[x, z - 1]);
                    board[x, z - 1].neighbors.Add(board[x, z]);
                }

                //Add previous cell in horizontal and on prevoius add this cell
                if (x != 0 && board[x - 1, z] != null)
                {
                    board[x, z].neighbors.Add(board[x - 1, z]);
                    board[x - 1, z].neighbors.Add(board[x, z]);
                }

            }
        }

        //start Choosing cell for instantiate floor
        StartCoroutine(GenerateMaze(isEditor));
    }


    //Generate level on Corutine
    private IEnumerator GenerateMaze(bool isEditor)
    {
        List<CellSpace> currentCell;
        //Get first Random Floor Type and Get the current Cell
        int randomFloor = TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(3);
        List<CellSpace> firstCell = GetPivotCell(randomFloor);

        int randomCellAvaliable;
        for (int x = 0; x < floorMaxNumber; x++)
        {
            if (x == 0)
            {
                currentCell = firstCell;
            }
            else
            {
                randomFloor = TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(3);
                randomCellAvaliable = TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(cellUsed.Count);
                cellSelectionX = cellUsed[randomCellAvaliable].selectionX;
                cellSelectionZ = cellUsed[randomCellAvaliable].selectionZ;
                currentCell = GetPivotCell(randomFloor);
            }

            if (currentCell.Count > 0)
            {
                foreach (CellSpace cell in currentCell)
                {
                    cellUsed.Add(cell);
                    cell.floorType = randomFloor;
                    cell.obstaclePositions = new List<Vector3>();
                }
            }
            else
            {
                x--;
            }
        }

        GameObject floor;
        foreach (CellSpace cellSpace in cellUsed)
        {
            floor = Instantiate(floorsPrefab[floorType], transform);
            floor.name = floor.name + "-" + cellSpace.selectionX + "-" + cellSpace.selectionZ + "-" + cellSpace.floorType;
            floor.transform.position = cellSpace.worldPos;
            FloorBehaviour floorBehaviour = floor.GetComponent<FloorBehaviour>();

            foreach (CellSpace cell in cellSpace.neighbors)
            {
                if (cell.visited) ChooseActiveAxis(cellSpace, cell);
            }

            int column = 0;
            int row = 0;
            Vector3 startPosition = new Vector3(cellSpace.worldPos.x - 20, 0, cellSpace.worldPos.z - 20);
            for (int j = 0; j < 81; j++)
            {
                if (column != 4 && row != 4)
                {
                    Vector3 currentPosition = new Vector3(startPosition.x + 5 * column, 0, startPosition.z + 5 * row);

                    cellSpace.obstaclePositions.Add(currentPosition);
                }

                if (column < 8) column++;
                else
                {
                    column = 0;
                    row++;
                }
            }

            if (floorBehaviour)
            {
                floorBehaviours.Add(floorBehaviour);
                floorBehaviour.SetEntrancesState(cellSpace, isEditor);
            }

            if (cellSpace.floorType != 0) floorBehaviour.DestroyWall(cellSpace.destroyableWallAxis, isEditor);

            yield return new WaitForSeconds(0.01f);
        }
        SpawnRandomObstacles();

        int playerCell = TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(floorBehaviours.Count);

        floorBehaviours[playerCell].SetPlayerStart();

        for (int k = 0; k < HQManager.Instance.FactoryLimit; k++)
        {
            int randomCell = TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(cellUsed.Count);
            if (randomCell == playerCell || cellUsed[randomCell].hasFactory && cellUsed.Count != 1)
            {
                k--;
            }
            else
            {
                HQManager.Instance.SpawnFactory(cellUsed[randomCell]);
                cellUsed[randomCell].hasFactory = true;
            }
        }

        yield return new WaitForSeconds(1);

        foreach (NavigationBuilder navigation in FindObjectsOfType<NavigationBuilder>())
        {
            navigation.Refresh();
        }
        
    }

    //private void SpawnEnemies(int player)
    //{
    //    GameObject enemy = null;
    //    Vector3 spawnPosition;
    //    for (int j = 0; j < floorMaxNumber; j++)
    //    {
    //        int randomCell = TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(cellUsed.Count);
    //        if (randomCell == player && cellUsed.Count != 1)
    //        {
    //            j--;
    //        }
    //        else
    //        {
    //            int randomWorldPosition = TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(cellUsed[randomCell].obstaclePositions.Count);
    //            enemy = Instantiate(enemiesTankPrefab[TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(enemiesTankPrefab.Count)], transform);
    //            spawnPosition = new Vector3(cellUsed[randomCell].obstaclePositions[randomWorldPosition].x, 0, cellUsed[randomCell].obstaclePositions[randomWorldPosition].z);
    //            enemy.transform.localPosition = spawnPosition;
    //            cellUsed[randomCell].obstaclePositions.RemoveAt(randomWorldPosition);
    //            enemiesTank.Add(enemy);
    //            GameStatus.Instance.PopulateEnemies(enemy.GetComponent<AIPlayerSearch>());
    //        }
    //    }

    //    GameStatus.Instance.CalculateEnemyHealth();
    //}

    private int ObstacleNumber()
    {
        return floorMaxNumber * obstacleMultiplier;
    }

    private void SpawnRandomObstacles()
    {
        int obstacleMaxNumber = ObstacleNumber();
        GameObject obstacle = null;
        for (int o = 0; o < obstacleMaxNumber; o++)
        {
            int randomCell = TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(cellUsed.Count);
            int randomWorldPosition = TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(cellUsed[randomCell].obstaclePositions.Count);
            obstacle = Instantiate(obstaclesPrefab[TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(obstaclesPrefab.Count)], transform);
            obstacle.transform.position = cellUsed[randomCell].obstaclePositions[randomWorldPosition];
            cellUsed[randomCell].obstaclePositions.RemoveAt(randomWorldPosition);
            obstacles.Add(obstacle);
        }

        UIManager.Instance.TimeFormStart();
    }

    private void ChooseActiveAxis(CellSpace floorCell, CellSpace neighborCell)
    {
        if (floorCell.selectionZ < neighborCell.selectionZ)
        {
            floorCell.est = true;
        }

        if (floorCell.selectionZ > neighborCell.selectionZ)
        {
            floorCell.ovest = true;
        }

        if (floorCell.selectionX < neighborCell.selectionX)
        {
            floorCell.nord = true;
        }

        if (floorCell.selectionX > neighborCell.selectionX)
        {
            floorCell.sud = true;
        }
    }

    //Get Pivot Cell for Instantiate Next Floor Type
    private List<CellSpace> GetPivotCell(int floorSelectedType)
    {
        List<CellSpace> cellSpaces = new List<CellSpace>();
        CellSpace firstCell;
        CellSpace secondCell = null;
        CellSpace thirdCell = null;

        switch (floorSelectedType)
        {
            case 0:

                //Get cell
                firstCell = QuodSelectionBase();

                //Choose if need to repeat selection of cell
                if (firstCell != null)
                {
                    cellSpaces.Add(firstCell);
                    firstCell.visited = true;
                }
                return cellSpaces;
            case 1:

                //Get Cells
                firstCell = QuodSelectionBase();

                //Controll if Previous Cell Is Not Visited
                if (firstCell != null)
                {
                    firstCell.visited = true;
                    secondCell = RectSelectionBase(firstCell);
                }
                else
                {
                    break;
                }

                //Choose if need to repeat selection of cell
                if (secondCell != null)
                {
                    cellSpaces.Add(firstCell);
                    cellSpaces.Add(secondCell);
                    secondCell.visited = true;
                    SetCellWalls(cellSpaces);
                }
                else
                {
                    firstCell.visited = false;
                }
                return cellSpaces;
            case 2:
                //Get Cells
                firstCell = QuodSelectionBase();

                //Controll if Previous Cell Is Not Visited
                if (firstCell != null)
                {
                    firstCell.visited = true;
                    secondCell = RectSelectionBase(firstCell);
                }
                else
                {
                    break;
                }

                Direction directionAxis;
                //Controll if Previous Cell Is Not Visited
                if (secondCell != null)
                {
                    directionAxis = GetDirection(firstCell, secondCell);
                    secondCell.visited = true;
                    thirdCell = LSelectionBase(secondCell, directionAxis);
                }
                else
                {
                    break;
                }

                //Choose if need to repeat selection of cell
                if (thirdCell != null)
                {
                    cellSpaces.Add(firstCell);
                    cellSpaces.Add(secondCell);
                    cellSpaces.Add(thirdCell);
                    thirdCell.visited = true;
                    SetCellWalls(cellSpaces);
                }
                else
                {
                    firstCell.visited = false;
                    secondCell.visited = false;
                }
                return cellSpaces;
            default:
                return cellSpaces;
        }
        return cellSpaces;
    }

    private void SetCellWalls(List<CellSpace> currentCellSpaces)
    {
        for(int i = 0; i < currentCellSpaces.Count; i++)
        {
            if(i == 0)
            {
                currentCellSpaces[i].destroyableWallAxis = ControllCellPosition(currentCellSpaces[i], currentCellSpaces[i + 1]);
            }
            else if(i == currentCellSpaces.Count - 1)
            {
                currentCellSpaces[i].destroyableWallAxis = ControllCellPosition(currentCellSpaces[i], currentCellSpaces[i - 1]);
            }
            else
            {
                WallAxis prevWall = ControllCellPosition(currentCellSpaces[i], currentCellSpaces[i - 1]);
                WallAxis nextWall = ControllCellPosition(currentCellSpaces[i], currentCellSpaces[i + 1]);
                switch (prevWall)
                {
                    case WallAxis.Nord:
                        if(nextWall == WallAxis.Ovest)
                        {
                            currentCellSpaces[i].destroyableWallAxis = WallAxis.NordOvest;
                        }
                        else
                        {
                            currentCellSpaces[i].destroyableWallAxis = WallAxis.EstNord;
                        }
                        break;
                    case WallAxis.Ovest:
                        if (nextWall == WallAxis.Sud)
                        {
                            currentCellSpaces[i].destroyableWallAxis = WallAxis.OvestSud;
                        }
                        else
                        {
                            currentCellSpaces[i].destroyableWallAxis = WallAxis.NordOvest;
                        }
                        break;
                    case WallAxis.Sud:
                        if (nextWall == WallAxis.Est)
                        {
                            currentCellSpaces[i].destroyableWallAxis = WallAxis.SudEst;
                        }
                        else
                        {
                            currentCellSpaces[i].destroyableWallAxis = WallAxis.OvestSud;
                        }
                        break;
                    case WallAxis.Est:
                        if (nextWall == WallAxis.Nord)
                        {
                            currentCellSpaces[i].destroyableWallAxis = WallAxis.EstNord;
                        }
                        else
                        {
                            currentCellSpaces[i].destroyableWallAxis = WallAxis.SudEst;
                        }
                        break;
                }
            }
        }
    }

    private WallAxis ControllCellPosition(CellSpace currentCell, CellSpace aboveCell)
    {
        switch (GetDirection(currentCell, aboveCell))
        {
            case Direction.Horizontal:
                if (currentCell.worldPos.x < aboveCell.worldPos.x)
                {
                    return WallAxis.Nord;
                }
                if (currentCell.worldPos.x > aboveCell.worldPos.x)
                {
                    return WallAxis.Sud;
                }
                break;
            default:
                if (currentCell.worldPos.z > aboveCell.worldPos.z)
                {
                    return WallAxis.Ovest;
                }
                if (currentCell.worldPos.z < aboveCell.worldPos.z)
                {
                    return WallAxis.Est;
                }
                break;
        }
        return WallAxis.Nord;
    }

    //Get Direction based on previousCellsSelected
    private Direction GetDirection(CellSpace firstCell, CellSpace secondCell)
    {
        if (firstCell.selectionZ == secondCell.selectionZ)
        {
            return Direction.Horizontal;
        }
        else
        {
            return Direction.Vertical;
        }
    }

    //Controll All neighbor of PrevCellSelected if Not Visisted
    private List<CellSpace> GetAvaliableNeighbordhood(int cellBoardX, int cellBoardZ)
    {
        List<CellSpace> avaliables = new List<CellSpace>();
        foreach (CellSpace neighbor in board[cellBoardX, cellBoardZ].neighbors)
        {
            if (!neighbor.visited)
            {
                avaliables.Add(neighbor);
            }
        }
        return avaliables;
    }

    //Get Cell Based On Quod Selection
    private CellSpace QuodSelectionBase()
    {
        List<CellSpace> avaliableNeighbor = GetAvaliableNeighbordhood(cellSelectionX, cellSelectionZ);

        if (avaliableNeighbor.Count > 0)
        {
            if (cellUsed.Count > 0)
            {
                int randomCellSelection = TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(avaliableNeighbor.Count);
                return avaliableNeighbor[randomCellSelection];
            }
            else return board[50, 50];
        }
        else
        {
            return null;
        }
    }

    //Get Cell Based On Rectangle Selection
    private CellSpace RectSelectionBase(CellSpace prevCellSelected)
    {
        List<CellSpace> avaliableNeighbor = GetAvaliableNeighbordhood(prevCellSelected.selectionX, prevCellSelected.selectionZ);

        if (avaliableNeighbor.Count > 0)
        {
            int randomCellSelection = TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(avaliableNeighbor.Count);
            return avaliableNeighbor[randomCellSelection];
        }
        else
        {
            return null;
        }
    }

    //Get Cell Based On L Selection
    private CellSpace LSelectionBase(CellSpace prevCellSelected, Direction currentDirection)
    {
        List<CellSpace> avaliableNeighbor = GetAvaliableNeighbordhood(prevCellSelected.selectionX, prevCellSelected.selectionZ);

        List<CellSpace> axisAvaliable = new List<CellSpace>();

        //Controll Direction of prevCellSelected for storing right cell
        if (avaliableNeighbor.Count > 0)
        {
            switch (currentDirection)
            {
                case Direction.Horizontal:
                    foreach (CellSpace neighbor in avaliableNeighbor)
                    {
                        if (neighbor.selectionX == prevCellSelected.selectionX)
                        {
                            axisAvaliable.Add(neighbor);
                        }
                    }
                    break;
                case Direction.Vertical:
                    foreach (CellSpace neighbor in avaliableNeighbor)
                    {
                        if (neighbor.selectionZ == prevCellSelected.selectionZ)
                        {
                            axisAvaliable.Add(neighbor);
                        }
                    }
                    break;
            }
        }

        if (axisAvaliable.Count > 0)
        {
            int randomCellSelection = TrueRandomInteger.TrueRandomInteger.GetRandomIntgerForList(axisAvaliable.Count);
            return axisAvaliable[randomCellSelection];
        }
        else
        {
            return null;
        }
    }

    public void CreateLevel(bool isEditor)
    {
        StartCells(isEditor);
        UIManager.Instance.FadeSetVisibility(false);
    }

    public void DestroyLevel(bool isEditor)
    {
        cellUsed = new List<CellSpace>();
        foreach(FloorBehaviour floorBehaviour in floorBehaviours)
        {
            DestroyImmediate(floorBehaviour.gameObject);
        }
        foreach(GameObject obstacle in obstacles)
        {
            DestroyImmediate(obstacle);
        }
        HQManager.Instance.DestoryFactory();
        floorBehaviours = new List<FloorBehaviour>();
        obstacles = new List<GameObject>();
        cellSelectionX = 50;
        cellSelectionZ = 50;
        GameStatus.Instance.ClearEnemies();
        GameStatus.Instance.SetPositionPlayer(new Vector3(0,0,0));
        UIManager.Instance.FadeSetVisibility(false);
    }
}

public enum Direction
{
    Horizontal = 0,
    Vertical = 1
}

public enum WallAxis
{
    Nord = 0,
    Ovest = 1,
    Sud = 2,
    Est = 3,
    NordOvest = 4,
    OvestSud = 5,
    SudEst = 6,
    EstNord = 7
}