using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompletedBuildingItem : MonoBehaviour
{
    //this is where you'd have code for tool tip hovering, possibly specialist control and whatnot

    public Text nameText;
    private BuildingBlueprint myBuilding;

    public void SetBuilding(BuildingBlueprint newBuilding)
    {
        this.myBuilding = newBuilding;
        nameText.text = newBuilding.name;
    }
}
