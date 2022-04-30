using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegoColissionSound : MonoBehaviour
    
{
    [SerializeField] private string EventName = "default";
    [SerializeField] private int MinimumSpeed = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > MinimumSpeed)
        {

            AkSoundEngine.PostEvent(EventName, gameObject);
            // Debug.Log("Playing sounds on: " + gameObject.name);
            // if (!collision.gameObject.CompareTag("Floor")) AkSoundEngine.PostEvent(EventName, gameObject);
        }
    }
}
