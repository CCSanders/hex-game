using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/*
 * Something important that im stuck on: 
 * 
 * since units are represented as game objects in the game, and are generally instantiated as prefabs,
 * i cannot think of a good solution to a unit database for building jobs that also corresponds to 
 * the prefabs. The solution that i am implementing is that this will still be a static database for the
 * blueprints, but the ids are going to match up in the prefab database that will be attached to the 
 * game manager object. 
 */
static public class UnitDatabase
{
    static Dictionary<int, UnitBlueprint> units;

    static UnitDatabase()
    {
        units = new Dictionary<int, UnitBlueprint>();
        LoadUnitBlueprints();
    }

    static public UnitBlueprint[] GetListOfUnits()
    {
        return units.Values.ToArray();
    }

    static void LoadUnitBlueprints()
    {
        UnitBlueprint ub;
        int id = 0;

        ub = new UnitBlueprint(id, "Settler");
        units.Add(id++, ub);

        ub = new UnitBlueprint(id, "Warrior");
        units.Add(id++, ub);
    }
}
