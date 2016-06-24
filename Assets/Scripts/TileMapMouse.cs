using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TileMap))]
public class TileMapMouse : MonoBehaviour {
    //public GameObject selectionCube;
    public Buildable whatToBuild;

    private TileMap tileMap;
    private Vector3 currentPoint = Vector3.zero;
    private Vector3 pressedPoint = Vector3.zero;
    private Vector3 lastPoint = Vector3.zero;

    private int currentRoomIndex = 0;
    private List<BuildableRoom> buildableRooms = new List<BuildableRoom>
    {
        new BReceptionRoom(0, 0, 0, 0),
        new BGeneralRoom(0, 0, 0, 0),
        new BDoubleBedroom(0, 0, 0, 0)
    };

    private SelectionTile selectionScript;
    void Start()
    {
        tileMap = GetComponent<TileMap>();
        whatToBuild = buildableRooms[currentRoomIndex];
        selectionScript = FindObjectOfType<SelectionTile>();
        selectionScript.setBuilder(whatToBuild);
    }

    // Update is called once per frame
    void Update () {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        Collider coll = GetComponent<Collider>();
        //Renderer rend = GetComponent<Renderer>();
        if (coll.Raycast(ray, out hit, Mathf.Infinity)){

            // Save the mosues current position
            int x = Mathf.FloorToInt(hit.point.x / tileMap.tileSize);
            int y = Mathf.FloorToInt(hit.point.z / tileMap.tileSize);
            currentPoint = new Vector3(x, 0, y);
        }


        // Don't build if we can't!
        if ( whatToBuild == null )
        {
            return;
        }


        // BUILD THE STUFF!
        if (Input.GetMouseButtonDown(0))
        {
            // Pressed mouse
            pressedPoint = currentPoint;
            whatToBuild.pressMouse(pressedPoint);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Release mouse
            whatToBuild.releaseMouse(pressedPoint, currentPoint);
            pressedPoint = Vector3.zero;
        }
        else if (pressedPoint != Vector3.zero)
        {
            // Drag Mouse
            if (lastPoint != currentPoint) {
                whatToBuild.dragMouse(pressedPoint, currentPoint);
            }
        }
        else if(lastPoint != currentPoint)
        {
            // Move Mouse
            whatToBuild.moveMouse(currentPoint);
            //selectionCube.transform.position = currentPoint;
        }
        lastPoint = currentPoint;


        if( Input.GetButtonDown("Submit"))
        {
            Debug.Log("Submit");
            if( !whatToBuild.hasNextStage())
            {
                whatToBuild = new BDoubleBedroom(0, 0, 0, 0);
                selectionScript.setBuilder(whatToBuild);
            }
            whatToBuild.applyStage();
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            Debug.Log("Cancel");
            whatToBuild = new BDoubleBedroom(0, 0, 0, 0);
            selectionScript.setBuilder(whatToBuild);
        }
        else if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jump");
            // If we havn't started buildign a room yet. Change type of room
            if ( whatToBuild is BuildableRoom)
            {
                BuildableRoom bRoom = (BuildableRoom)whatToBuild;
                if (bRoom.getStage() == BuildableRoom.STAGE_BLUEPRINT && bRoom.getProperty() == BuildableRoom.PROPERTY_BP_CREATE)
                {

                    currentRoomIndex++;
                    if(currentRoomIndex >= buildableRooms.Count)
                    {
                        currentRoomIndex = 0;
                    }
                    whatToBuild = buildableRooms[currentRoomIndex];
                    selectionScript.setBuilder(whatToBuild);
                    Debug.Log(whatToBuild.GetType().Name);
                }
                else
                {
                    Debug.Log("C");
                    // Can't change room type. Change items instead if we can
                    whatToBuild.switchValue();
                }
            }
            else
            {
                Debug.Log("Not Room");
            }
        }
    }
}
