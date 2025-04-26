using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    [Header("Setup")]
    public int itemID, requiredItemID;
    public Transform goToPoint;
    public string objectName;
    public Vector2 nameTagSize = new Vector2(3, 0.65f);
    public string itemName;
    public Vector2 itemNameTagSize = new Vector2(3, 0.65f);

    [Header("Item picked up")]
    public GameObject[] objectsToRemove;
    public GameObject[] objectsToSetActive;
    public Sprite itemSlotSprite;

    [Header("Fail/Cant pick up")]
    [TextArea(4,4)]
    public string hintMessage;
    public Vector2 hintBoxSize = new Vector2(4,0.85f);
}
