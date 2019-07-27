using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private MapCamera camera;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!camera)
        {
            camera = FindObjectOfType<MapCamera>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles);
    }
}
