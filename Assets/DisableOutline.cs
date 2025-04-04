using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOutline : MonoBehaviour
{
    private float delay = 0.01f;
    //Outline outline;

    void Start()
    {
      //  outline = GetComponent<Outline>();
        Invoke("OutlineDisabled", delay);
    }
    
    void OutlineDisabled()
    {
        //outline.enabled = false;
    }

}
