using System.Collections;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CustomObjects/Tool", order = 1)]
public class Tool : ScriptableObject
{
    public string Name;
    public GameObject toolPrefab;
    public Vector3 InitRotation;
    public AudioSource AudioSource = new AudioSource();
    public AudioClip[] audioClips;

    private GameObject _instance;

    public GameObject Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Instantiate(toolPrefab, Vector3.zero, Quaternion.Euler(InitRotation));
                _instance.layer = LayerMask.NameToLayer("Tools");
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (toolPrefab == null)
            Debug.LogError("NO PREFAB IN TOOL");

        if (audioClips.Length == 0)
            Debug.LogError("NO Sounds IN TOOL");
    }

    
    public void UseTool()
    {
        
    }

    public AudioClip GetRandomAudio()
    {
        return audioClips[Random.Range(0, audioClips.Length)];
    }
}
