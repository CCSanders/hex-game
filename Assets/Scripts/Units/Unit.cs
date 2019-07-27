using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Unit : MonoBehaviour {

    public string unitName = "Unnamed Unit";
    public int healthPoints = 100;
    public int strength = 8;
    public int movement = 4; //to do: change this to small values and edit pathfinding algorithm
    public int movementRemaining = 4;
    public int visionRange = 10;

    public bool canBuildCities = false;
    public bool skipThisUnit = false;

    public int factionID = 0;

    public int prefabID;

	public HexGrid Grid { get; set; }

	public HexCell Location {
		get {
			return location;
		}
		set {
			if (location) {
				Grid.DecreaseVisibility(location, visionRange);
                location.RemoveUnit(this);
			}
			location = value;
			location.AddUnit(this);
			Grid.IncreaseVisibility(value, visionRange);
			transform.localPosition = value.Position;
			Grid.MakeChildOfColumn(transform, value.ColumnIndex);
		}
	}

	HexCell location;
    List<HexCell> currentPath;
    List<HexCell> pathTraveledThisTurn;

    UnitView unitView;

    public delegate void UnitMovedDelegate(List<HexCell> path);
    public event UnitMovedDelegate OnUnitMoved;

    TurnController turnController;

    public void ValidateLocation () {
		transform.localPosition = location.Position;
	}

	public bool IsValidDestination (HexCell cell) {
        return cell.IsExplored && !cell.IsUnderwater;
	}

    public List<HexCell> GetCurrentPath()
    {
        return currentPath;
    }

    public bool UnitWaitingForOrders()
    {
        if (skipThisUnit)
        {
            return false;
        }

        //if the unit has movement left and no current path, the unit is waiting for orders
        if(movementRemaining > 0 && (currentPath == null || currentPath.Count == 0))
        {
            //to-do: add options for fortify, alert, skipturn
            return true;
        }

        return false;
    }

	public void Travel (List<HexCell> path)
    {
        skipThisUnit = false;
        unitView.ClearPath();
        Grid.ClearPath();

        if (path == null || path.Count <= 1)
        {
            currentPath = null;

            return;
        }

        currentPath = path;

        pathTraveledThisTurn = new List<HexCell>();
        pathTraveledThisTurn.Add(location);

        int i = 0;
        while (this.DoMove())
        {
            i++;
        }

        if(pathTraveledThisTurn != null && pathTraveledThisTurn.Count > 1)
        {
            OnUnitMoved?.Invoke(pathTraveledThisTurn);
        }
    }

    //processes one tile worth of movement for the unit. returns true if it should be called immediately again
    public bool DoMove()
    {
        if (movementRemaining <= 0)
        {
            Debug.Log("Unit out of movement.");
            return false;
        }

        if(currentPath == null || currentPath.Count <= 0)
        {
            Debug.Log("Unit finished path.");
            return false;
        }

        HexCell leavingCell = currentPath[0];
        HexCell newCell = currentPath[1];

        int costToEnterNewCell = newCell.MovementCost;

        leavingCell.RemoveUnit(this);
        currentPath.RemoveAt(0);

        if(currentPath.Count == 1)
        {
            //only hex left, so we have arrived.
            currentPath.Clear();
            currentPath = null;
        }

        location = newCell;
        location.AddUnit(this);
        pathTraveledThisTurn.Add(location);

        movementRemaining = Mathf.Max(movementRemaining - costToEnterNewCell, 0);

        return currentPath != null && movementRemaining > 0;
    }

    public UnitView GetUnitView()
    {
        return unitView;
    }

	public float GetMoveCost (HexCell fromCell, HexCell toCell, HexDirection direction)
	{
		if (!IsValidDestination(toCell)) {
			return -1;
		}
		EdgeType edgeType = fromCell.GetEdgeType(toCell);
		if (edgeType == EdgeType.Cliff) {
			return -1;
		}
		float moveCost;
		if (fromCell.HasRoadThroughEdge(direction)) {
			moveCost = .5f;
		}
		//else if (fromCell.Walled != toCell.Walled) { //for now, turn off not being able to move through walls
			//return -1;
		//}
		else {
			moveCost = edgeType == EdgeType.Flat ? 1 : 2;
			if(toCell.PlantLevel == TerrainType.Forest)
            {
                moveCost = 2; //if its a forest, takes an extra turn
            }
            else if(toCell.PlantLevel == TerrainType.Jungle)
            {
                moveCost += 3;
            }
		}
		return moveCost;
	}

    public float MovementCostToEnterHex(HexCell cell)
    {
        if (!IsValidDestination(cell))
        {
            return -1;
        }

        if(cell.Elevation == TerrainType.Mountain)
        {
            return -1;
        }

        //default is that 1 turn = 5 movement cost. 
        float moveCost;
        moveCost = cell.Elevation == TerrainType.Flat ? 1 : 2;
        if (cell.PlantLevel == TerrainType.Forest)
        {
            moveCost = 2; //if its a forest, takes an extra turn
        }
        else if (cell.PlantLevel == TerrainType.Jungle)
        {
            moveCost = 3;
        }
        return moveCost;
    }

    public void RefreshMovement()
    {
        skipThisUnit = false;
        movementRemaining = movement;
    }

	public void Die () {
		if (location) {
			Grid.DecreaseVisibility(location, visionRange);
		}
        unitView.ClearPath();
        location.RemoveUnit(this);
        turnController.onTurnEnded -= RefreshMovement;
		Destroy(gameObject);
	}

	public void Save (BinaryWriter writer) {
		location.coordinates.Save(writer);
		writer.Write(unitView.Orientation);
	}

	public void Load (BinaryReader reader, HexGrid grid) {
		HexCoordinates coordinates = HexCoordinates.Load(reader);
		float orientation = reader.ReadSingle();
		//grid.AddUnit(
			//Instantiate(unitPrefab), unitPrefab, grid.GetCell(coordinates), orientation
		//);
	}

	void OnEnable () {
        unitView = GetComponent<UnitView>();
        turnController = FindObjectOfType<TurnController>();
        turnController.onTurnEnded += RefreshMovement;

        if (location) {
			transform.localPosition = location.Position;
            Grid.IncreaseVisibility(location, visionRange);
		}
	}
}