using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(TileMap))]
public class TileMapMouse : MonoBehaviour {


    public Buildable whatToBuild;

    private TileMap tileMap;
    private Vector3 currentPoint = Vector3.zero;
    private Vector3 pressedPoint = Vector3.zero;
    private Vector3 lastPoint = Vector3.zero;

    private int currentTypeIndex = 0;
    private int currentValueIndex = 0;
    private List<List<Type>> buildableTypes = new List<List<Type>>
    {
        new List<Type>
        {
            typeof(BReceptionRoom),
            typeof(BGeneralRoom),
            typeof(BDoubleBedroom),
            typeof(BSharedBedroom),
            typeof(BHouseKeepingRoom),
        },

        new List<Type>
        {
            typeof(BuildableTile)
        },

        new List<Type>
        {
            typeof(BuildableStaff)
        },
    };

    private GameData data;
    private SelectionTile selectionScript;
    void Start()
    {
        tileMap = GetComponent<TileMap>();
        whatToBuild = (Buildable)ScriptableObject.CreateInstance(buildableTypes[currentTypeIndex][currentValueIndex].ToString());
        data = FindObjectOfType<GameData>();
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

        Buildable.MouseButton down = Input.GetMouseButtonDown(0) ? Buildable.MouseButton.Left : Input.GetMouseButtonDown(2) ? Buildable.MouseButton.Middle : Buildable.MouseButton.Unknown;
        Buildable.MouseButton up = Input.GetMouseButtonUp(0) ? Buildable.MouseButton.Left : Input.GetMouseButtonUp(2) ? Buildable.MouseButton.Middle : Buildable.MouseButton.Unknown;

        // BUILD THE STUFF!
        if (down != Buildable.MouseButton.Unknown)
        {
            // Pressed mouse
            pressedPoint = currentPoint;
            whatToBuild.pressMouse(clone(pressedPoint), down);
        }
        else if (up != Buildable.MouseButton.Unknown)
        {
            // Release mouse
            whatToBuild.releaseMouse(clone(pressedPoint), clone(currentPoint), up);
            pressedPoint = Vector3.zero;
        }
        else if (pressedPoint != Vector3.zero)
        {
            // Drag Mouse
            if (lastPoint != currentPoint) {
                whatToBuild.dragMouse(clone(pressedPoint), clone(currentPoint));
            }
        }
        else if(lastPoint != currentPoint)
        {
            // Move Mouse
            whatToBuild.moveMouse(clone(currentPoint));
        }
        lastPoint = currentPoint;


        if( Input.GetButtonDown("Submit"))
        {
            //Debug.Log("Submit");
            if( !whatToBuild.hasNextStage())
            {
                if (whatToBuild is BuildableRoom)
                {
                    // Record this room as being constructed
                    data.dTileMap.RecordRoom();
                }
                whatToBuild = (Buildable)ScriptableObject.CreateInstance(buildableTypes[currentTypeIndex][currentValueIndex].ToString());
                //Debug.Log(whatToBuild);
                selectionScript.setBuilder(whatToBuild);
            }
            whatToBuild.applyStage();
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            //Debug.Log("Cancel");
            whatToBuild.cancelStage();
            //whatToBuild = (Buildable)ScriptableObject.CreateInstance(buildableTypes[currentTypeIndex][currentValueIndex].ToString());
            //selectionScript.setBuilder(whatToBuild);
        }
        else if (Input.GetButtonDown("SwitchValue"))
        {
            
            // If we havn't started buildign a room yet. Change type of room
            if ( whatToBuild is BuildableRoom)
            {
                BuildableRoom bRoom = (BuildableRoom)whatToBuild;
                if (bRoom.getStage() == BuildableRoom.STAGE_BLUEPRINT && bRoom.getProperty() == BuildableRoom.PROPERTY_BP_CREATE)
                {

                    currentValueIndex++;
                    if(currentValueIndex >= buildableTypes[currentTypeIndex].Count)
                    {
                        currentValueIndex = 0;
                    }
                    whatToBuild = (Buildable)ScriptableObject.CreateInstance(buildableTypes[currentTypeIndex][currentValueIndex].ToString());
                    selectionScript.setBuilder(whatToBuild);
                }
                else
                {
                    // Can't change room type. Change items instead if we can
                    whatToBuild.switchValue();
                }
            }
            else if (whatToBuild is BuildableTile)
            {
                BuildableTile bTile = (BuildableTile)whatToBuild;
                bTile.setMaxValue(data.dTileMap.maxTileTypes()-1);
                bTile.switchValue();
            }
            else
            {
                Debug.Log("Unknown Type to switch value");
            }
        }
        else if(Input.GetButtonDown("SwitchType"))
        {
            // Make sure we are allowed to switch the type
            if (whatToBuild is BuildableRoom)
            {
                BuildableRoom bRoom = (BuildableRoom)whatToBuild;
                if (bRoom.getStage() != BuildableRoom.STAGE_BLUEPRINT && bRoom.getProperty() != BuildableRoom.PROPERTY_BP_CREATE)
                {
                    // Already started building a room.
                    return;
                }
            }

            currentValueIndex = 0;
            currentTypeIndex++;
            if (currentTypeIndex >= buildableTypes.Count)
            {
                currentTypeIndex = 0;
            }
            whatToBuild = (Buildable)ScriptableObject.CreateInstance(buildableTypes[currentTypeIndex][currentValueIndex].ToString());
            selectionScript.setBuilder(whatToBuild);
            Debug.Log(whatToBuild.GetType().Name);
        }
    }

    Vector3 clone(Vector3 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

    void OnEnable()
    {
        whatToBuild = (Buildable)ScriptableObject.CreateInstance(buildableTypes[currentTypeIndex][currentValueIndex].ToString());
    }
}
