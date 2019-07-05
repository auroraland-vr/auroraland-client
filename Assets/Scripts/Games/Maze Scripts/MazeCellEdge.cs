using UnityEngine;
using System.Collections;

public abstract class MazeCellEdge : MonoBehaviour
{

    public MazeCell cell;
    public MazeCell otherCell;
    public MazeDirection direction;



    public void Initialize(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        this.cell = cell;
        this.otherCell = otherCell;
        this.direction = direction;

        cell.SetEdge(direction, this);
        transform.parent = cell.transform;
        // HACK Added to support Will's new middle-center-pivoted walls. Need to remove once the pivot is center-bottom.
        transform.localPosition = Vector3.zero;
        transform.localRotation = direction.ToRotation();

    }


}
