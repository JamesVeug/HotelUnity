using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class TileMap : MonoBehaviour
{

    protected class TileMapImage{
        public Color[] pixels;
        public int width;
        public int height;
    }
    

    public int width = 50;
    public int height = 50;
    public float tileSize = 1.0f;
    public float resolution = 8f;
    public Material[] materials;
    public GameObject wallPrefab;
    public GameObject doorPrefab;
    public GameObject windowPrefab;
    public GameObject[] itemObjects;
    public BuildableItem[] itemScripts = {
        new BuildableMinibar(0,0),
        new BuildableBed(0,0)
    };


    public GameObject blueprintWallPrefab;
    public GameObject blueprintDoorPrefab;
    public GameObject blueprintWindowPrefab;

    private Material[] currentMaterials;
    private TileMapImage[] imageMaps;
    private int texWidth;
    private int texHeight;
    // Create map copy
    // When we add buildings only change the pixels of the changed tiles

    // Built map
    private DTileMap map;
    private SelectionTile selectionScript;

    // Use this for initialization
    void Start()
    {
        BuildMesh();
        selectionScript = FindObjectOfType<SelectionTile>();
    }

    void Update()
    {
        if( map.changes == null)
        {
            Debug.LogError("Map.Changes is null");
        }
        else if (map.changes.Count > 0)
        {
            MeshRenderer rend = GetComponent<MeshRenderer>();

            // Every change in the game
            foreach (Vector2 c in map.changes)
            {
                int x = (int)c.x;
                int y = (int)c.y;
                int index = x * height + y;

                // Place every tile as a 
                /*var m = new Material[rend.materials.Length];
                for(int i = 0; i < m.Length; i++)
                {
                    m[i] = materials[1];
                }
                rend.materials = m;*/
                int tile = map.getTile(x, y);
                currentMaterials[index] = materials[tile];
                rend.materials = currentMaterials;


                if ( map.isWall(x, y) )
                {
                    DWall wall = map.getWall(x, y);
                    buildWall(wall);
                }
                else if (map.isDoor(x, y))
                {
                    Debug.Log("BuildDoor");
                    DDoor door = map.getDoor(x, y);
                    buildDoor(door);
                }
                else if (map.isWindow(x, y))
                {
                    DWindow window = map.getWindow(x, y);
                    buildWindow(window);
                }
                else if (map.isItem(x, y))
                {
                    BuildableItem item = map.getItem(x, y);
                    buildItem(item);
                }
                //texture.filterMode = FilterMode.Bilinear;
            }
            map.changes.Clear();
        }

        // STYLE Selection box
        if (selectionScript.isModified())
        {
            if (selectionScript.getBuildable() is BuildableRoom) {
                BuildableRoom build = (BuildableRoom)selectionScript.getBuildable();
                int stage = build.getStage();
                string property = build.getProperty();

                // Selection cube has changed. We need to style it
                if (stage == BuildableRoom.STAGE_BLUEPRINT )
                {
                    Transform selectionCube = selectionScript.transform.GetChild(0);
                    if (property == BuildableRoom.PROPERTY_BP_CREATE)
                    {
                        //selectionCube.localScale = new Vector3(1, 0.2f, 1);
                        //selectionCube.position = selectionCube.localScale/2;
                    }
                    else if( property == BuildableRoom.PROPERTY_BP_RESIZE)
                    {
                        //selectionCube.localScale = new Vector3(1, 2.5f, 1);
                        //selectionCube.position += new Vector3(0, selectionCube.localScale.y / 2, 0);
                    }
                    else if (property == BuildableRoom.PROPERTY_BP_MOVE)
                    {
                        //Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
                    }
                }
                else if(stage == BuildableRoom.STAGE_WINDOWSDOORS)
                {
                    Transform selectionCube = selectionScript.transform;

                    // If we just changed to this Stage. Build the meshes
                    if (selectionScript.stageModified())
                    {
                        // We are now changing the windows and doors
                        // Show each wall
                        
                        // Create new walls
                        Vector3 position = selectionScript.rect.position;
                        for (int x = 0; x < selectionScript.rect.width; x++)
                        {
                            for (int y = 0; y <= 1; y++)
                            {
                                Vector3 newPos = new Vector3(x, 0, y * (selectionScript.rect.height - 1));
                                GameObject bpWall = (GameObject)Instantiate(blueprintWallPrefab);
                                bpWall.transform.position = position + newPos;
                                bpWall.transform.parent = selectionCube;
                                bpWall.name = "BPWall" + newPos.x + "," + newPos.z;
                            }
                        }
                        for (int y = 0; y < selectionScript.rect.height; y++)
                        {
                            for (int x = 0; x <= 1; x++)
                            {
                                Vector3 newPos = new Vector3(x * (selectionScript.rect.width - 1), 0, y);
                                GameObject bpWall = (GameObject)Instantiate(blueprintWallPrefab);
                                bpWall.transform.position = position + newPos;
                                bpWall.transform.parent = selectionCube;
                                bpWall.name = "BPWall" + newPos.x + "," + newPos.z;
                            }
                        }
                    }

                    // Check if the door positions
                    if (selectionScript.doorsModified())
                    {
                        Debug.Log("DOORS CHANGED");
                        // Get the position
                        foreach (DDoor door in build.doors)
                        {
                            Vector3 newPos = new Vector3(door.position.x, 0, door.position.y);
                            Vector3 roomPos = newPos - selectionScript.rect.position;

                            // Remove anything that was originally there
                            foreach (Transform child in selectionCube)
                            {
                                if (child.name.EndsWith(roomPos.x + "," + roomPos.z))
                                {
                                    Destroy(child.gameObject);
                                    break;
                                }
                            }


                            // Add the door
                            GameObject bpDoor = (GameObject)Instantiate(blueprintDoorPrefab);
                            bpDoor.transform.position = newPos;
                            bpDoor.transform.parent = selectionCube;
                            bpDoor.name = "BPDoor" + newPos.x + "," + newPos.z;
                            if (roomPos.x == 0 || roomPos.x == selectionScript.rect.width - 1)
                            {
                                bpDoor.transform.GetChild(0).localRotation = Quaternion.Euler(0, 90, 0);
                            }
                        }
                    }

                    // Get the position
                    if (selectionScript.windowsModified())
                    {
                        foreach (DWindow window in build.windows)
                        {
                            Vector3 newPos = new Vector3(window.position.x, 0, window.position.y);
                            Vector3 roomPos = newPos - selectionScript.rect.position;

                            // Remove anything that was originally there
                            foreach (Transform child in selectionCube)
                            {
                                if (child.name.EndsWith(roomPos.x + "," + roomPos.z))
                                {
                                    Destroy(child.gameObject);
                                    break;
                                }
                            }


                            // Add the Window
                            GameObject bpDoor = (GameObject)Instantiate(blueprintWindowPrefab);
                            bpDoor.transform.position = newPos;
                            bpDoor.transform.parent = selectionCube;
                            bpDoor.name = "BPWindow" + newPos.x + "," + newPos.z;
                            if (roomPos.x == 0 || roomPos.x == selectionScript.rect.width - 1)
                            {
                                bpDoor.transform.GetChild(0).localRotation = Quaternion.Euler(0, 90, 0);
                            }
                        }
                    }
                }
                else if(stage == BuildableRoom.STAGE_ITEMS)
                {
                    if (selectionScript.stageModified())
                    {
                        Transform selectionCube = selectionScript.transform;
                        while (selectionCube.childCount > 1)
                        {
                            Transform child = selectionScript.transform.GetChild(1);
                            if (child != selectionScript.tileObject)
                            {
                                child.parent = null;
                                Destroy(child.gameObject);
                            }
                        }
                    }
                    else if( selectionScript.placedItemsModified() )
                    {
                        Transform selectionCube = selectionScript.transform;

                        // Get the position
                        foreach (BuildableItem position in build.items)
                        {
                            Vector3 newPos = new Vector3(position.left, 0, position.top);

                            // Remove anything that was originally there
                            foreach (Transform child in selectionCube)
                            {
                                if (child.name.EndsWith(newPos.x + "," + newPos.z))
                                {
                                    Destroy(child.gameObject);
                                    break;
                                }
                            }

                            // Add the door
                            /*GameObject item = (GameObject)Instantiate(minibarPrefab);
                            item.transform.position = selectionScript.rect.position;
                            item.transform.GetChild(0).rotation = position.rotation;
                            item.name = "BuildableItem" + item.transform.position.x + "," + item.transform.position.z;
                            //map.AddItem(position);

                            BuildableRoom room = map.getRoom((int)selectionScript.rect.position.x, (int)selectionScript.rect.position.z);
                            if( room != null)
                            {
                                //item.transform.parent = room.transform;
                            }
                            else
                            {
                                Debug.Log("Doesn't exist " + "Room'" + selectionScript.rect.position.x + "," + selectionScript.rect.position.z + "'");
                            }*/
                        }
                    }
                }
            }
        }
    }

    public void buildTexture(Mesh mesh)
    {

        MeshRenderer rend = GetComponent<MeshRenderer>();
        int numTiles = width * height;
        int numTriangles = numTiles * 2;

        currentMaterials = new Material[numTriangles];
        for (int i = 0; i < numTriangles; i++)
        {
            currentMaterials[i] = materials[0];
        }
        rend.materials = currentMaterials;

    }


    public void BuildMesh()
    {
        GameData data = FindObjectOfType<GameData>();
        data.graphicsMap = this;
        map = data.createTileMap(width, height);

        int numTiles = width * height;
        int numTriangles = numTiles * 2;

        // Vertex Calculation
        int vertexSize_x = width*2 + 1;
        int vertexSize_y = height*2 + 1;
        int numVerticies = numTriangles*6;

        // Generate mesh data
        Vector3[] verticies = new Vector3[numVerticies];
        List<int[]> triangles = new List<int[]>(numTriangles + 1);
        Vector3[] normals = new Vector3[verticies.Length];
        Vector2[] uvs = new Vector2[verticies.Length];

        float x = 0;
        float y = 0;
        for (int t = 0; t < numTriangles; t++)
        {

            int vIndex = t * 6;

            // Triangle 1
            verticies[vIndex + 0] = new Vector3(x, 0, y);
            verticies[vIndex + 1] = new Vector3(x, 0, y + tileSize);
            verticies[vIndex + 2] = new Vector3(x + tileSize, 0, y);
            normals[vIndex + 0 ] = Vector3.up;
            normals[vIndex + 1 ] = Vector3.up;
            normals[vIndex + 2 ] = Vector3.up;
            uvs[vIndex + 0] = new Vector2(0,0);
            uvs[vIndex + 1] = new Vector2(0, 1);
            uvs[vIndex + 2] = new Vector2(1, 0);

            // Triangle 2
            verticies[vIndex + 3] = new Vector3(x + tileSize, 0, y);
            verticies[vIndex + 4] = new Vector3(x, 0, y + tileSize);
            verticies[vIndex + 5] = new Vector3(x + tileSize, 0, y + tileSize);
            normals[vIndex + 3] = Vector3.up;
            normals[vIndex + 4] = Vector3.up;
            normals[vIndex + 5] = Vector3.up;
            uvs[vIndex + 3] = new Vector2(1, 0);
            uvs[vIndex + 4] = new Vector2(1, 1);
            uvs[vIndex + 5] = new Vector2(0, 1);

            int[] triangle = new int[6];
            triangle[0] = vIndex + 0;
            triangle[1] = vIndex + 1;
            triangle[2] = vIndex + 2;
            triangle[3] = vIndex + 3;
            triangle[4] = vIndex + 4;
            triangle[5] = vIndex + 5;
            

            triangles.Add(triangle);

            y += tileSize;
            if( y >= tileSize*width)
            {
                x += (int)tileSize;
                y = 0;
            }
        }


        // Create mesh and populate it
        Mesh mesh = new Mesh();
        mesh.vertices = verticies;
        mesh.subMeshCount = numTiles;
        for (int i = 0; i < numTiles; i++)
        {
            mesh.SetTriangles(triangles[i], i);
        }
        mesh.normals = normals;
        mesh.uv = uvs;

        // Assign out mesh to the object
        MeshFilter mesh_filter = GetComponent<MeshFilter>();
        MeshCollider mesh_collider = GetComponent<MeshCollider>();

        mesh_filter.mesh = mesh;
        mesh_collider.sharedMesh = mesh;

        buildTexture(mesh);
    }

    private void buildWall(DWall wall)
    {
        GameObject wallObject = (GameObject)Instantiate(wallPrefab);
        wallObject.transform.position = new Vector3(wall.position.x, 0, wall.position.y);
        //wallObject.transform.parent = roomObject.transform;
        wall.gameObject = wallObject;
    }

    private void buildWindow(DWindow window)
    {
        if (window == null)
        {
            Debug.LogError("Window is null!");
        }

        GameObject windowObject = (GameObject)Instantiate(windowPrefab);
        windowObject.transform.position = new Vector3(window.position.x, 0, window.position.y);
        //windowObject.transform.parent = roomObject.transform;
        window.gameObject = windowObject;

        // Rotate it
        BuildableRoom room = map.getRoom((int)window.position.x, (int)window.position.y);
        Vector3 doorPosition = windowObject.transform.position - room.position;
        if (doorPosition.x == 0 || doorPosition.x == room.width - 1)
        {
            windowObject.transform.GetChild(0).localRotation = Quaternion.Euler(0, 90, 0);
        }
    }

    private void buildDoor(DDoor door)
    {
        if( door == null)
        {
            Debug.LogError("Door is null!");
        }

        GameObject doorObject = (GameObject)Instantiate(doorPrefab);
        doorObject.transform.position = new Vector3((int)door.position.x, 0, (int)door.position.y);
        //doorObject.transform.parent = roomObject.transform;
        door.gameObject = doorObject;

        // Rotate it
        BuildableRoom room = map.getRoom((int)door.position.x, (int)door.position.y);
        Vector3 doorPosition = doorObject.transform.position - room.position;
        Debug.Log("Door position " + doorPosition);
        if( doorPosition.x == 0 || doorPosition.x == room.width-1)
        {
            doorObject.transform.GetChild(0).localRotation = Quaternion.Euler(0, 90, 0);
        }

        
    }

    private void buildItem(BuildableItem item)
    {
        if (item == null)
        {
            Debug.LogError("Item is null!");
        }

        string name = item.GetType().Name + item.position.x + "," + item.position.z;
        GameObject itemObject = GameObject.Find(name);
        if (itemObject == null)
        {
            for(int i = 0; i < itemScripts.Length;i++)
            {

                if( itemScripts[i].GetType().Equals(item.GetType()))
                {
                    itemObject = (GameObject)Instantiate(itemObjects[i]);
                    itemObject.name = name;
                    break;
                }
            }
            if( itemObject == null)
            {
                // No possible combinations
                Debug.LogError("Could not match type of " + item.GetType());
            }
        }
        itemObject.transform.position = new Vector3((int)item.position.x, 0, (int)item.position.z);
        itemObject.transform.GetChild(0).transform.rotation = item.rotation;
    }
}
