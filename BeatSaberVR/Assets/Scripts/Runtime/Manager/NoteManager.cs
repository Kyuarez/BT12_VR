using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeatSaber.Runtime.Game
{
    [RequireComponent(typeof(AudioSource))]
    public class NoteManager : MonoBehaviour
    {
        [SerializeField] GameObject _notePrefab;
        [SerializeField] Vector2 _spawnRange = new(1f, 1f);
        [SerializeField] Vector2 _spawnRangeOffset = new(0f, 1f);
        [SerializeField] float _preSpawnDelay = 5f;
        [SerializeField] float _deadlineZ = -1f;
        [SerializeField] NoteDissolveAnimation _dissolveAnimation;

        List<Transform> _cachedDeadNotes;
        List<float> _peaks;
        Dictionary<Transform, float> _noteDict;
        AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _noteDict = new Dictionary<Transform, float>();
            _cachedDeadNotes = new List<Transform>();
        }

        private void Start()
        {
            _peaks = GameManager.gameSession.SelectedSongSpec.Peaks;
            StartSpawn();
        }

        public void StartSpawn()
        {
            StartCoroutine(CoPlayAudio());
            StartCoroutine(CoSpawnNotes());
            StartCoroutine(CoMoveNotes());
        }

        /// <summary>
        /// Sabor에서 호출 : Note 파괴
        /// </summary>
        public bool TryHit(Transform note)
        {
            return _noteDict.Remove(note);
        }

        private IEnumerator CoPlayAudio()
        {
            yield return new WaitForSeconds(_preSpawnDelay);
            _audioSource.Play();
        }

        private IEnumerator CoSpawnNotes()
        {
            int index = 0;
            float playTime = _audioSource.clip.length;
            float playSpeed = GameManager.gameSession.playSpeed;

            while (index < _peaks.Count) 
            {
                //현재 프레임에서 생성할 수 있는 노드 다생성
                while (true)
                {
                    if(_audioSource.time >= _peaks[index])
                    {
                        GameObject noteObj = Instantiate(_notePrefab);
                        int x = Random.Range(-1, 2);
                        int y = Random.Range(-1, 2);
                        noteObj.transform.position = new Vector3(x * _spawnRange.x / 2f + _spawnRangeOffset.x,
                                                                 y * _spawnRange.y / 2f + _spawnRangeOffset.y,
                                                                 _peaks[index] * playSpeed + _preSpawnDelay * playSpeed);
                        _noteDict.Add(noteObj.transform, _peaks[index]);
                        index++;
                    }
                    else
                    {
                        break;
                    }   
                }

                yield return null;
            }
        }

        private IEnumerator CoMoveNotes()
        {
            float playSpeed = GameManager.gameSession.playSpeed;

            while(true)
            {
                foreach (KeyValuePair<Transform, float> noteKv in _noteDict)
                {
                    Vector3 position = noteKv.Key.position;
                    position.z = (noteKv.Value - _audioSource.time + _preSpawnDelay) * playSpeed;
                    noteKv.Key.position = position;

                    if(position.z < _deadlineZ)
                    {
                        Renderer renderer = noteKv.Key.GetComponent<Renderer>();
                        //TODO : Disolve effect
                        _dissolveAnimation.PlayDissolve(renderer, () => Destroy(renderer.gameObject));
                        _cachedDeadNotes.Add(noteKv.Key);
                    }
                }

                for (int i = 0; i < _cachedDeadNotes.Count; i++)
                {
                    _noteDict.Remove(_cachedDeadNotes[i]);
                }
                yield return null;
            }
        }
    }
}