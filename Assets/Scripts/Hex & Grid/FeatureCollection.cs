using UnityEngine;

[System.Serializable]
public struct FeatureCollection {

	public Transform[] prefabs;

	public Transform Pick (float choice) {
		return prefabs[(int)(choice * prefabs.Length)];
	}
}