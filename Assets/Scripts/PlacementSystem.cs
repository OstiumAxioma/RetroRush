using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;
    private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;

    private GridData landData;

    private List<GameObject> placedGameObjects = new();

    [SerializeField]
    private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    public event Action OnPlacementCompleted;

    private void Start()
    {
        StopPlacement();
        landData = new GridData();
        initFirstLand(0);
    }

    private void initFirstLand(int firstID) 
    {
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == firstID);
        Vector3Int gridPosition = new Vector3Int(0, 0, 0);
        GameObject newGameObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        newGameObject.transform.position = grid.CellToWorld(gridPosition);
        placedGameObjects.Add(newGameObject);
        // Debug.Log(newGameObject.name);
        GridData selectedData = landData;
        selectedData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size, database.objectsData[selectedObjectIndex].ID,
            placedGameObjects.Count - 1);
        StopPlacement();
    }

    public void StartPlacement(int ID) 
    {
        // StopPlacement();
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex < 0)
        {
            Debug.Log($"No ID found {ID}");
            return;
        }
        gridVisualization.SetActive(true);
        preview.StartShowingPlacementPreview(database.objectsData[selectedObjectIndex].Prefab,
            database.objectsData[selectedObjectIndex].Size);
        inputManager.OnClicked += PlaceStructure;
        // inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure() 
    {
        if (inputManager.IsPointerOverUI()) {
            EventSystem.current.IsPointerOverGameObject();
            Debug.Log("pointer on UI");
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition + new Vector3(10, 0, 10));
        // Debug.Log(gridPosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (!placementValidity)
        {
            inputManager.OnClicked += PlaceStructure;
            //preview.StopShowingPreview();
            //StartPlacement(GameStatusManager.randomLevel);
            return;
        }
        GameObject newGameObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        newGameObject.transform.position = grid.CellToWorld(gridPosition);
        newGameObject.transform.rotation = preview.GetPreviewObjectRotation();
        placedGameObjects.Add(newGameObject);

        GridData selectedData = landData;
        selectedData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size, database.objectsData[selectedObjectIndex].ID,
            placedGameObjects.Count - 1);
        preview.UpdatePosition(grid.CellToWorld(gridPosition), false);
        StopPlacement();
        OnPlacementCompleted?.Invoke();
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex) 
    {
        GridData selectedData = landData;
        if (selectedObjectIndex >= database.objectsData.Count || selectedObjectIndex < 0) {
            Debug.Log(selectedObjectIndex + ";" + database.objectsData.Count);
            return false;
        }
        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    private void StopPlacement() 
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        preview.StopShowingPreview();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
    }

    private void Update()
    {
        if (selectedObjectIndex < 0)
        {
            return;
        }
        // Debug.Log("Current Index:"  + selectedObjectIndex);
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition + new Vector3(10, 0, 10));
        if (gridPosition != lastDetectedPosition)
        {
            bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

            mouseIndicator.transform.position = mousePosition;
            preview.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
            lastDetectedPosition = gridPosition;
        }
        if (Input.GetKeyDown(KeyCode.Tab) && preview != null)
        {
            preview.UpdateRotation();
        }
    }
}
