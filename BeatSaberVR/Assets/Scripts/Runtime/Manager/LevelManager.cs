using UnityEngine;

namespace BeatSaber.Runtime.Game
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] GameObject _mapUnitPrefab;
        [SerializeField] int _initSpawnCount = 20;
        [SerializeField] float _mapUnitLengthZ = 2f;
        [SerializeField] float _baseZOffset = -10f;
        [SerializeField] float _speed = 3f;

        Transform[] _mapUnits;
        int[] _unitIndices;

        private void Awake()
        {
            GameManager.gameSession.playSpeed = _speed;
        }

        private void Start()
        {
            _mapUnits = new Transform[_initSpawnCount];
            _unitIndices = new int[_initSpawnCount];

            for (int i = 0; i < _initSpawnCount; i++)
            {
                GameObject mapUnit = Instantiate(_mapUnitPrefab, transform);
                _mapUnits[i] = mapUnit.transform;
                _unitIndices[i] = i;

                float z = _baseZOffset + _unitIndices[i] * _mapUnitLengthZ;
                _mapUnits[i].localPosition = new Vector3(0, 0, z);
            }
        }

        private void Update()
        {
            float scrollZ = Time.time * _speed;
            float recycleThreshold = _baseZOffset - _mapUnitLengthZ;

            for (int i = 0; i < _mapUnits.Length; i++)
            {
                float z = _baseZOffset + _unitIndices[i] * _mapUnitLengthZ - scrollZ;

                if (z < recycleThreshold)
                {
                    _unitIndices[i] += _initSpawnCount;
                    z = _baseZOffset + _unitIndices[i] * _mapUnitLengthZ - scrollZ;
                }

                _mapUnits[i].localPosition = new Vector3(0, 0, z);
            }
        }

    }
}
