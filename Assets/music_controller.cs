using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class music_controller : MonoBehaviour
{
    public AudioClip[] songs;
    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = songs[0];
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!source.isPlaying) {
            int next = Random.Range(0, songs.Length);
            source.clip = songs[next];
            source.Play();
        }
    }
}
