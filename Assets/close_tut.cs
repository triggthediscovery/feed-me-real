using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class close_tut : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Fire1") > 0.5f ||
            Input.GetAxis("Fire2") > 0.5f ||
            Input.GetAxis("Fire3") > 0.5f ||
            Input.GetAxis("Jump") > 0.5f ||
            Input.GetAxis("Cancel") > 0.5f) {
            gameObject.SetActive(false);
        }
    }
}
