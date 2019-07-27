using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityNamePlateManager : MonoBehaviour
{
    private void Start()
    {
        FindObjectOfType<HexGrid>().OnCityCreated += CreateCityNameplate;
    }

    public GameObject CityNameplatePrefab;

    public void CreateCityNameplate(City city)
    {
        GameObject nameGO = Instantiate(CityNameplatePrefab, this.transform);
        nameGO.GetComponent<CityNamePlate>().myCity = city;
        nameGO.GetComponentInChildren<Text>().text = city.cityName;
    }
}
