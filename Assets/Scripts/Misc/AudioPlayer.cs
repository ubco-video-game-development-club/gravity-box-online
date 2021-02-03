using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AudioPlayerClip 
{
    public string name;
    public AudioClip clip;
}

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioPlayerClip[] clips;
    [SerializeField] private AudioSource source;
    private Dictionary<string, AudioClip> indices = new Dictionary<string, AudioClip>();

    void Awake() 
    {
        if(source == null) 
        {
            Debug.LogWarning("Source was not set. Getting component from self.");
            source = GetComponent<AudioSource>();
        }

        InitializeClips();
    }

    public void PlayClip(string name)
    {
        if(indices.ContainsKey(name))
        {
            source.PlayOneShot(indices[name]);
        } else
        {
            Debug.LogError($"Clip {name} does not exist.");
        }
    }

    private void InitializeClips() 
    {
        //If there are no audio clips provided, throw an error.
        if(clips == null || clips.Length == 0) 
        {
            Debug.LogError("No clips provided to audio player.");
        } else //Otherwise, populate our indices
        {
            foreach(AudioPlayerClip clip in clips) 
            {
                if(indices.ContainsKey(clip.name)) 
                {
                    Debug.LogError($"Duplicate named audio clip ({clip.name})");
                    continue;
                }

                indices.Add(clip.name, clip.clip);
            }
        }
    }
}
