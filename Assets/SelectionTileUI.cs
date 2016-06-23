using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectionTileUI : MonoBehaviour {
    public SelectionTile selectionScript;
    public Text stageText;
    public Text propertyText;
    public Text mapText;
    public Image currentMouseImage;
    public Texture2D rotateImage;
    public Texture2D IconImage;

    private GameData data;

    // Use this for initialization
    void Start () {
        data = FindObjectOfType<GameData>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        stageText.text = ""+selectionScript.getBuildable().getStage();
        propertyText.text = selectionScript.getBuildable().getProperty();

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


        if (selectionScript.getBuildable().getProperty() == BuildableRoom.PROPERTY_ITEMS_EDIT)
        {
            currentMouseImage.enabled = true;
            currentMouseImage.material.mainTexture = rotateImage;
            float imageX = mouse.x + currentMouseImage.rectTransform.rect.width / 2;
            float imageY = mouse.y + currentMouseImage.rectTransform.rect.height / 2;
            currentMouseImage.transform.position = new Vector2(imageX, imageY);
        }
        else if (selectionScript.getBuildable().getStage() == BuildableRoom.STAGE_ITEMS)
        {
            currentMouseImage.enabled = true;
            currentMouseImage.material.mainTexture = IconImage;
            float imageX = mouse.x + currentMouseImage.rectTransform.rect.width / 2;
            float imageY = mouse.y + currentMouseImage.rectTransform.rect.height / 2;
            currentMouseImage.transform.position = new Vector2(imageX, imageY);
        }
        else
        {
            currentMouseImage.enabled = false;
        }

    }
}
