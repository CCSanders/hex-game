using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Utility class to find paths between cells. 
public class GridPathfinder : MonoBehaviour
{
    HexGrid grid;

    HexPriorityQueue searchFrontier;
    int searchFrontierPhase;

    int turnToDistanceRatio = 5;

    public bool HasPath
    {
        get
        {
            return currentPathExists;
        }
        private set { currentPathExists = value; }
    }

    bool currentPathExists;
    HexCell currentPathFrom, currentPathTo;

    //checks the bool to see if a current path has been generated.
    //if so, it gets a list and adds the cells along the path starting at the end and 
    //moving backwards. adds the starting cell, reverses the list. 
    public List<HexCell> GetPath()
    {
        if (!currentPathExists)
        {
            return null;
        }
        List<HexCell> path = ListPool<HexCell>.Get();
        for (HexCell c = currentPathTo; c != currentPathFrom; c = c.PathFrom)
        {
            path.Add(c);
        }
        path.Add(currentPathFrom);
        path.Reverse();
        return path;
    }

    //checks to see if a current path exists. if so, it goes through it, removes the active gui
    //and sets the references to false.
    //if a path doesn't exists, but there start cell reference isn't null, it removes the active gui
    //and then sets the cell references to null.
    public void ClearPath()
    {
        if (currentPathExists)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                current.SetLabel(null);
                current.DisableHighlight();
                current = current.PathFrom;
            }
            current.DisableHighlight();
            currentPathExists = false;
        }
        else if (currentPathFrom)
        {
            currentPathFrom.DisableHighlight();
            currentPathTo.DisableHighlight();
        }
        currentPathFrom = currentPathTo = null;
    }

    //shows the current path at a given speed, if it exists.
    //even it if doesn't, set the highlight on the start and end hexes. 
    void ShowPath(int unitMovement, int unitMovementRemaining)
    {
        if (currentPathExists)
        {
            HexCell current = currentPathTo;
            while (current != currentPathFrom)
            {
                int turn = (current.MovementCost - 1) / unitMovement; //turn cost if cell has full movement
                if(unitMovementRemaining == 0)
                {
                    turn++;
                }
                else if (unitMovementRemaining < unitMovement)
                {
                    int remainder = (current.MovementCost - 1) % unitMovement;
                    if(remainder >= unitMovementRemaining)
                    {
                        turn++;
                    }
                }

                current.SetLabel(turn.ToString());
                current.EnableHighlight(Color.white);
                current = current.PathFrom;
            }
        }
        currentPathFrom.EnableHighlight(Color.blue);
        currentPathTo.EnableHighlight(Color.red);
    }

    //clears the old path, sets the goal start and end cells, calculates the path
    //and then shows the path with the units speed.
    public void FindPath(HexCell fromCell, HexCell toCell, Unit unit)
    {
        ClearPath();
        currentPathFrom = fromCell;
        currentPathTo = toCell;
        currentPathExists = Search(fromCell, toCell, unit);
        ShowPath(unit.movement, unit.movementRemaining);
    }

    //the main search method. returns whether or not a path has been found. 
    bool Search(HexCell fromCell, HexCell toCell, Unit unit)
    {
        searchFrontierPhase += 2;
        if (searchFrontier == null)
        {
            searchFrontier = new HexPriorityQueue();
        }
        else
        {
            searchFrontier.Clear();
        }

        fromCell.SearchPhase = searchFrontierPhase;
        fromCell.MovementCost = 0; //in turns, but unadjusted to speed
        searchFrontier.Enqueue(fromCell);
        while (searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            current.SearchPhase += 1;

            if (current == toCell)
            {
                return true;
            }

            int currentCellMovementCost = current.MovementCost - 1;

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (neighbor == null || neighbor.SearchPhase > searchFrontierPhase)
                {
                    continue;
                }
                if (!unit.IsValidDestination(neighbor))
                {
                    continue;
                }


                float movementCost = unit.GetMoveCost(current, neighbor, d);


                if (movementCost < 0)
                {
                    continue;
                }

                //DEBUG
                movementCost = 1;
                //DEBUG

                int newNeighborMC = (int)(current.MovementCost + movementCost);

                if (neighbor.SearchPhase < searchFrontierPhase)
                {
                    neighbor.SearchPhase = searchFrontierPhase;
                    neighbor.MovementCost = newNeighborMC;
                    neighbor.PathFrom = current;
                    neighbor.SearchHeuristic = neighbor.coordinates.DistanceTo(toCell.coordinates);
                    searchFrontier.Enqueue(neighbor);
                }
                else if (newNeighborMC < neighbor.MovementCost)
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.MovementCost = newNeighborMC;
                    neighbor.PathFrom = current;
                    searchFrontier.Change(neighbor, oldPriority);
                }
            }
        }
        return false;
    }

    public void IncreaseVisibility(HexCell fromCell, int range)
    {
        List<HexCell> cells = GetVisibleCells(fromCell, range);
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].IncreaseVisibility();
        }
        ListPool<HexCell>.Add(cells);
    }

    public void DecreaseVisibility(HexCell fromCell, int range)
    {
        List<HexCell> cells = GetVisibleCells(fromCell, range);
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].DecreaseVisibility();
        }
        ListPool<HexCell>.Add(cells);
    }

    List<HexCell> GetVisibleCells(HexCell fromCell, int range)
    {
        List<HexCell> visibleCells = ListPool<HexCell>.Get();

        searchFrontierPhase += 2;
        if (searchFrontier == null)
        {
            searchFrontier = new HexPriorityQueue();
        }
        else
        {
            searchFrontier.Clear();
        }

        range += fromCell.ViewElevation;
        fromCell.SearchPhase = searchFrontierPhase;
        fromCell.MovementCost = 0;
        searchFrontier.Enqueue(fromCell);
        HexCoordinates fromCoordinates = fromCell.coordinates;
        while (searchFrontier.Count > 0)
        {
            HexCell current = searchFrontier.Dequeue();
            current.SearchPhase += 1;
            visibleCells.Add(current);

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);
                if (
                    neighbor == null ||
                    neighbor.SearchPhase > searchFrontierPhase ||
                    !neighbor.Explorable
                )
                {
                    continue;
                }

                int distance = current.MovementCost + 1;
                if (distance + neighbor.ViewElevation > range ||
                    distance > fromCoordinates.DistanceTo(neighbor.coordinates)
                )
                {
                    continue;
                }

                if (neighbor.SearchPhase < searchFrontierPhase)
                {
                    neighbor.SearchPhase = searchFrontierPhase;
                    neighbor.MovementCost = distance;
                    neighbor.SearchHeuristic = 0;
                    searchFrontier.Enqueue(neighbor);
                }
                else if (distance < neighbor.MovementCost)
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.MovementCost = distance;
                    searchFrontier.Change(neighbor, oldPriority);
                }
            }
        }
        return visibleCells;
    }

    public void Start()
    {
        grid = GetComponent<HexGrid>();
    }
}
