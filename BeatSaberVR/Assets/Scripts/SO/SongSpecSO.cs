using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Music/SongSpec", fileName = "SongSpec_")]
public class SongSpecSO : ScriptableObject
{
    [field: SerializeField] public string Title { get; private set; }
    [field: SerializeField] public float BPM { get; private set; }
    [field: SerializeField] public AudioClip AudioClip { get; private set; }
    [field: SerializeField] public List<float> Peaks { get; private set; }

    public void BakePeaks(AudioClip clip, List<float> peaks)
    {
        Peaks = peaks;
        AudioClip = clip;
    }
}
