using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCityButton : MonoBehaviour
{
    public void BuildCity()
    {
        HexGrid grid = FindObjectOfType<HexGrid>();
        SelectionController sc = FindObjectOfType<SelectionController>();

        City newCity = Instantiate(grid.cityPrefab);
        grid.SpawnCityAt(newCity, grid.cityPrefab, sc.SelectedUnit.Location);

        grid.RemoveUnit(sc.SelectedUnit);
        sc.SelectedUnit = null;
    }
}
