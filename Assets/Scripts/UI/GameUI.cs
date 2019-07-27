using UnityEngine;

public class GameUI : MonoBehaviour {

	public HexGrid grid;

    public GameObject actionButton;
    public GameObject cityUI;
    public GameObject unitPanelUI;
    public GameObject mapEditor;

    private bool isEditorMode = false;

	HexCell currentCell;

	Unit selectedUnit;
    City selectedCity;

    private void Start()
    {
        if (mapEditor)
        {
            isEditorMode = true;
        }
        else
        {
            isEditorMode = false;
        }
    }

    public void SetEditMode (bool toggle) {
		enabled = !toggle;
		grid.ShowUI(!toggle);
		grid.ClearPath();
		if (toggle) {
			Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
		}
		else {
			Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
		}
	}

    public void OpenCityUI(City selectedCity)
    {
        actionButton.SetActive(false);
        unitPanelUI.SetActive(false);
        if (isEditorMode)
        {
            mapEditor.SetActive(false);
        }

        if (selectedCity)
        {
            cityUI.SetActive(true);
            cityUI.GetComponent<CityScreen>().Open(selectedCity);
            this.selectedCity = selectedCity;
        }
        else
        {
            this.selectedCity = null;
            return;
        }
    }

    public void CloseCityUI()
    {
        cityUI.GetComponent<CityScreen>().OnClose();
        cityUI.SetActive(false);
        this.selectedCity = null;

        actionButton.SetActive(true);
        unitPanelUI.SetActive(true);
        if (isEditorMode)
        {
            mapEditor.SetActive(true);
        }
    }
}