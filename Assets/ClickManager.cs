using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public bool playerWalking;
    public Transform player;
    GameManager gameManager;


    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }


    public void GoToItem(ItemData item)
    {
        //play click sound
        //gameManager.PlaySound(GameManager.soundsNames.click);
        //update hint box
        gameManager.UpdateHintBox(null, false);
        //play walk animation
        player.GetComponent<SpriteAnimator>().PlayAnimation(gameManager.playerAnimations[1]);
        playerWalking = true;
        //start moving player
        StartCoroutine(gameManager.MoveToPoint(player, item.goToPoint.position));
        //equipment stuff
        TryGettingItem(item);
    }




    private void TryGettingItem(ItemData item)
    {
        bool canGetItem = item.requiredItemID == -1 || gameManager.selectedItemID == item.requiredItemID;
        if (canGetItem)
        {
            // Create a new empty GameObject just to hold the data
            GameObject cloneGO = new GameObject("InventoryItem_" + item.itemID);
            DontDestroyOnLoad(cloneGO); // Optional, to persist across scenes

            // Add ItemData and copy the fields
            ItemData itemCopy = cloneGO.AddComponent<ItemData>();

            itemCopy.itemID = item.itemID;
            itemCopy.requiredItemID = item.requiredItemID;
            itemCopy.objectName = item.objectName;
            itemCopy.nameTagSize = item.nameTagSize;
            itemCopy.itemName = item.itemName;
            itemCopy.itemNameTagSize = item.itemNameTagSize;
            itemCopy.hintMessage = item.hintMessage;
            itemCopy.hintBoxSize = item.hintBoxSize;
            itemCopy.itemSlotSprite = item.itemSlotSprite;

            // No world references (like goToPoint, objectsToRemove), just UI data
            GameManager.collectedItems.Add(itemCopy);
        }

        StartCoroutine(UpdateSceneAfterAction(item, canGetItem));
    }




    private IEnumerator UpdateSceneAfterAction(ItemData item, bool canGetItem)
    {
        //wait for player reaching target
        while (playerWalking)
            yield return new WaitForSeconds(0.05f);
        //play player's base animation
        player.GetComponent<SpriteAnimator>().PlayAnimation(null);
        yield return new WaitForSeconds(0.5f);

        if (canGetItem)
        {
            //play use sound
           // gameManager.PlaySound(GameManager.soundsNames.use);
            //play use animation
            //player.GetComponent<SpriteAnimator>().PlayAnimation(gameManager.playerAnimations[2]);
            //remove objects
            foreach (GameObject g in item.objectsToRemove)
                Destroy(g);

            //show objects
            foreach (GameObject g in item.objectsToSetActive)
                g.SetActive(true);

            gameManager.UpdateEquipmentCanvas();
        }
        else
        {
            gameManager.UpdateHintBox(item, player.GetComponentInChildren<SpriteRenderer>().flipX);
        }
        gameManager.CheckSpecialConditions(item);
        yield return null;
    }
}