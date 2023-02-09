using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Klasse regelt den Baumodus //

public class Build : MonoBehaviour {
    bool isBuildModeOn;
    InventoryItem currentItem;
    GameObject cursorBlock;
    GameObject block;
    GameObject chestPanel;
    Transform crosshairPos;
    GameObject crosshair;
    GameObject player;
    GameObject handImage;
    Vector3 blockRotation = new Vector3(0, 0, 0);
    GameObject cameraMain;
    Vector3 rotation = new Vector3(0,0,0);
    // variables for placing the roof at correct height
    float wallHeight = 0f;
    float additionalDistance = 0.1739f;

    private void Awake()
    {
        chestPanel = GameObject.Find("ChestPanel");
        handImage = GameObject.Find("HandImage");
    }

    // Use this for initialization
    void Start () {
        crosshair = GameObject.Find("Crosshair");
        crosshairPos = crosshair.GetComponent<Transform>();
        player = GameObject.Find("Player");
        cameraMain = GameObject.Find("Main Camera");
    }
	
	// Update is called once per frame
	void Update () {
		if(this.isBuildModeOn && cursorBlock != null)
        {
            if (currentItem.itemNumber == 12)
            {
                FindWallPos();
            }
            if (currentItem.itemNumber == 13)
            {
                FindFoundationFloorTileChestPos();
            }
            if (currentItem.itemNumber == 14)
            {
                FindFoundationFloorTileChestPos();
            }
            if (currentItem.itemNumber == 15)
            {
                FindRoofPos();
            }
            if (currentItem.itemNumber == 16)
            {
                FindDoorWallPos();
            }
            if (currentItem.itemNumber == 17)
            {
                FindFoundationFloorTileChestPos();
            }
            if (currentItem.itemNumber == 18)
            {
                FindFoundationFloorTileChestPos();
            }
        }
        else
        {
            blockRotation = new Vector3(0, 0, 0);
        }
	}

    public void startBuildMode(InventoryItem ip)
    {
        SetCurrentItem(ip);
        SetIsBuildModeOn(true);
        InstantiateBlock();
    }

    public void cancelBuildMode()
    {
        SetIsBuildModeOn(false);
        CancelBlockPlacement();
    }

    /* Wird aus dem InventoryUI-Skript gesetzt
    Der Baumodus ist solange an, wie BauItem sich in der Hand befindet*/
    public void SetIsBuildModeOn(bool isBuildModeOn)
    {
        this.isBuildModeOn = isBuildModeOn;
    }

    public bool GetIsBuildModeOn()
    {
        return isBuildModeOn;
    }

    public void InstantiateBlock()
    {
        cursorBlock = (GameObject) Instantiate(this.currentItem.prefab, new Vector3(0,0,0), this.player.transform.localRotation);

        if (currentItem.itemNumber == 16)
        {
            for (int i = 0; i < this.cursorBlock.GetComponentsInChildren<Collider>().Length; i++)
            {
                if (cursorBlock.GetComponentsInChildren<Collider>()[i] != null) Destroy(this.cursorBlock.GetComponentsInChildren<Collider>()[i]);
            }
            for (int i = 0; i < this.cursorBlock.GetComponentsInChildren<Renderer>().Length; i++)
            {
                if(cursorBlock.GetComponentsInChildren<Renderer>()[i] != null) cursorBlock.GetComponentsInChildren<Renderer>()[i].material = MaterialMode.GetTransparentMaterial();
            }
        } else
        {
            Destroy(this.cursorBlock.GetComponent<Collider>());
            cursorBlock.GetComponent<Renderer>().material = MaterialMode.GetTransparentMaterial();
        }
    }

    private void FindWallPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(crosshairPos.position);
        RaycastHit hit;
        // wird etwas getroffen?
        if (Physics.Raycast(ray, out hit))
        {
            var localPoint = hit.transform.InverseTransformDirection(hit.point);
            
            // Handelt es sich bei dem getroffenen GO um eine andere Wand?
            if (hit.collider.CompareTag("block"))
            {
                var localHitDirection = hit.transform.InverseTransformDirection(hit.normal);
                var localBlockDimensions = cursorBlock.transform.InverseTransformDirection(GetDimensions(cursorBlock));
                var localWallPos = hit.transform.InverseTransformDirection(hit.transform.position);
                wallHeight = GetDimensions(cursorBlock).z;

                // Wand von der Seite getroffen?
                if (Math.Abs(Math.Abs(localHitDirection.normalized.y) - Math.Abs(Vector3.up.y)) <= 0.3 || Math.Abs(Math.Abs(localHitDirection.normalized.y) - Math.Abs(Vector3.down.y)) <= 0.3)
                {
                    // Position der getroffenen Wand + Breite der Wand in Richtung des Strahlursprungs
                    cursorBlock.transform.position = hit.transform.position - hit.normal * localBlockDimensions.z;
                    cursorBlock.transform.localEulerAngles = hit.transform.eulerAngles;
                }// Wand von vorne getroffen?
                else if(Math.Abs(Math.Abs(localHitDirection.normalized.x) - Math.Abs(Vector3.right.x)) <= 0.3 || Math.Abs(Math.Abs(localHitDirection.normalized.x) - Math.Abs(Vector3.left.x)) <= 0.3)
                {
                    cursorBlock.transform.localEulerAngles = new Vector3(hit.transform.localEulerAngles.x, hit.transform.localEulerAngles.y, hit.transform.localEulerAngles.z + 90f);  
                    localPoint = hit.transform.InverseTransformDirection(hit.point);

                    if(Math.Abs(localPoint.z) > Math.Abs((localWallPos.z - (hit.transform.localScale.z -4f))))
                    {
                        cursorBlock.transform.localPosition = hit.transform.position + new Vector3(0,hit.transform.localScale.z,0);
                        cursorBlock.transform.localEulerAngles = hit.transform.eulerAngles;
                    } else
                    {
                        if (localPoint.y > localWallPos.y)
                        {
                            cursorBlock.transform.position = hit.transform.position - (hit.normal * localBlockDimensions.z / 2 + hit.transform.up * localBlockDimensions.z / 2);
                        }
                        else
                        {
                            cursorBlock.transform.position = hit.transform.position - (hit.normal * localBlockDimensions.z / 2 - hit.transform.up * localBlockDimensions.z / 2);
                        }
                    }
                }
                else
                {
                    cursorBlock.transform.position = hit.transform.position + localHitDirection * hit.transform.localScale.z;
                    cursorBlock.transform.localEulerAngles = hit.transform.eulerAngles;
                }
            }
            else if (hit.collider.CompareTag("foundation"))
            {
                cursorBlock.transform.position = hit.point + new Vector3(0f, (cursorBlock.transform.localScale.z / 2), 0f);
                //cursorBlock.transform.localEulerAngles = new Vector3(90f, player.transform.localEulerAngles.y, 90f);
                cursorBlock.transform.localEulerAngles = new Vector3(90f, hit.transform.localEulerAngles.y, 90f) + rotation;
            }
            else
            {

            }
        }
    }

    private void FindFoundationFloorTileChestPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(crosshairPos.position);
        RaycastHit hit;
        // wird etwas getroffen?
        if (Physics.Raycast(ray, out hit))
        {
            var localPoint = hit.transform.InverseTransformDirection(hit.point);

            // Handelt es sich bei dem getroffenen GO um einen Block?
            if (hit.collider.CompareTag("block"))
            {      
            }
            else
            {
                if (cursorBlock != null)
                {
                    cursorBlock.transform.position = new Vector3(hit.point.x, hit.point.y + 0.05f, hit.point.z);
                    cursorBlock.transform.localEulerAngles = new Vector3(0f, hit.transform.localEulerAngles.y, 0f) + rotation;
                }
            }
        }
    }

    private void FindRoofPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(crosshairPos.position);
        RaycastHit hit;
        // wird etwas getroffen?
        if (Physics.Raycast(ray, out hit))
        {
            // Handelt es sich bei dem getroffenen GO um ein Fundament?
            if (hit.collider.CompareTag("foundation"))
            {
                cursorBlock.transform.position = hit.transform.position + new Vector3(0, wallHeight + additionalDistance, 0);
                cursorBlock.transform.rotation = hit.transform.rotation;
            }
            else
            {
                cursorBlock.transform.position = new Vector3(hit.point.x, hit.point.y + 0.05f, hit.point.z);
                cursorBlock.transform.localEulerAngles = new Vector3(0f, player.transform.localEulerAngles.y, 0f) + rotation;
            }
        }
    }

    private void FindDoorWallPos()
    {
        // Achtung: DoorWall besteht aus mehreren Child-GameObjects!

        Ray ray = Camera.main.ScreenPointToRay(crosshairPos.position);
        RaycastHit hit;
        // wird etwas getroffen?
        if (Physics.Raycast(ray, out hit))
        {
            // Handelt es sich bei dem getroffenen GO um eine Wand?
            if (hit.collider.CompareTag("foundation"))
            {
                cursorBlock.transform.position = hit.point + new Vector3(0f, GetDimensions(cursorBlock.transform.GetChild(3).gameObject).z / 2, 0f);
                cursorBlock.transform.localRotation = new Quaternion(0, 0, 0, 0);
                cursorBlock.transform.localEulerAngles += rotation;
            }
            else
            {
                if (hit.collider.CompareTag("block"))
                {
                    // Wand getroffen?
                    var localHitDirection = hit.transform.InverseTransformDirection(hit.normal);
                    var localBlockDimensionsChild0 = cursorBlock.transform.InverseTransformDirection(GetDimensions(cursorBlock.transform.GetChild(0).gameObject));
                    var localBlockDimensionsChild2 = cursorBlock.transform.InverseTransformDirection(GetDimensions(cursorBlock.transform.GetChild(2).gameObject));
                    var localBlockDimensionsChild3 = cursorBlock.transform.InverseTransformDirection(GetDimensions(cursorBlock.transform.GetChild(3).gameObject));

                    // Wand von der Seite getroffen?
                    if (Math.Abs(Math.Abs(localHitDirection.normalized.y) - Math.Abs(Vector3.up.y)) <= 0.3 || Math.Abs(Math.Abs(localHitDirection.normalized.y) - Math.Abs(Vector3.down.y)) <= 0.3)
                    {
                        // Position der getroffenen Wand + Breite der Türwand in Richtung des Strahlursprungs
                        cursorBlock.transform.position = hit.transform.position + hit.normal * (localBlockDimensionsChild0.y + localBlockDimensionsChild2.y + localBlockDimensionsChild3.y);
                        cursorBlock.transform.localEulerAngles = new Vector3(hit.transform.eulerAngles.x - 90f, hit.transform.eulerAngles.y -90f, hit.transform.eulerAngles.z);
                        cursorBlock.transform.localEulerAngles += rotation;
                    } else
                    {
                        cursorBlock.transform.position = new Vector3(hit.point.x, hit.point.y + GetDimensions(cursorBlock.transform.GetChild(3).gameObject).z / 2, hit.point.z);
                        cursorBlock.transform.localRotation = new Quaternion(0, 0, 0, 0);
                        cursorBlock.transform.localEulerAngles += rotation;
                    }
                } else
                {
                    cursorBlock.transform.position = new Vector3(hit.point.x, hit.point.y + GetDimensions(cursorBlock.transform.GetChild(3).gameObject).z / 2, hit.point.z);
                    cursorBlock.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    cursorBlock.transform.localEulerAngles += rotation;
                }

            }
        }
    }

    public void SetCurrentItem(InventoryItem currentItem)
    {
        this.currentItem = currentItem;
    }

    public void RotateBlockOnMouseClick()
    {
        //cursorBlock.transform.localEulerAngles += new Vector3(0, 90f, 0);
        this.rotation += new Vector3(0, 90f, 0);
    }

    public void ReplaceWithRealBlock()
    {
        block = (GameObject)Instantiate(currentItem.prefab, cursorBlock.transform.position, cursorBlock.transform.localRotation);

        // Handelt es sich um eine Truhe?
        if (currentItem.itemNumber == 17)
        {
            // Dann instanziere zusätzlich ein Truhen-UI und setze wichtige Variablen!
            //instantiateChestUI(block);
        }
    }

    // wird aufgerufen von Player.equip() wenn Bauitem unequiped wird
    public void CancelBlockPlacement()
    {
        if(cursorBlock)
        {
            Destroy(cursorBlock);
        }
    }

    private Vector3 GetDimensions(GameObject obj)
    {
        Vector3 min = Vector3.one * Mathf.Infinity;
        Vector3 max = Vector3.one * Mathf.NegativeInfinity;

        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 vert = mesh.vertices[i];
            min = Vector3.Min(min, vert);
            max = Vector3.Max(max, vert);
        }

        // the size is max-min multiplied by the object scale:
        return Vector3.Scale(max - min, obj.transform.localScale);
    }

    public void instantiateChestUI()
    {
        if (chestPanel == null)
        {
            chestPanel = GameObject.Find("ChestPanel");
        }
        int z = chestPanel.transform.GetSiblingIndex();
        GameObject oldChestPanel = chestPanel;
        chestPanel = (GameObject)Instantiate(chestPanel, chestPanel.transform.position, chestPanel.transform.localRotation);
        chestPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 255.91f);
        chestPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 272.06f);
        chestPanel.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
        chestPanel.GetComponent<RectTransform>().localScale = oldChestPanel.GetComponent<RectTransform>().localScale;
        chestPanel.transform.SetSiblingIndex(z + 1);

        Inventory chestInventory = block.GetComponent<Inventory>();
        chestInventory.setChestPanel(chestPanel);

        //for (int i = 0; i < chestInventory.itemImageSlots.Length; i++)
        //{
        //    chestInventory.itemImageSlots[i] = chestPanel.transform.GetChild(0).GetChild(2 + i).GetChild(0).gameObject;
        //}

        if (handImage != null) handImage.GetComponent<DropItem>().addChest(chestPanel, block);
    }
}
