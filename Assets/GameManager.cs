using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  
using TMPro;
using UnityEditor.SearchService;
using static UnityEditor.Progress;

public class GameManager : MonoBehaviour
{
    [Header("Setup")]
    public RectTransform nameTag, hintBox;
    public AnimationData[] playerAnimations;


    [Header("Local Scenes")]
    public Image blockingImage;
    public GameObject[] localScenes;
    int activeLocalScene = 0;
    public Transform[] playerStartPositions;

    [Header("Equipment")]
    public GameObject equipmentCanvas;
    public Image[] equipmentSlots, equipmentImages;
    public Sprite emptyItemSlotSprite;
    public Color selectedItemColor;
    public int selectedCanvasSlotID = 0, selectedItemID = -1;


    public static List<ItemData> collectedItems = new  List<ItemData>();
    static float moveSpeed = 3.5f, moveAccuracy = 0.15f;
    
    private bool isTransitioning = false;

    public ParticleSystem playerSmokeFX;

    public enum soundsNames
    {
        //none,
        //click,
        //step,
        //use
    }

    public AudioSource soundtrackSource;
    public AudioClip[] soundEffects;







    public IEnumerator MoveToPoint(Transform myObject, Vector2 point)
    {
        Vector2 positionDifference = point - (Vector2)myObject.position;

        SpriteRenderer spriteRenderer = myObject.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null && positionDifference.x != 0)
            spriteRenderer.flipX = positionDifference.x < 0;  //Flip character code



        while (positionDifference.magnitude > moveAccuracy)
        {
            myObject.Translate(moveSpeed * positionDifference.normalized * Time.deltaTime);
            positionDifference = point - (Vector2)myObject.position;

            if (!playerSmokeFX.isPlaying)
                playerSmokeFX.Play();

            yield return null;
        }

        // Stop the particle system when the object reaches the target point
        if (playerSmokeFX.isPlaying)
            playerSmokeFX.Stop();

        myObject.position = point;
        if (myObject == FindObjectOfType<ClickManager>().player)
            FindObjectOfType<ClickManager>().playerWalking = false;
        myObject.GetComponent<SpriteAnimator>().PlayAnimation(null);

        yield return null;
    }
    public void UpdateNameTag(ItemData item)
    {
        if (item == null) 
        {
            Debug.Log("[UpdateNameTag] Displaying name for item: " + item.itemName);

            nameTag.parent.gameObject.SetActive(false);
            return;
        }

        Debug.Log("[UpdateNameTag] Displaying name for item: " + item.itemName);

        nameTag.parent.gameObject.SetActive(true);
        string nameText = item.objectName;
        Vector2 size = item.nameTagSize;

        nameTag.GetComponentInChildren<TextMeshProUGUI>().text = item.objectName;
        nameTag.sizeDelta = item.nameTagSize;
        nameTag.localPosition = new Vector2(item.nameTagSize.x/2, -1.5f);
    }

    public void UpdateNameTag_Inventory(ItemData item)
    {
        if (item == null)
        {
            nameTag.parent.gameObject.SetActive(false);
            return;
        }

        nameTag.parent.gameObject.SetActive(true);
        nameTag.GetComponentInChildren<TextMeshProUGUI>().text = item.itemName;
        nameTag.sizeDelta = item.itemNameTagSize;
        nameTag.localPosition = new Vector2(item.itemNameTagSize.x / 2, -1.5f);
    }


    public void UpdateHintBox(ItemData Item, bool playerFlipped)
    {
        if (Item == null)
        {
            hintBox.gameObject.SetActive(false); // or just return;
            return;
        }

        hintBox.gameObject.SetActive(true);
        hintBox.GetComponentInChildren<TextMeshProUGUI>().text = Item.hintMessage;
        hintBox.sizeDelta = Item.hintBoxSize;
        hintBox.localPosition = new Vector2(Item.nameTagSize.x / 3, -1.5f);
    }


    public void HideHintBox()
    {
        hintBox.GetComponentInChildren<TextMeshProUGUI>().text = "";
        hintBox.sizeDelta = new Vector2(0, 0);
        hintBox.localPosition = new Vector2(0, 0);
    }

    //EQUIPMENT CODE
    public void SelectItem(int equipmentCanvasID)
    {
        Color c = Color.white;
        c.a = 0;
        //change the alpha of the previous slot to 0
        equipmentSlots[selectedCanvasSlotID].color = c;

        //save changes and stop if an empty slot is clicked or the last item is removed
        if(equipmentCanvasID>= collectedItems.Count || equipmentCanvasID <0)
        {
            //no items selected
            selectedItemID = -1;
            selectedCanvasSlotID = 0;
            return;
        }
        //change the alpha of the new slot to x
        equipmentSlots[equipmentCanvasID].color = selectedItemColor;
        //save changes
        selectedCanvasSlotID = equipmentCanvasID;
        selectedItemID = collectedItems[selectedCanvasSlotID].itemID;
    }



    public void UpdateEquipmentCanvas()
    {
        //find out how many items we have and when to stop
        int itemsAmount = collectedItems.Count, itemSlotsAmount = equipmentSlots.Length;
        //replace no item sprites and old sprites with collectedItems[x].itemSlotSprite
        for (int i = 0; i < itemSlotsAmount; i++)
        {
            //choose between emptyItemSlotSprite and an item sprite
            if (i < itemsAmount && collectedItems[i].itemSlotSprite != null)
                equipmentImages[i].sprite = collectedItems[i].itemSlotSprite;
            else
                equipmentImages[i].sprite = emptyItemSlotSprite;
        }
        //add special conditions for selecting items
        if (itemsAmount == 0)
            SelectItem(-1);
        else if (itemsAmount == 1)
            SelectItem(0);
    }

    public void ShowItemName(int equipmentCanvasID)
    {
        Debug.Log($"[ShowItemName] Called for slot: {equipmentCanvasID}");
        Debug.Log($"[ShowItemName] collectedItems.Count: {collectedItems.Count}");

        if (equipmentCanvasID >= 0 && equipmentCanvasID < collectedItems.Count)
        {
            Debug.Log($"[ShowItemName] Valid index. Showing: {collectedItems[equipmentCanvasID].itemName}");
            UpdateNameTag_Inventory(collectedItems[equipmentCanvasID]);
        }
        else
        {
            Debug.Log("[ShowItemName] Invalid index, hiding nametag.");
            UpdateNameTag(null);
        }
    }



    //SCENE TRANSITION CODE
    public void CheckSpecialConditions(ItemData item)

    {
        switch (item.itemID)
        {
            //if item id == something, go to scene 1
            //if item id == something, go to scene 2
            //if item id == something, end game

            case -10:
                StartCoroutine(ChangeScene(1, 0));
                break;

            case -11:
                StartCoroutine(ChangeScene(1, 0));
                break;

            case -12:
                StartCoroutine(ChangeScene(0, 0));
                break;

            case -13:
                StartCoroutine(ChangeScene(2, 0));
                break;

            case -14:
                StartCoroutine(ChangeScene(2, 1));
                break;

            case -15:
                StartCoroutine(ChangeScene(3, 0));
                break;


            case -20:
                StartCoroutine(ChangeScene(2, 0));
                break;

            case -21:
                StartCoroutine(ChangeScene(4, 0));
                break;
        }
    }



    public IEnumerator ChangeScene(int sceneNumber, float delay)
    {
        Debug.Log("ChangeScene called with sceneNumber: " + sceneNumber);

        Color c = blockingImage.color;
        blockingImage.enabled = true;

        while (blockingImage.color.a < 1)
        {
            c.a += Time.deltaTime;
            blockingImage.color = c;
            yield return null;
        }
        c.a = 1;
        blockingImage.color = c;

        // Wait for the specified delay
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        // Disable all scenes
        foreach (GameObject scene in localScenes)
        {
            if (scene != null)
            {
                Debug.Log("Disabling scene: " + scene.name);
                scene.SetActive(false);
            }
        }

        // Enable the target scene
        if (sceneNumber >= 0 && sceneNumber < localScenes.Length)
        {
            Debug.Log("Enabling target scene: " + sceneNumber);
            localScenes[sceneNumber].SetActive(true);
            activeLocalScene = sceneNumber;
        }
        else
        {
            Debug.LogError("Invalid target scene number: " + sceneNumber);
            yield break;
        }

        if (activeLocalScene == 2)
        {
            //start playing soundtrack music 
            soundtrackSource.Play();
            //turn on the global light
        }

        if (sceneNumber < playerStartPositions.Length && playerStartPositions[sceneNumber] != null)
        {
            FindObjectOfType<ClickManager>().player.position = playerStartPositions[sceneNumber].position;
        }

        foreach (SpriteAnimator spriteAnimator in FindObjectsOfType<SpriteAnimator>())
        {
            spriteAnimator.PlayAnimation(null);
        }

        while (blockingImage.color.a > 0)
        {
            c.a -= Time.deltaTime;
            blockingImage.color = c;
            yield return null;
        }
        blockingImage.color = c;
        blockingImage.enabled = false;

        yield return null;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartGame()
    {
    }



    public IEnumerator GlobalSceneTransition(int sceneIndex)
    {
        Color c = blockingImage.color;
        blockingImage.enabled = true;

        // Fade in
        while (blockingImage.color.a < 1)
        {
            c.a += Time.deltaTime;
            blockingImage.color = c;
            yield return null;
        }
        c.a = 1;
        blockingImage.color = c;

        // Load the new scene
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneIndex);

        // Give the new scene time to initialize
        yield return new WaitForSeconds(0.5f);

        foreach (SpriteAnimator spriteAnimator in FindObjectsOfType<SpriteAnimator>())
        {
            spriteAnimator.PlayAnimation(null);
        }

        // Fade out
        while (blockingImage.color.a > 0)
        {
            c.a -= Time.deltaTime;
            blockingImage.color = c;
            yield return null;
        }
        c.a = 0;
        blockingImage.color = c;
        blockingImage.enabled = false;

    }

    public void ChangeMainScene(int sceneIndex)
    {
        StartCoroutine(GlobalSceneTransition(sceneIndex));
    }





    //CUTSCENE CHANGE HANDLE CODE BELOW
    //UNSUBSCRIBING TO GM IN CASE OF SCENE CHANGE

    void Start()
    {

        BackgroundController.OnCutsceneEnd += HandleCutsceneEnd;
    }

    void OnDestroy()
    {
        BackgroundController.OnCutsceneEnd -= HandleCutsceneEnd;
    }

    void OnDisable()
    {
        BackgroundController.OnCutsceneEnd -= HandleCutsceneEnd;
    }



    private void HandleCutsceneEnd()
    {
        if (isTransitioning) return;  // Prevent multiple transitions
        isTransitioning = true;       // Lock transition

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(GlobalSceneTransition(nextSceneIndex));
        }
        else
        {
            Debug.LogWarning("No more scenes to load! Reached the end of the build order.");
        }
    }
    //Width: 19.16 * 2
    //height: 10.8 * 2



    //AUDIO CODE

    public void PlaySound(soundsNames name)
    {
        //if (name != soundsNames.none)
           // AudioSource.PlayClipAtPoint(soundEffects[(int)name], transform.position);
    }

}