using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CustomObjects/SoundArray", order = 1)]
public class SoundArray : ScriptableObject
{
    public AudioClip[] audioClips;
    public AudioClip GetRandom()
    {
        return audioClips[Random.Range(0, audioClips.Length)];
    }
}