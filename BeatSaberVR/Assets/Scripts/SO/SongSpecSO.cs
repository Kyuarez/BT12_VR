using UnityEngine;

[CreateAssetMenu(menuName = "Music/SongSpec", fileName = "SongSpec_")]
public class SongSpecSO : ScriptableObject
{
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public float BPM { get; private set; }

}
