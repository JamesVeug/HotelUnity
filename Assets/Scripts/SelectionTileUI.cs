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
    public Image currentMouseImage;
    public Sprite rotateImage;
    public Sprite IconImage;

    private GameData data;

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
        itemText.text =     "" + ((BuildableRoom)buildable).getCurrentBuildingItem();

        string text = "";
        for (int y = data.dTileMap.height-1; y >= 0; y--)
        {
            for(int x = 0; x < data.dTileMap.width; x++)
            {
                text += data.dTileMap.getTile(x, y);
            }
            text += "\n";
        }

        mapText.text = text;


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
