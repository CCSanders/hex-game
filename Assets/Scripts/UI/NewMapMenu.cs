using UnityEngine;

public class NewMapMenu : MonoBehaviour {

	public HexGrid hexGrid;

	public MapGenerator mapGenerator;

	bool generateMaps = true;

	bool wrapping = true;

	public void ToggleMapGeneration (bool toggle) {
		generateMaps = toggle;
	}

	public void ToggleWrapping (bool toggle) {
		wrapping = toggle;
	}

	public void Open () {
		gameObject.SetActive(true);
		MapCamera.Locked = true;
	}

	public void Close () {
		gameObject.SetActive(false);
		MapCamera.Locked = false;
	}

    //mimics civ 6 tiny map (2100 tiles vs 2280)
	public void CreateSmallMap () {
		CreateMap(60, 35);
	}

    //mimics civ 6 standard (4400 vs 4536)
	public void CreateMediumMap () {
		CreateMap(80, 55);
	}

    //mimics civ 6 large (5700 vs 5760)
	public void CreateLargeMap () {
        CreateMap(95, 60);
	}

	void CreateMap (int x, int z) {
		if (generateMaps) {
			mapGenerator.GenerateMap(x, z, wrapping);
		}
		else {
			hexGrid.CreateMap(x, z, wrapping);
		}
		MapCamera.ValidatePosition();
		Close();
	}
}