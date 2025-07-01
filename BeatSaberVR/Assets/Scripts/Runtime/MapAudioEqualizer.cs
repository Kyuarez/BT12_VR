using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MapAudioEqualizer : MonoBehaviour
{
    [SerializeField] GameObject _unitPrefab;
    [SerializeField] float _unitZLength = 1f;
    [SerializeField] Vector3 _spawningOffset;
    [SerializeField, Range(64, 512)] int _spawnCount = 128;
    [SerializeField] float _scaleYMax = 10f;
    [SerializeField] float _gamma = 2f;
    [SerializeField] bool _doShuffle = true;

    AudioSource _audioSource;
    Transform[] _spawnedUnits;
    float[] _spectrumDatum;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _spawnedUnits = new Transform[_spawnCount];
        _spectrumDatum = new float[_spawnCount];

        for (int i = 0; i < _spawnCount; i++)
        {
            Transform spawned = Instantiate(_unitPrefab).transform;
            spawned.position = _spawningOffset + new Vector3(0, 0, i * _unitZLength);
            _spawnedUnits[i] = spawned;
        }
    }

    private void Update()
    {
        _audioSource.GetSpectrumData(_spectrumDatum, 0, FFTWindow.Hamming);

        if (_doShuffle) 
        {
            _spectrumDatum.Shuffle();
        }

        //manipulates y scale 
        _spectrumDatum[0] = _spectrumDatum[_spawnCount - 1] = 0f;
        _spawnedUnits[0].localScale = _spawnedUnits[_spawnCount - 1].localScale = new Vector3(1f, 0.01f, 1f);

        for (int i = 1; i < _spawnCount - 1; i++)
        {
            _spectrumDatum[i] = Mathf.Log(1f + _spectrumDatum[i], 2) * _scaleYMax; 
            _spectrumDatum[i] = Mathf.Pow(_spectrumDatum[i], _gamma); //½ºÆåÆ®·³ ÁõÆø
            _spectrumDatum[i] = (_spectrumDatum[i - 1] + _spectrumDatum[i] + _spectrumDatum[i + 1]) / 3f;
            float prevScaleY = _spawnedUnits[i].localScale.y;
            _spawnedUnits[i].localScale = new Vector3(1f, Mathf.Lerp(prevScaleY, _spectrumDatum[i], Time.deltaTime * 30f), 1f);
        }
    }

}
