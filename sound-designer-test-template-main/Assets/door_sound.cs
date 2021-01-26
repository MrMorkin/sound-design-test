using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door_sound : MonoBehaviour
{

     public AK.Wwise.Event dooropen_Event;
     public AK.Wwise.Event doorclose_Event;
     public AK.Wwise.Event bottom_Event;
   Animator AnimatorController;

    bool StartSound;

    // Start is called before the first frame update
    void Start()
    {
        AnimatorController = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
         if (other.CompareTag("Player"))
          { 
        AnimatorController.SetBool("isOpen", true);
        dooropen_Event.Post(gameObject);
         bottom_Event.Post(gameObject);
        print("PlaySound");
        StartSound = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            AnimatorController.SetBool("isOpen", false);
            doorclose_Event.Post(gameObject);

            StartSound = true;
        }
    }


}
