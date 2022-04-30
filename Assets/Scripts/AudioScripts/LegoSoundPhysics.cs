using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegoSoundPhysics : MonoBehaviour
{
    // private float mass;     //Mass is set to the rigidbody mass
   // private float maxMassVal = 100; //for now
    //private float RTPCMassVal = 0.0f; //number between 0-100
    private float speed; //set when collision is happening
    private float maxSpeedVal = 20; // for now
    private float RTPCSpeedVal = 0.0f; //number between 0-100 

    [Header("Minimum Collision Speed")]
    [SerializeField]
    private int MinimumSpeed = 2;

    [Header("COLLISION SFX")]
    [SerializeField]
    public AK.Wwise.Event soundEffectEvent;

    void Start()
    {
       // mass = gameObject.GetComponent<Rigidbody>().mass;  //Mass is set to the rigidbody mass
       // RTPCMassVal = (mass / maxMassVal) * 100;
       // AkSoundEngine.SetRTPCValue("RTPC_ObjectMass", RTPCMassVal, gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        //Gets speed of collision and sets RTPC_ObjectSpeed
        speed = collision.relativeVelocity.magnitude;
        RTPCSpeedVal = (speed / maxSpeedVal) * 100;
        AkSoundEngine.SetRTPCValue("RTPC_BrickSpeed", RTPCSpeedVal, gameObject);
       // Debug.Log("RTPCVALUE" + RTPCSpeedVal);
        //if SoundMaterial exist set switch
        //collision.gameObject.GetComponent<SoundMaterial>()?.material.SetValue(gameObject);

        //play sound effect
        if (collision.relativeVelocity.magnitude > MinimumSpeed)
            soundEffectEvent.Post(gameObject);

        // Debug.Log("Switch to material from: "+collision.gameObject.name+ " with RTPCMass: "+RTPCMassVal+" and RTPCSpeed: "+RTPCSpeedVal);
    }
}