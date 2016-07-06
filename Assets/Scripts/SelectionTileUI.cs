using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectionTileUI : MonoBehaviour {
    public SelectionTile selectionScript;
    public Text roomText;
    public Text stageText;
    public Text propertyText;
    public Text itemText;
    public Text mapText;
    public Text guestInfo;
    public Image currentMouseImage;
    public Sprite rotateImage;
    public Sprite IconImage;

    private GameData data;


    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    } 

    // Use this for initialization
    void Start () {
        data = FindObjectOfType<GameData>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        Buildable buildable = selectionScript.getBuildable();
        roomText.text =     "" + buildable.GetType().Name;
        stageText.text =    "" + buildable.getStage();
        propertyText.text = "" + buildable.getProperty();
        itemText.text =     "";
        if (buildable is BuildableRoom)
        {
            itemText.text += ((BuildableRoom)buildable).getCurrentBuildingItem();
        }
        else if (buildable is BuildableTile)
        {
            itemText.text += "Something of a tile";
        }

        // Map Text
        string text = "";
        for (int y = data.dTileMap.height-1; y >= 0; y--)
        {
            for(int x = 0; x < data.dTileMap.width; x++)
            {
                text += data.dTileMap.getTileType(x, y);
            }
            text += "\n";
        }

        mapText.text = text;

        // Guest Info
        GuestAI[] ais = FindObjectsOfType<GuestAI>();
        guestInfo.text = "Guests: " + ais.Length;

        foreach(GuestAI ai in ais)
        {
            Order o = ai.getCurrentOrder();
            string orderText = o != null ? o.ToString() : "Idle";
            guestInfo.text += orderText + "\n";
        }


        // Cursor stuff
        Vector2 mouse = Input.mousePosition;

        int stage = selectionScript.getBuildable().getStage();
        string property = selectionScript.getBuildable().getProperty();

        if (stage == BuildableRoom.STAGE_ITEMS && property == BuildableRoom.PROPERTY_ITEMS_EDIT)
        {
            //Debug.Log("Place");
            currentMouseImage.enabled = true;
            currentMouseImage.sprite = rotateImage;
            float imageX = mouse.x + currentMouseImage.rectTransform.rect.width;
            float imageY = mouse.y - currentMouseImage.rectTransform.rect.height / 2;
            currentMouseImage.transform.position = new Vector2(imageX, imageY);
        }
        else if (stage == BuildableRoom.STAGE_ITEMS && property == BuildableRoom.PROPERTY_ITEMS_PLACE)
        {
            //Debug.Log("Create");
            currentMouseImage.enabled = true;
            currentMouseImage.sprite = IconImage;
            float imageX = mouse.x + currentMouseImage.rectTransform.rect.width;
            float imageY = mouse.y - currentMouseImage.rectTransform.rect.height / 2;
            currentMouseImage.transform.position = new Vector2(imageX, imageY);
        }
        else
        {
            currentMouseImage.enabled = false;
        }

    }
}
