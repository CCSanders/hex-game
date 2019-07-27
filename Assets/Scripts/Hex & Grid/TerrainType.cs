using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainType
{
    //terrain types
    public const int Sand = 0;
    public const int Grass = 1;
    public const int Rock = 2;
    public const int Dirt = 3;
    public const int Snow = 4;

    //elevation types
    public const int Ocean = -2;
    public const int Coast = -1;
    public const int Flat = 0;
    public const int Hills = 1;
    public const int Mountain = 2;

    //plant level
    public const int NoPlants = 0;
    public const int LightPlants = 1;
    public const int Forest = 2;
    public const int Jungle = 3; //happens with high moisture and high heat
}
