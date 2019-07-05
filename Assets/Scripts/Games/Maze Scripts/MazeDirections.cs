using UnityEngine;
using System.Collections;



public enum MazeDirection
{
    North,
    East,
    South,
    West
}


public static class MazeDirections
{

    public const int Count = 4;

    public static MazeDirection RandomValue
    {
        get
        {
            return (MazeDirection)Random.Range(0, Count);
        }
    }

    static Vector2Int[] vectors =
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };

    static Quaternion[] rotations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 90f, 0f),
        Quaternion.Euler(0f, 180f, 0f),
        Quaternion.Euler(0f, 270f, 0f)
    };

    static MazeDirection[] opposites = {
        MazeDirection.South,
        MazeDirection.West,
        MazeDirection.North,
        MazeDirection.East
    };

    public static Vector2Int ToIntVector2(this MazeDirection direction)
    {
        return vectors[(int)direction];
    }          

    public static MazeDirection GetOpposite(this MazeDirection direction)
    {
        return opposites[(int)direction];
    }
    
    public static Quaternion ToRotation(this MazeDirection direction)
    {
        return rotations[(int)direction];
    }

    public static bool isBetween(this int value, int min, int max)
    {
        return value > min && value < max;
       
    }


    public static Vector3Int toVector2Int(this Vector3 vec3)
    {
        int x = Mathf.RoundToInt(vec3.x);
        int y = Mathf.RoundToInt(vec3.y);
        int z = Mathf.RoundToInt(vec3.z);

        return new Vector3Int(x,y,z);
    }

    public static int floorToInt(this float value)
    {
        return Mathf.FloorToInt(value);
    }
    
}


