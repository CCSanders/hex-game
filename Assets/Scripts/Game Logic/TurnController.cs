using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnController : MonoBehaviour
{
    HexGrid grid;
    SelectionController selectionController;

    int turnNumber = 1;

    //game ui should have buttons that connect to the methods

    public enum ActionButtonState
    {
        EndTurn, UnitWaitingForOrders, CityReadyForProduction
    }

    private ActionButtonState currentState = ActionButtonState.EndTurn;
    public ActionButtonState CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            currentState = value;
            OnActionButtonStateChanged();
        }
    }

    public Button ActionButton;

    //C# HELP: Delegates are simply pointers to functions.
    public delegate void OnTurnEnded();
    public event OnTurnEnded onTurnEnded;

    public void Start()
    {
        grid = FindObjectOfType<HexGrid>();
        selectionController = FindObjectOfType<SelectionController>();
    }

    public void OnActionButtonPressed()
    {
        switch (CurrentState)
        {
            case ActionButtonState.EndTurn:
                EndTurn();
                break;
            case ActionButtonState.UnitWaitingForOrders:
                UnitWaitingForOrders();
                break;
            case ActionButtonState.CityReadyForProduction:
                CityReadyForProduction();
                break;
        }
    }

    private void Update()
    {
        //this is where you could run the ai code.

        List<Unit> units = grid.GetUnits();
        List<City> cities = grid.GetCities();

        CurrentState = ActionButtonState.EndTurn;

        foreach (Unit u in units)
        {
            if (u.UnitWaitingForOrders())
            {
                CurrentState = ActionButtonState.UnitWaitingForOrders;
                break;
            }
        }

        foreach (City c in cities)
        {
            if(c.GetCurrentBuildingJob() == null)
            {
                CurrentState = ActionButtonState.CityReadyForProduction;
                break;
            }
        }
    }

    private void OnActionButtonStateChanged()
    {
        switch (CurrentState)
        {
            case ActionButtonState.EndTurn:
                ActionButton.GetComponentInChildren<Text>().text = "End Turn";
                break;
            case ActionButtonState.UnitWaitingForOrders:
                ActionButton.GetComponentInChildren<Text>().text = "Unit is Waiting for Orders";
                break;
            case ActionButtonState.CityReadyForProduction:
                ActionButton.GetComponentInChildren<Text>().text = "Choose Production for City";
                break;
        }
    }

    public void UnitWaitingForOrders()
    {
        //this method happens when the action button is clicked and there is a unit waiting for orders.

        //first thing that should happen is that selection controller should determine which unit is waiting for orders
        selectionController.SelectNextUnit(true);

        //then the camera should move towards that unit

        Debug.Log("there is a unit waiting for orders");
    }

    public void CityReadyForProduction()
    {
        List<City> cities = grid.GetCities();

        foreach (City c in cities)
        {
            if (c.GetCurrentBuildingJob() == null)
            {
                FindObjectOfType<MapCamera>()?.MoveToPosition(c.transform.position);
                FindObjectOfType<GameUI>()?.OpenCityUI(c);
                return;
            }
        }
    }

    public void EndTurn()
    {
        //to-do: make it only get the current player's units.
        List<Unit> units = grid.GetUnits();
        List<City> cities = grid.GetCities();

        //if there is a unit waiting for orders, halt the end turn function and select the unit
        foreach (Unit u in units)
        {
            if (u.UnitWaitingForOrders())
            {
                selectionController.SelectedUnit = u;
                CurrentState = ActionButtonState.UnitWaitingForOrders;
                FindObjectOfType<MapCamera>()?.MoveToPosition(u.transform.position);
                return;
            }
        }

        Debug.Log("no units waiting for orders.");

        //units that have a current path should travel
        foreach (Unit u in units)
        {
            u.Travel(u.GetCurrentPath());
        }

        //TO-DO heal units that are resting

        //do city logic
        foreach (City c in cities)
        {
            c.DoTurn();
        }

        //unselect units and go to next player
        selectionController.SelectedUnit = null;

        turnNumber++;

        //anything that's waiting for the turn to end can subscribe to this and will be called here
        //unit movement is restored through the on turn ended event

        onTurnEnded?.Invoke();
    }

    public int GetCurrentTurnNumber()
    {
        return turnNumber;
    }
}
