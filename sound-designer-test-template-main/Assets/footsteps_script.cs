using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footsteps_script : MonoBehaviour
{
    public AK.Wwise.Event footEvent;

  
    
    void footsteps()
    {
        footEvent.Post(gameObject);
    }
}
