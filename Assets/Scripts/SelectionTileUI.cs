using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SelectionTileUI : MonoBehaviour {
    public SelectionTile selectionScript;
    public Text roomText;
    public Text stageText;
    public Text propertyText;
    public Text itemText;
    public Text mapText;
    public Text guestInfo;
    public Text soldBedText;
    public Text rejectedText;
    public Text goldText;
    public GameObject subChangeGoldTextPrefab;
    public GameObject addChangeGoldTextPrefab;
    public Image currentMouseImage;
    public Sprite rotateImage;
    public Sprite IconImage;
    public Camera guestCamera;

    private GameData data;
    private Gold visibleGold;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    } 

    // Use this for initialization
    void Start () {
        data = FindObjectOfType<GameData>();
        visibleGold = Gold.create(0);

    }
	
	// Update is called once per frame
	void LateUpdate () {
        Buildable buildable = selectionScript.getBuildable();
        soldBedText.text  = "" + data.gameLogic.soldBeds;
        rejectedText.text = "" + data.gameLogic.rejectedPeople;
        updateGoldText();        
        roomText.text =     "" + buildable.GetType().Name;
        stageText.text =    "" + buildable.getStage();
        propertyText.text = "" + buildable.getProperty();
        itemText.text =     "";
        if (buildable is BuildableRoom)
        {
            BuildableItem item = ((BuildableRoom)buildable).getCurrentBuildingItem();
            itemText.text += item == null ? "-" : item.GetType().Name;
        }
        else if (buildable is BuildableTile)
        {
            itemText.text += "Something of a tile";
        }
        else if (buildable is BuildableSelector)
        {
            BuildableSelector selector = ((BuildableSelector)buildable);

            string message = "Nothing Selected";
            BuildableRoom room = selector.selectedRoom;
            if (room != null)
            {
                message = room.GetType().Name;
                BuildableItem item = selector.selectedItem;
                if (item != null)
                {
                    message += "->" + item.GetType().Name;
                }
            }
            itemText.text = message;

            // Guest Info
            GuestAI[] ais = FindObjectsOfType<GuestAI>();
            guestInfo.text = "Guests: " + ais.Length;
            if (selector.selectedAI != null)
            {
                guestInfo.text += "\n\nSelectedAI: " + selector.selectedAI.GetType().ToString();
                guestInfo.text += "\nName: " + selector.selectedAI.getAIName();
                guestInfo.text += "\nGold: " + selector.selectedAI.gold;

                if(selector.selectedAI.getOwnedBed() != null ){
                    guestInfo.text += "\nBed: " + selector.selectedAI.getOwnedBed().GetType().ToString();
                }
                guestCamera.enabled = true;
                guestCamera.GetComponent<TrackGuest>().track(selector.selectedAI.transform);
            }
            else
            {
                guestCamera.enabled = false;
                guestCamera.GetComponent<TrackGuest>().stopTracking();
            }
            
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

    private void updateGoldText()
    {
        if (!goldText.text.Equals(data.gameLogic.getGold()))
        {
            Gold gameGold = data.gameLogic.getGold();
            if (visibleGold < gameGold)
            {
                // Gained Money
                GameObject o = Instantiate(addChangeGoldTextPrefab);
                o.transform.position = goldText.transform.position;
                o.transform.SetParent(FindObjectOfType<Canvas>().transform);
                o.GetComponent<Text>().text = "+" + (gameGold - visibleGold).amount;
            }
            if (visibleGold > gameGold)
            {
                // Lost Money
                GameObject o = Instantiate(subChangeGoldTextPrefab);
                o.transform.position = goldText.transform.position;
                o.transform.SetParent(FindObjectOfType<Canvas>().transform);
                o.GetComponent<Text>().text = "-" + (gameGold - visibleGold).amount;
            }
            visibleGold.amount = gameGold.amount;
            goldText.text = "" + visibleGold;
        }
    }
}
