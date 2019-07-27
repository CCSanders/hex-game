using System.Collections;
using System.Collections.Generic;
using System.Linq;

static public class BuildingDatabase
{
    static Dictionary<int, BuildingBlueprint> buildings;

    static BuildingDatabase()
    {
        buildings = new Dictionary<int, BuildingBlueprint>();
        LoadBuildingBlueprints();
    }

    static public BuildingBlueprint[] GetListOfBuildings()
    {
        return buildings.Values.ToArray();
    }

    //im not sure if this method is bad
    static public BuildingBlueprint[] GetListOfBuildingsWithBlacklist(List<int> idsNotAllowed)
    {
        List<BuildingBlueprint> list = new List<BuildingBlueprint>();
        foreach(int i in buildings.Keys)
        {
            if (idsNotAllowed.Contains(i))
            {
                continue;
            }
            else
            {
                list.Add(buildings[i]);
            }
        }

        return list.ToArray();
    }

    static public BuildingBlueprint GetBuildingByID(int id)
    {
        if (!buildings.ContainsKey(id))
        {
            return null;
        }
        
        //need to check for exceptions
        return buildings[id];
    }

    static void LoadBuildingBlueprints()
    {
        //could possibly load from xml file

        BuildingBlueprint bb;
        BuildingYieldChange yield;
        int id = 0;

        //culture test
        bb = new BuildingBlueprint(id, "Monument");
        yield = new BuildingYieldChange(BuildingYieldChange.YieldType.YIELD_CULTURE, 2);
        bb.AddYieldChange(yield);
        buildings.Add(id++, bb);

        //food test
        bb = new BuildingBlueprint(id, "Granary");
        yield = new BuildingYieldChange(BuildingYieldChange.YieldType.YIELD_FOOD, 2);
        bb.AddYieldChange(yield);
        buildings.Add(id++, bb);

        //gold test
        bb = new BuildingBlueprint(id, "Market");
        yield = new BuildingYieldChange(BuildingYieldChange.YieldType.YIELD_GOLD, 2);
        bb.AddYieldChange(yield);
        yield = new BuildingYieldChange(BuildingYieldChange.YieldType.YIELD_GOLD_PERCENT, .50f);
        bb.AddYieldChange(yield);
        buildings.Add(id++, bb);

        //science test
        bb = new BuildingBlueprint(id, "Library");
        yield = new BuildingYieldChange(BuildingYieldChange.YieldType.YIELD_SCIENCE, 2);
        bb.AddYieldChange(yield);
        buildings.Add(id++, bb);

        //production test
        bb = new BuildingBlueprint(id, "Workshop");
        yield = new BuildingYieldChange(BuildingYieldChange.YieldType.YIELD_PRODUCTION, 2);
        bb.AddYieldChange(yield);
        buildings.Add(id++, bb);
    }
}
