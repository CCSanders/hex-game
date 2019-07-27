using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityNamePlate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        rectTransform = GetComponent<RectTransform>();
        mapCamera = FindObjectOfType<MapCamera>();
    }

    public City myCity;

    public Vector3 screenPositionOffset = new Vector3(0, 30, 0);
    public Vector3 worldPositionOffset = new Vector3(0, .8f, 0);
    public float minSize = .8f;
    public float maxSize = 1.5f;

    RectTransform rectTransform;

    private Camera mainCamera;
    private MapCamera mapCamera;

    // this happens in late update because its jittery in update (since camera movement would happen simultaneously) 
    void LateUpdate()
    {
        if (!myCity)
        {
            //if the target / city is destroyed, then destroy itself 
            Destroy(gameObject);
            return;
        }

        AdjustTransform();
    }

    public void OnClick()
    {
        FindObjectOfType<GameUI>().OpenCityUI(myCity);
    }

    void AdjustTransform()
    {
        //adjust position
        Vector3 screenPosOfTarget = mainCamera.WorldToScreenPoint(myCity.transform.position + worldPositionOffset);
        rectTransform.anchoredPosition = screenPosOfTarget + screenPositionOffset;

        //adjust size
        float scale = Mathf.Lerp(minSize, maxSize, mapCamera.GetZoom());
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
