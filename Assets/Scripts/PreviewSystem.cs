using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size) 
    {
        previewObject = Instantiate(prefab);
        SetPreviewLayer(previewObject, 23);
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

    private void SetPreviewLayer(GameObject obj, int layerLevel)
    {
        obj.layer = layerLevel;
        //Debug.Log("Obj name:" + obj.name);
        foreach (Transform child in obj.transform)
        {
            SetPreviewLayer(child.gameObject, layerLevel);
        }
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0) 
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;

        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++) 
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        Destroy(previewObject);
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        MovePreview(position);
        /*
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            previewObject.transform.Rotate(new Vector3(0, 90, 0));
            Debug.Log("Rotate");
        }
        */
        MoveCursor(position);
        ApplyFeedback(validity);
    }

    public void UpdateRotation() 
    {
        previewObject.transform.Rotate(new Vector3(0, 90, 0));
        Debug.Log("Rotate");
    }

    public Quaternion GetPreviewObjectRotation() 
    {
        return previewObject.transform.rotation;
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }

    private void ApplyFeedback(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        cellIndicatorRenderer.material.color = c;
        c.a = 0.05f;
        previewMaterialInstance.color = c;
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset, position.z);
    }
}
