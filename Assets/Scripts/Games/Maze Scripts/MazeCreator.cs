using System.Collections;
using UnityEngine;

public class MazeCreator : MonoBehaviour
{
    public Maze maze;
    Vector2Int cellPosition;
    [SerializeField] readonly int RoomOpeningWidth = 8;
    [SerializeField] Transform startRoom;
    [SerializeField] Transform endRoom;
    [SerializeField] Material wallDissolveMaterial;
    [SerializeField] Material wallDefaultMaterial;
    [SerializeField] MeshRenderer[] walls;
    public bool canRebuild;
    public float stepDelay = .01f;


    IEnumerator Start()
    {
        yield return new WaitForSeconds(4);
        GenerateMaze();
    }

    void OnEnable()
    {
        maze.OnGenerationComplete += PlaceRooms;
    }

    void OnDisable()
    {
        maze.OnGenerationComplete -= PlaceRooms;
        wallDissolveMaterial.SetFloat("_DissolveAmount", 0);//insures inspector values are reset.
    }

    void GenerateMaze()
    {
        StartCoroutine(maze.GenerateMaze(stepDelay));
    }

    void Update()
    {
        if (canRebuild)
        {
            StartCoroutine(DissolveWallsReverse());
        }
    }


    public static bool CanStep(ref bool canStep)
    {
        if (canStep)
        {
            canStep = false;
            return false;
        }

        return true;
    }
    

    void Rebuild()
    {
        StopAllCoroutines();
        maze.Destroy();
        StartCoroutine(maze.GenerateMaze(stepDelay));
        canRebuild = false;
    }

    void PlaceRooms()
    {
        RemoveWalls(startRoom);
        RemoveWalls(endRoom);
        StartCoroutine(DissolveWalls());
    }

    void SetDissolveMaterials()
    {
        foreach (var wall in walls)
        {
            wall.material = wallDissolveMaterial;
        }
    }
    void SetDefaultMaterials()
    {
        foreach (var wall in walls)
        {
            wall.material = wallDefaultMaterial;
        }
    }
    void SetWallsActive(bool activeState)
    {
        foreach (var wall in walls)
        {
            wall.gameObject.SetActive(activeState);
        }
    }

    IEnumerator DissolveWalls()
    {
        SetWallsActive(true);
        SetDissolveMaterials();
        wallDissolveMaterial.SetFloat("_DissolveAmount", 0);

        float time = 0;
        while (time < 4)
        {
            time += Time.deltaTime;
            wallDissolveMaterial.SetFloat("_DissolveAmount",time / 4 );//this is 0-1 normalized
            yield return null;
        }

        SetWallsActive(false);


    }
     IEnumerator DissolveWallsReverse()
    {
        SetWallsActive(true);
        SetDissolveMaterials();
        wallDissolveMaterial.SetFloat("_DissolveAmount", 1);
        float time = 4;

        while (time > 0.001f)
        {
            time -= Time.deltaTime;
            wallDissolveMaterial.SetFloat("_DissolveAmount", time / 4 );//this is 0-1 normalized
            yield return null;
        }

        SetDefaultMaterials();
        Rebuild();

    }

    void RemoveWalls(Transform room)
    {
        RaycastHit hitInfo;

        for (var i = -1; i < 2; i++)
        {
            var offset = new Vector3(i * RoomOpeningWidth, 2, 0);

            Debug.DrawRay(room.position + offset, room.forward * 10, Color.green, 10);

            if (Physics.Raycast(room.position + offset, room.forward, out hitInfo, 10))
            {
                Destroy(hitInfo.transform.parent.gameObject);
            }
        }
    }
}