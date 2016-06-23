using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TileMap))]
public class TileMapMouse : MonoBehaviour {
    //public GameObject selectionCube;
    public Buildable whatToBuild;

    private TileMap tileMap;
    private Vector3 currentPoint = Vector3.zero;
    private Vector3 pressedPoint = Vector3.zero;
    private Vector3 lastPoint = Vector3.zero;


    private SelectionTile selectionScript;
    void Start()
    {
        tileMap = GetComponent<TileMap>();
        whatToBuild = new BuildableRoom(0, 0, 0, 0);
        selectionScript = FindObjectOfType<SelectionTile>();
        selectionScript.setBuilder(whatToBuild);
    }

    // Update is called once per frame
    void Update () {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        Collider coll = GetComponent<Collider>();
        Renderer rend = GetComponent<Renderer>();
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
                whatToBuild = new BuildableRoom(0, 0, 0, 0);
                selectionScript.setBuilder(whatToBuild);
            }
            whatToBuild.applyStage();
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            Debug.Log("Cancel");
            whatToBuild = new BuildableRoom(0, 0, 0, 0);
            selectionScript.setBuilder(whatToBuild);
        }
        else if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jump");
            whatToBuild.switchValue();
        }
    }
}
