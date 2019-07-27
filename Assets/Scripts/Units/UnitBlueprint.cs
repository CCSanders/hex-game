public class UnitBlueprint
{
    public readonly int id;
    public string name;
    public int productionCost = 3;

    public UnitBlueprint(int id, string name)
    {
        this.id = id;
        this.name = name;
    }

    public void OnUnitComplete(City city)
    {
        if (!city.Location.HasUnit())
        {
            city.Grid.SpawnUnitAt(id, city.Location, UnityEngine.Random.Range(0f, 360f));
        }

        city.SetCurrentBuildingJob(null);
    }
}