using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class Maze : MonoBehaviour
{

    public event Action OnGenerationComplete;

    public Vector2Int size;

    [Tooltip("Our floor prefabs size of 1x1 or 2x2 etc.")]
    public int sizeMultiplier = 8;
    

    //Prefabs
    public MazeCell cellPrefab;

    public GameObject columnPrefab;

    public MazePassage passagePrefab;

    public MazeWall wallPrefab;


    //Objects
    MazeCell[,] cells;//2D 

    List<GameObject> columnObjects;

    readonly List<Vector3Int> cornerPositions = new List<Vector3Int>();

    readonly List<Vector3> hallwayPositions = new List<Vector3>();

    readonly List<Vector3> innerCornersPositions = new List<Vector3>();

    readonly List<Vector3> deadEndPositions = new List<Vector3>();


    //Settings
    public bool showGizmos;

    public bool showAllCorners;

    public bool showInnerCorners;

    public bool showMazeBounds;



    void SpawnColumns()
    {
        columnObjects = new List<GameObject>();

        foreach (Vector3Int cornerPosition in cornerPositions)
        {
            GameObject column = Instantiate(columnPrefab);

            column.transform.position = new Vector3(cornerPosition.x, cornerPosition.y, cornerPosition.z);

            column.transform.SetParent(transform);

            columnObjects.Add(column);

            //randomize rotation ??
        }
    }


    MazeCell CreateCell(Vector2Int coordinates)
    {
        MazeCell newCell = Instantiate(cellPrefab) as MazeCell;

        cells[coordinates.x, coordinates.y] = newCell;

        newCell.coordinates = coordinates;

        newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.y;

        newCell.transform.parent = transform;

        newCell.transform.localPosition = new Vector3((coordinates.x - size.x * 0.5f + 0.5f) * sizeMultiplier, 0f, (coordinates.y - size.y * 0.5f + 0.5f) * sizeMultiplier);

        return newCell;

    }

    public IEnumerator GenerateMaze(float stepDelay)
    {
        WaitForSeconds delay = new WaitForSeconds(stepDelay);

        cells = new MazeCell[size.x, size.y];//2x2 or 1x1

        List<MazeCell> activeCells = new List<MazeCell>();

        DoFirstGenerationStep(activeCells);

        while (activeCells.Count > 0)
        {
            //delay only if we have a step time.

            if (stepDelay > 0)
            {
                yield return delay;
            }

            //yield return new WaitWhile(() => MazeCreator.CanStep(ref canStep));

            DoNextGenerationStep(activeCells);
        }

        yield return null;

        SpawnColumns();

        yield return null;

        if (OnGenerationComplete != null)
        {
            OnGenerationComplete();
        }

    }


    void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazePassage passage = Instantiate(passagePrefab) as MazePassage;
        passage.Initialize(cell, otherCell, direction);
        passage = Instantiate(passagePrefab) as MazePassage;
        passage.Initialize(otherCell, cell, direction.GetOpposite());
    }


    void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazeWall wall = Instantiate(wallPrefab);

        wall.Initialize(cell, otherCell, direction);

        if (otherCell != null)
        {
            wall = Instantiate(wallPrefab) as MazeWall;
            wall.Initialize(otherCell, cell, direction.GetOpposite());
        }

    }
    
    public Vector2Int RandomCoordinates
    {
        get
        {
            return new Vector2Int(Random.Range(0, size.x), Random.Range(0, size.y));
        }
    }

    public bool ContainsCoordinates(Vector2Int coordinate)
    {
        return coordinate.x >= 0 && coordinate.x < size.x && coordinate.y >= 0 && coordinate.y < size.y;
    }


    void DoFirstGenerationStep(List<MazeCell> activeCells)
    {
        activeCells.Add(CreateCell(RandomCoordinates));
    }

    void DoNextGenerationStep(List<MazeCell> activeCells)
    {
        int currentIndex = activeCells.Count - 1;//we always use the last cell on the list until we remove them all.

        MazeCell currentCell = activeCells[currentIndex];

        if (currentCell.IsFullyInitialized)
        {
            StoreCellPoints(currentCell);
            activeCells.RemoveAt(currentIndex);
            return;
        }

        MazeDirection direction = currentCell.RandomUninitializedDirection;

        Vector2Int coordinates = currentCell.coordinates + direction.ToIntVector2();

        if (ContainsCoordinates(coordinates))
        {
            MazeCell neighbor = GetCell(coordinates);
            if (neighbor == null)
            {
                neighbor = CreateCell(coordinates);
                CreatePassage(currentCell, neighbor, direction);
                activeCells.Add(neighbor);
            }
            else
            {
                CreateWall(currentCell, neighbor, direction);
            }
        }
        else
        {
            CreateWall(currentCell, null, direction);
        }
    }
    
    void StoreCellPoints(MazeCell cell)
    {
        var passages = cell.GetComponentsInChildren<MazePassage>();

        //dead end
        if (passages.Length == 1)
        {
            deadEndPositions.Add(cell.transform.position);
        }


        //corner or hallway
        if (passages.Length == 2)
        {
            Vector3 a = passages[0].transform.forward * sizeMultiplier / 2;
            Vector3 b = passages[1].transform.forward * sizeMultiplier / 2;

            int angle = (int)Vector3.Angle(a, b);

            //Corner passage
            if (angle.isBetween(88, 92))
            {
                Vector3 pos = new Vector3();


                pos.x = cell.transform.position.x + a.x + b.x;
                pos.y = cell.transform.position.y + a.y + b.y;
                pos.z = cell.transform.position.z + a.z + b.z;


                if (!cornerPositions.Contains(pos.toVector2Int()))
                {
                    cornerPositions.Add(pos.toVector2Int());

                }



                //inner corner
                a = -passages[0].transform.forward * sizeMultiplier / 2;
                b = -passages[1].transform.forward * sizeMultiplier / 2;
                pos.x = cell.transform.position.x + a.x + b.x;
                pos.y = cell.transform.position.y + a.y + b.y;
                pos.z = cell.transform.position.z + a.z + b.z;


                if (!innerCornersPositions.Contains(pos.toVector2Int()))
                {
                    innerCornersPositions.Add(pos.toVector2Int());
                }

            }
            //Hallway
            else
            {
                hallwayPositions.Add(cell.transform.position);
            }
        }


        //intersection T
        if (passages.Length == 3)
        {


            Vector3 a = passages[0].transform.forward * sizeMultiplier / 2; ;
            Vector3 b = passages[1].transform.forward * sizeMultiplier / 2; ;
            Vector3 c = passages[2].transform.forward * sizeMultiplier / 2; ;

            int angleA = Vector3.Angle(a, b).floorToInt();
            int angleB = Vector3.Angle(b, c).floorToInt();
            int angleC = Vector3.Angle(c, a).floorToInt();

            //print("Angle " + angleC);

            cell.angles.Add(angleA);
            cell.angles.Add(angleB);
            cell.angles.Add(angleC);


            if (angleA.isBetween(88, 92))
            {
                Vector3 pos = new Vector3();

                pos.x = cell.transform.position.x + a.x + b.x;
                pos.y = cell.transform.position.y + a.y + b.y;
                pos.z = cell.transform.position.z + a.z + b.z;


                if (!cornerPositions.Contains(pos.toVector2Int()))
                {
                    cornerPositions.Add(pos.toVector2Int());
                }

            }


            if (angleB.isBetween(88, 92))
            {
                Vector3 pos = new Vector3();

                pos.x = cell.transform.position.x + b.x + c.x;
                pos.y = cell.transform.position.y + b.y + c.y;
                pos.z = cell.transform.position.z + b.z + c.z;


                if (!cornerPositions.Contains(pos.toVector2Int()))
                {
                    cornerPositions.Add(pos.toVector2Int());
                }

            }

            if (angleC.isBetween(88, 92))
            {
                Vector3 pos = new Vector3();

                pos.x = cell.transform.position.x + c.x + a.x;
                pos.y = cell.transform.position.y + c.y + a.y;
                pos.z = cell.transform.position.z + c.z + a.z;


                if (!cornerPositions.Contains(pos.toVector2Int()))
                {
                    cornerPositions.Add(pos.toVector2Int());
                }

            }



        }


    }

    public MazeCell GetCell(Vector2Int coordinates)
    {
        return cells[coordinates.x, coordinates.y];
    }

    public void Destroy()
    {
        
        cornerPositions.Clear();
        innerCornersPositions.Clear();
        hallwayPositions.Clear();
        deadEndPositions.Clear();

        foreach (MazeCell mazeCell in cells)
        {
            Destroy(mazeCell.gameObject);
        }
        foreach (GameObject obj in columnObjects)
        {
            Destroy(obj);
        }
        
        columnObjects.Clear();


    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        if (showMazeBounds)
        {
            Gizmos.color = Color.blue;
            Vector3 center = new Vector3(0, 1, 0);
            Vector3 boxSize = new Vector3(size.x * sizeMultiplier, 2, size.y * sizeMultiplier);
            Gizmos.DrawWireCube(center, boxSize);
        }


        Vector3 columnSize = new Vector3(1, 3, 1);

        if (showAllCorners && cornerPositions.Count > 0)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < cornerPositions.Count; i++)
            {
                Gizmos.DrawCube(cornerPositions[i] + Vector3.up * columnSize.y / 2, columnSize);
            }

        }

        if (showInnerCorners && innerCornersPositions.Count > 0)
        {
            Gizmos.color = Color.yellow;
            columnSize.y *= 2;
            for (int i = 0; i < innerCornersPositions.Count; i++)
            {
                Gizmos.DrawWireCube(innerCornersPositions[i] + Vector3.up * columnSize.y / 2, columnSize);
            }

        }


        if (hallwayPositions.Count > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < hallwayPositions.Count; i++)
            {
                Gizmos.DrawSphere(hallwayPositions[i], .5f);
            }

        }

        if (deadEndPositions.Count > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < deadEndPositions.Count; i++)
            {
                Gizmos.DrawSphere(deadEndPositions[i], .5f);
            }

        }
    }


}