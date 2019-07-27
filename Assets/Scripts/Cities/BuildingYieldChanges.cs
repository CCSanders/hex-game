using System.Collections;
using System.Collections.Generic;

public class BuildingYieldChange
{
    public enum YieldType
    {
        YIELD_FOOD, YIELD_FOOD_PERCENT,
        YIELD_CULTURE, YIELD_CULTURE_PERCENT,
        YIELD_GOLD, YIELD_GOLD_PERCENT,
        YIELD_SCIENCE, YIELD_SCIENCE_PERCENT,
        YIELD_PRODUCTION, YIELD_PRODUCTION_PERCENT
    };

    public YieldType yieldType;
    public float byHowMuch;

    public BuildingYieldChange(YieldType yieldType, float byHowMuch)
    {
        this.yieldType = yieldType;
        this.byHowMuch = byHowMuch;
    }
}
