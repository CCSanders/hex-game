using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{
    public HexGrid grid;
    HexCell currentCell;

    Vector3 lastMousePosition;
    MapCamera mapCamera;

    delegate void UpdateFunc();
    UpdateFunc Update_CurrentFunc;

    SelectionController selectionController;

    private void Start()
    {
        mapCamera = FindObjectOfType<MapCamera>();
        selectionController = GetComponent<SelectionController>();
        Update_CurrentFunc = Update_DetectModeStart;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelUpdateFunc();
            return;
        }

        Update_CurrentFunc();

        Update_ScrollZoom();

        lastMousePosition = Input.mousePosition;
    }

    void CancelUpdateFunc()
    {
        grid.ClearPath();

        if (Input.GetMouseButton(1))
        {
            //if the update function was cancelled but the right mouse button is still held down, do nothing.
            Debug.Log("trying to cancel update function but mouse is still held down");
        }

        Update_CurrentFunc = Update_DetectModeStart;
    }

    void Update_DetectModeStart()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //this checks if we are over a UI element. TODO: do we want to ignore all GUI objects?
            // consider things like unit health bars, resource icons, etc. 
            // although, if those are set to noninteractive or not block raycasts, maybe this 
            //will return false for them anyway.

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            DoSelection(); //only change the selection when the mouse isn't clicking on UI
        }
        else if (Input.GetMouseButton(0) && Input.mousePosition != lastMousePosition)
        {
            //could be used for dragging the camera... 
        }
        else if (selectionController.SelectedUnit && Input.GetMouseButtonDown(1))
        {
            //selected unit, holding down rmb. unit movement mode. 

            //first, clear the selected units actual path if it exists (just visually)
            selectionController.SelectedUnit.GetUnitView().ClearPath();

            Update_CurrentFunc = Update_UnitPathfinding;
        }
    }
    
    void Update_ScrollZoom()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta == 0f)
        {
            return;
        }

        mapCamera.AdjustZoom(zoomDelta);
    }

    void Update_UnitMovement()
    {
        if (grid.HasPath())
        {
            List<HexCell> path = grid.GetPath();
            grid.ClearPath();

            selectionController.SelectedUnit.Travel(path);
        }

        CancelUpdateFunc();
        return;
    }

    void Update_UnitPathfinding()
    {
        if (Input.GetMouseButtonUp(1))
        {
            Update_CurrentFunc = Update_UnitMovement;
        }

        if (UpdateCurrentCell())
        {
            if (currentCell && selectionController.SelectedUnit.IsValidDestination(currentCell))
            {
                grid.FindPath(selectionController.SelectedUnit.Location, currentCell, selectionController.SelectedUnit);
            }
            else
            {
                grid.ClearPath();
            }
        }
    }

    void DoSelection()
    {
        selectionController.DoSelection();
    }

    bool UpdateCurrentCell()
    {
        HexCell cell = grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
        if (cell != currentCell)
        {
            currentCell = cell;
            return true;
        }
        return false;
    }

    public HexCell GetCurrentCell()
    {
        UpdateCurrentCell();
        return currentCell;
    }
}
