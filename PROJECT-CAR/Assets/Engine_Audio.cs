using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine_Audio : MonoBehaviour
{
    AudioSource audioSound;
    [SerializeField] float audioPitch = 1; 
    
    // Start is called before the first frame update
    void Start()
    {
        audioSound = GetComponent<AudioSource>();
        //audioSound.pitch = audioPitch; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
