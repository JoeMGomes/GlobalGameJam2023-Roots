using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CustomObjects/Tool", order = 1)]
public class Tool : ScriptableObject
{
    public string Name;
    public GameObject toolPrefab;
    public Vector3 InitRotation;
    public AudioClip[] audioClips;
    public ParticleSystem EffectsPS;
    private GameObject _instance;

    public GameObject Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Instantiate(toolPrefab, Vector3.zero, Quaternion.Euler(InitRotation));
                _instance.layer = LayerMask.NameToLayer("Tools");
                for(int i = 0; i< _instance.transform.childCount; i++)
                {
                    _instance.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Tools");
                }
            }
            return _instance;
        }
    }

    public void Init()
    {
        if (toolPrefab == null)
            Debug.LogError("NO PREFAB IN TOOL");

        if (audioClips.Length == 0)
            Debug.LogError("NO Sounds IN TOOL");

        EffectsPS = Instance.GetComponentInChildren<ParticleSystem>();
    }

    
    public void StartUseTool()
    {
        EffectsPS.Play();

    }

    internal void StopUseTool()
    {
        EffectsPS.Stop();
    }

    public AudioClip GetRandomAudio()
    {
        return audioClips[Random.Range(0, audioClips.Length)];
    }


}
