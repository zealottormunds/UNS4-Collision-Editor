using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    Vector2 rotation = new Vector2();

    void Update()
    {
        float camx = Input.GetAxis("Mouse Y");
        float camy = Input.GetAxis("Mouse X");

        rotation.x -= camx * 100 * Time.deltaTime;
        rotation.y += camy * 100 * Time.deltaTime;
        rotation.x = Mathf.Clamp(rotation.x, -90, 90);

        float movx = Input.GetAxis("Horizontal");
        float movy = Input.GetAxis("Vertical");

        transform.position += ((movx * transform.right) * Time.deltaTime * 20 + (movy * transform.forward) * Time.deltaTime * 20);
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
    }
}
