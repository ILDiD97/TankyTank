using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cell
{
    public Vector3 worldPos;
    public FloorBehaviour floorBehaviour;
    public bool visited;
    public int angleType;
    public int selectionX;
    public int selectionZ;
    public List<Cell> neighbors = new List<Cell>();
    public int activeChild;
}

public class CellsGenerator : MonoBehaviour
{

    private List<FloorBehaviour> floorBehaviours;
    
    private Cell[,] board = new Cell[100, 100];
    
    private List<Cell> cellUsed;

    [SerializeField]
    private List<GameObject> floors;

    private int cellSelectionX = 50;
    private int cellSelectionZ = 50;

    [SerializeField]
    private int level;

    private void Awake()
    {
        //print(board.Length);
        StartCells();
    }

    public void StartCells()
    {
        cellUsed = new List<Cell>();
        floorBehaviours = new List<FloorBehaviour>();
        //Generate Board of cells
        BoardGenerator();
    }

    private void BoardGenerator()
    {
        for (int x = 0; x < 100; x++)
        {
            for (int z = 0; z < 100; z++)
            {

                //Generate Cell in Listo Board
                board[x, z] = new Cell();

                //Detailing Cell
                board[x, z].worldPos = new Vector3(50 * x, -0.5f, 50 * z);
                board[x, z].selectionX = x;
                board[x, z].selectionZ = z;

                //Add previous cell in vertical and on prevoius add this cell
                if (z != 0 && board[x , z - 1] != null)
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
        StartCoroutine(GenerateMaze());
    }

    private int GetRandomFloor()
    {
        int randomFloor;
        float randomRange = Random.Range(0, 99);
        if (randomRange < 33.0f)
        {
            randomFloor = 0;
        }
        else if(randomRange >= 33.0f && randomRange < 66.0f)
        {
            randomFloor = 1;
        }
        else
        {
            randomFloor = 2;
        }

        return randomFloor;
    }

    //Generate level on Corutine
    private IEnumerator GenerateMaze()
    {
        Cell currentCell;
        //Get first Random Floor Type and Get the current Cell
        int randomFloor = GetRandomFloor();
        Cell firstCell = GetCellPivot(randomFloor);
        GameObject floor;
        cellSelectionX = 50;
        cellSelectionZ = 50;

        int randomCellAvaliable;
        for (int x = 0; x < level; x++)
        {
            if(x == 0)
            {
                currentCell = firstCell;
            }
            else
            {
                randomFloor = GetRandomFloor();
                randomCellAvaliable = Random.Range(0, cellUsed.Count - 1);
                cellSelectionX = cellUsed[randomCellAvaliable].selectionX;
                cellSelectionZ = cellUsed[randomCellAvaliable].selectionZ;
                currentCell = GetCellPivot(randomFloor);
            }

            if(currentCell != null)
            {
                floor = Instantiate(floors[randomFloor], transform);
                floor.name = floor.name + "-" + x;
                floor.transform.position = currentCell.worldPos;
                floor.transform.Rotate(new Vector3(0, currentCell.angleType, 0), Space.World);
                FloorBehaviour floorBehaviour = floor.GetComponent<FloorBehaviour>();

                if (floorBehaviour) 
                {
                    SetFloor(floorBehaviour, randomFloor, currentCell.activeChild); 
                }

                //print("angle " + currentCell.angleType + " x " + currentCell.selectionX + " z " + currentCell.selectionZ);
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                x--;
            }
        }

        floorBehaviours[Random.Range(0, floorBehaviours.Count - 1)].SetPlayerStart();
        yield return new WaitForSeconds(6);

        UIManager.Instance.TimeFormStart();
        GameStatus.Instance.SetStartGame(false);
    }

    void SetFloor(FloorBehaviour behaviour, int floorType, int child)
    {

        floorBehaviours.Add(behaviour);
        
        if (floorType == 2)
        {
            //behaviour.DestroyChildIfThird(child);
            //behaviour.childActive = child;
        }

    }
    //Get Pivot Cell for Instantiate Next Floor Type
    private Cell GetCellPivot(int floorSelectedType)
    {
        Cell firstCell;
        Cell secondCell = null;
        Cell thirdCell = null;
        
        switch (floorSelectedType)
        {
            case 0:

                //Get cell
                firstCell = QuodSelectionBase();

                 //Choose if need to repeat selection of cell
                if(firstCell != null)
                {
                    cellUsed.Add(firstCell);
                    firstCell.visited = true;
                }
                return firstCell;
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
                    cellUsed.Add(firstCell);
                    cellUsed.Add(secondCell);
                    secondCell.visited = true;
                    //Debug.DrawRay(secondCell.worldPos, firstCell.worldPos, Color.blue,10);
                    //print("Second Cell Board Position " + secondCell.selectionX + " " + secondCell.selectionZ);
                    //print("First Cell Board Position " + firstCell.selectionX + " " + firstCell.selectionZ);
                    secondCell.angleType = GetAngleForRect(
                        GetDirection(firstCell, secondCell), 
                        firstCell, secondCell);
                    
                }
                else
                {
                    firstCell.visited = false;
                }
                return secondCell;
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

                DirectionAxis directionAxis ;
                //Controll if Previous Cell Is Not Visited
                if (secondCell !=null)
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
                if(thirdCell != null)
                {
                    cellUsed.Add(firstCell);
                    cellUsed.Add(secondCell);
                    cellUsed.Add(thirdCell);
                    thirdCell.visited = true;
                    //Debug.DrawRay(thirdCell.worldPos, firstCell.worldPos, Color.blue,100);
                    // print("Third Cell Board Position " + thirdCell.selectionX + " " + thirdCell.selectionZ);
                    // print("Second Cell Board Position " + secondCell.selectionX + " " + secondCell.selectionZ);
                    // print("First Cell Board Position " + firstCell.selectionX + " " + firstCell.selectionZ);
                    thirdCell.angleType = GetAngleForL(directionAxis, firstCell, secondCell, thirdCell);
                }
                else
                {
                    firstCell.visited = false;
                    secondCell.visited = false;
                }
                return thirdCell;
            default:
                return null;
        }
        return null;
    }

    //Calculate angle in case of Rect selection
    private int GetAngleForRect(DirectionAxis directionAxis, Cell prevCell, Cell currentCell)
    {
        int angle;
        switch (directionAxis)
        {
            case DirectionAxis.Horizontal:
                if (currentCell.selectionX > prevCell.selectionX) angle = 90;
                else angle = 270;
                return angle;
            default:
                if (currentCell.selectionZ > prevCell.selectionZ) angle = 0;
                else angle = 180;
                return angle;
        }
    }

    //Calculate angle in case of L Selection
    private int GetAngleForL(DirectionAxis directionAxis, Cell firstSelected, Cell secondSelected, Cell thirdSelected)
    {
        int angle;
        switch (directionAxis)
        {
            case DirectionAxis.Horizontal:
                if (thirdSelected.selectionZ < firstSelected.selectionZ)
                {
                    angle = 180;
                    thirdSelected.activeChild = thirdSelected.selectionX < firstSelected.selectionX? 1 : 0;
                }
                else
                {
                    angle = 0;
                    thirdSelected.activeChild = thirdSelected.selectionX < firstSelected.selectionX ? 0 : 1;
                }
                break;
            default:
                if (thirdSelected.selectionX < firstSelected.selectionX)
                {
                    angle = 270;
                    thirdSelected.activeChild = thirdSelected.selectionZ < firstSelected.selectionZ ? 0 : 1;
                }
                else
                {
                    angle = 90;
                    thirdSelected.activeChild = thirdSelected.selectionZ < firstSelected.selectionZ ? 1 : 0;
                }
                break;
        }
        return angle;
    }

    //Get Direction based on previousCellsSelected
    private DirectionAxis GetDirection(Cell firstCell, Cell secondCell)
    {
        if(firstCell.selectionZ == secondCell.selectionZ)
        {
            return DirectionAxis.Horizontal;
        }
        else
        {
            return DirectionAxis.Vertical;
        }
    }

    //Controll All neighbor of PrevCellSelected if Not Visisted
    private List<Cell> GetAvaliableNeighbordhood(int cellBoardX, int cellBoardZ)
    {
        List<Cell> avaliables = new List<Cell>();
        foreach (Cell neighbor in board[cellBoardX, cellBoardZ].neighbors)
        {
            if (!neighbor.visited)
            {
                avaliables.Add(neighbor);
            }
        }
        return avaliables;
    }

    //Get Cell Based On Quod Selection
    private Cell QuodSelectionBase()
    {
        List<Cell> avaliableNeighbor = GetAvaliableNeighbordhood(cellSelectionX, cellSelectionZ);

        if(avaliableNeighbor.Count > 0)
        {
            int randomCellSelection = Random.Range(0, avaliableNeighbor.Count - 1);
            return avaliableNeighbor[randomCellSelection];
        }
        else
        {
            return null;
        }
    }

    //Get Cell Based On Rectangle Selection
    private Cell RectSelectionBase(Cell prevCellSelected)
    {
        List<Cell> avaliableNeighbor = GetAvaliableNeighbordhood(prevCellSelected.selectionX, prevCellSelected.selectionZ);

        if (avaliableNeighbor.Count > 0)
        {
            int randomCellSelection = Random.Range(0, avaliableNeighbor.Count - 1);
            return avaliableNeighbor[randomCellSelection];
        }
        else
        {
            return null;
        }
    }

    //Get Cell Based On L Selection
    private Cell LSelectionBase(Cell prevCellSelected, DirectionAxis currentDirection)
    {
        List<Cell> avaliableNeighbor = GetAvaliableNeighbordhood(prevCellSelected.selectionX, prevCellSelected.selectionZ);

        List<Cell> axisAvaliable = new List<Cell>();
        //Controll Direction of prevCellSelected for storing right cell
        if(avaliableNeighbor.Count > 0)
        {
            switch (currentDirection)
            {
                case DirectionAxis.Horizontal:
                    foreach(Cell neighbor in avaliableNeighbor)
                    {
                        if(neighbor.selectionX == prevCellSelected.selectionX)
                        {
                            axisAvaliable.Add(neighbor);
                        }
                    }
                    break;
                case DirectionAxis.Vertical:
                    foreach (Cell neighbor in avaliableNeighbor)
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
            int randomCellSelection = Random.Range(0, axisAvaliable.Count - 1);
            return axisAvaliable[randomCellSelection];
        }
        else
        {
            return null;
        }
    }
}

public enum DirectionAxis
{
    Horizontal = 0,
    Vertical = 1
}
