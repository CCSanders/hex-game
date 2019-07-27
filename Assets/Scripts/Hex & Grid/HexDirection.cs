using UnityEngine;

public enum HexDirection {
	NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions {

	public static HexDirection Opposite (this HexDirection direction) {
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}

	public static HexDirection Previous (this HexDirection direction) {
		return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
	}

	public static HexDirection Next (this HexDirection direction) {
		return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
	}

	public static HexDirection Previous2 (this HexDirection direction) {
		direction -= 2;
		return direction >= HexDirection.NE ? direction : (direction + 6);
	}

	public static HexDirection Next2 (this HexDirection direction) {
		direction += 2;
		return direction <= HexDirection.NW ? direction : (direction - 6);
	}

    public static HexDirection GetRandom()
    {
        float rng = Random.value;
        if(rng < .17f)
        {
            return HexDirection.NW;
        }
        else if(rng >= .17f && rng < .34f)
        {
            return HexDirection.W;
        }
        else if (rng >= .34f && rng < .50f)
        {
            return HexDirection.SW;
        }
        else if(rng >= .50f && rng < .67f)
        {
            return HexDirection.SE;
        }
        else if (rng >= .67f && rng < .83f)
        {
            return HexDirection.E;
        }
        else
        {
            return HexDirection.NE;
        }
    }
}