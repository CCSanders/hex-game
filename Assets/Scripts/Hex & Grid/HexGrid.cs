using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class HexGrid : MonoBehaviour {

	public int cellCountX = 20, cellCountZ = 15;

	public bool wrapping;

	public HexCell cellPrefab;
	public Text cellLabelPrefab;
	public HexGridChunk chunkPrefab;

    public City cityPrefab;

	public Texture2D noiseSource;

	public int seed;

	Transform[] columns;
	HexGridChunk[] chunks;
	HexCell[] cells;

	int chunkCountX, chunkCountZ;

    GridPathfinder pathfinder;
    UnitPrefabDatabase unitPrefabDatabase;

	int currentCenterColumnIndex = -1;

	List<Unit> units = new List<Unit>();
    List<City> cities = new List<City>();

    public delegate void CityCreatedDelegate(City city);
    public event CityCreatedDelegate OnCityCreated;

	HexCellShaderData cellShaderData;

	void Awake () {
		HexMetrics.noiseSource = noiseSource;
		HexMetrics.InitializeHashGrid(seed);

        pathfinder = GetComponent<GridPathfinder>();

        cellShaderData = gameObject.AddComponent<HexCellShaderData>();
        cellShaderData.Grid = this;

		//CreateMap(cellCountX, cellCountZ, wrapping);
	}

	public void SpawnUnitAt (Unit unit, HexCell location, float orientation) {
		units.Add(unit);
		unit.Grid = this;
		unit.Location = location;
        unit.GetUnitView().Orientation = orientation;
	}

    public void SpawnUnitAt(int unitID, HexCell location, float orientation)
    {
        if (!unitPrefabDatabase)
        {
            unitPrefabDatabase = FindObjectOfType<UnitPrefabDatabase>();
        }

        Unit unitPrefab = unitPrefabDatabase.GetPrefab(unitID);
        Unit newUnit = Instantiate(unitPrefab);
        units.Add(newUnit);
        newUnit.Grid = this;
        newUnit.Location = location;
        newUnit.GetUnitView().Orientation = orientation;
    }

    public void SpawnCityAt(City city, City prefab, HexCell location)
    {
        try
        {
            city.Grid = this;
            city.Location = location;
        }
        catch(UnityException e)
        {
            Debug.LogError(e.Message);
            Destroy(city);
            return;
        }

        cities.Add(city);
        if(OnCityCreated != null)
        {
            OnCityCreated(city);
        }
    }

	public void RemoveUnit (Unit unit) {
		units.Remove(unit);
		unit.Die();
	}

	public void MakeChildOfColumn (Transform child, int columnIndex) {
		child.SetParent(columns[columnIndex], false);
	}

	public bool CreateMap (int x, int z, bool wrapping) {
		if (x <= 0 || x % HexMetrics.chunkSizeX != 0 || z <= 0 || z % HexMetrics.chunkSizeZ != 0) {
			Debug.LogError("Unsupported map size.");
			return false;
		}

		pathfinder.ClearPath();
		ClearUnits();
		if (columns != null) {
			for (int i = 0; i < columns.Length; i++) {
				Destroy(columns[i].gameObject);
			}
		}

		cellCountX = x;
		cellCountZ = z;
		this.wrapping = wrapping;
		currentCenterColumnIndex = -1;
		HexMetrics.wrapSize = wrapping ? cellCountX : 0;
		chunkCountX = cellCountX / HexMetrics.chunkSizeX;
		chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;
		cellShaderData.Initialize(cellCountX, cellCountZ);
		CreateChunks();
		CreateCells();
		return true;
	}

	void CreateChunks () {
		columns = new Transform[chunkCountX];
		for (int x = 0; x < chunkCountX; x++) {
			columns[x] = new GameObject("Column").transform;
			columns[x].SetParent(transform, false);
		}

		chunks = new HexGridChunk[chunkCountX * chunkCountZ];
		for (int z = 0, i = 0; z < chunkCountZ; z++) {
			for (int x = 0; x < chunkCountX; x++) {
				HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
				chunk.transform.SetParent(columns[x], false);
			}
		}
	}

	void CreateCells () {
		cells = new HexCell[cellCountZ * cellCountX];

		for (int z = 0, i = 0; z < cellCountZ; z++) {
			for (int x = 0; x < cellCountX; x++) {
				CreateCell(x, z, i++);
			}
		}
	}

    public List<Unit> GetUnits()
    {
        return units;
    }

    public List<City> GetCities()
    {
        return cities;
    }

	void ClearUnits () {
		for (int i = 0; i < units.Count; i++) {
			units[i].Die();
		}
		units.Clear();
	}

	void OnEnable () {
		if (!HexMetrics.noiseSource) {
			HexMetrics.noiseSource = noiseSource;
			HexMetrics.InitializeHashGrid(seed);
			HexMetrics.wrapSize = wrapping ? cellCountX : 0;
			ResetVisibility();
		}
	}

	public HexCell GetCell (Ray ray) {
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			return GetCell(hit.point);
		}
		return null;
	}

	public HexCell GetCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		return GetCell(coordinates);
	}

	public HexCell GetCell (HexCoordinates coordinates) {
		int z = coordinates.Z;
		if (z < 0 || z >= cellCountZ) {
			return null;
		}
		int x = coordinates.X + z / 2;
		if (x < 0 || x >= cellCountX) {
			return null;
		}
		return cells[x + z * cellCountX];
	}

	public HexCell GetCell (int xOffset, int zOffset) {
		return cells[xOffset + zOffset * cellCountX];
	}

	public HexCell GetCell (int cellIndex) {
		return cells[cellIndex];
	}

	public void ShowUI (bool visible) {
		for (int i = 0; i < chunks.Length; i++) {
			chunks[i].ShowUI(visible);
		}
	}

	void CreateCell (int x, int z, int i) {
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * HexMetrics.innerDiameter;
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.Index = i;
		cell.ColumnIndex = x / HexMetrics.chunkSizeX;
		cell.ShaderData = cellShaderData;

		if (wrapping) {
			cell.Explorable = z > 0 && z < cellCountZ - 1;
		}
		else {
			cell.Explorable =
				x > 0 && z > 0 && x < cellCountX - 1 && z < cellCountZ - 1;
		}

		if (x > 0) {
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
			if (wrapping && x == cellCountX - 1) {
				cell.SetNeighbor(HexDirection.E, cells[i - x]);
			}
		}
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
				if (x > 0) {
					cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
				}
				else if (wrapping) {
					cell.SetNeighbor(HexDirection.SW, cells[i - 1]);
				}
			}
			else {
				cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
				if (x < cellCountX - 1) {
					cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
				}
				else if (wrapping) {
					cell.SetNeighbor(
						HexDirection.SE, cells[i - cellCountX * 2 + 1]
					);
				}
			}
		}

		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		cell.uiRect = label.rectTransform;

		cell.Elevation = -2; //start all cells as deep ocean

		AddCellToChunk(x, z, cell);
	}

	void AddCellToChunk (int x, int z, HexCell cell) {
		int chunkX = x / HexMetrics.chunkSizeX;
		int chunkZ = z / HexMetrics.chunkSizeZ;
		HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

		int localX = x - chunkX * HexMetrics.chunkSizeX;
		int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
		chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
	}

	public void Save (BinaryWriter writer) {
		writer.Write(cellCountX);
		writer.Write(cellCountZ);
		writer.Write(wrapping);

		for (int i = 0; i < cells.Length; i++) {
			cells[i].Save(writer);
		}

		writer.Write(units.Count);
		for (int i = 0; i < units.Count; i++) {
			units[i].Save(writer);
		}
	}

	public void Load (BinaryReader reader, int header) {
        pathfinder.ClearPath();
        ClearUnits();
		int x = 20, z = 15;
		if (header >= 1) {
			x = reader.ReadInt32();
			z = reader.ReadInt32();
		}
		bool wrapping = header >= 5 ? reader.ReadBoolean() : false;
		if (x != cellCountX || z != cellCountZ || this.wrapping != wrapping) {
			if (!CreateMap(x, z, wrapping)) {
				return;
			}
		}

		bool originalImmediateMode = cellShaderData.ImmediateMode;
		cellShaderData.ImmediateMode = true;

		for (int i = 0; i < cells.Length; i++) {
			cells[i].Load(reader, header);
		}
		for (int i = 0; i < chunks.Length; i++) {
			chunks[i].Refresh();
		}

		if (header >= 2) {
			int unitCount = reader.ReadInt32();
			for (int i = 0; i < unitCount; i++) {
                //loading a game is broken because we can't instnantiate a unit prefab like this?
                //main solution i can think of in my head is to create a dictionary of IDs to prefabs, save the id on save, load the id, instantiate prefab and then load other saved values;
                //Unit unit = new Unit();
                //unit.Load(reader, this);
			}
		}

		cellShaderData.ImmediateMode = originalImmediateMode;
	}

    public void ResetVisibility()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].ResetVisibility();
        }
        for (int i = 0; i < units.Count; i++)
        {
            Unit unit = units[i];
            IncreaseVisibility(unit.Location, unit.visionRange);
        }
    }
    
    //Next few methods are calls to the pathfinder, but other places still reference the grid.

    public void ClearPath()
    {
        pathfinder.ClearPath();
    }

    public List<HexCell> GetPath()
    {
        return pathfinder.GetPath();
    }

    public bool HasPath()
    {
        return pathfinder.HasPath;
    }

    public void FindPath(HexCell fromCell, HexCell toCell, Unit unit)
    {
        pathfinder.FindPath(fromCell, toCell, unit);
    }

    public void IncreaseVisibility(HexCell location, int visionRange)
    {
        pathfinder.IncreaseVisibility(location, visionRange);
    }

    public void DecreaseVisibility(HexCell location, int visionRange)
    {
        pathfinder.DecreaseVisibility(location, visionRange);
    }

    //end methods that are forwarded to the pathfinder.

    public void CenterMap (float xPosition) {
		int centerColumnIndex = (int)
			(xPosition / (HexMetrics.innerDiameter * HexMetrics.chunkSizeX));
		
		if (centerColumnIndex == currentCenterColumnIndex) {
			return;
		}
		currentCenterColumnIndex = centerColumnIndex;

		int minColumnIndex = centerColumnIndex - chunkCountX / 2;
		int maxColumnIndex = centerColumnIndex + chunkCountX / 2;

		Vector3 position;
		position.y = position.z = 0f;
		for (int i = 0; i < columns.Length; i++) {
			if (i < minColumnIndex) {
				position.x = chunkCountX *
					(HexMetrics.innerDiameter * HexMetrics.chunkSizeX);
			}
			else if (i > maxColumnIndex) {
				position.x = chunkCountX *
					-(HexMetrics.innerDiameter * HexMetrics.chunkSizeX);
			}
			else {
				position.x = 0f;
			}
			columns[i].localPosition = position;
		}
	}
}