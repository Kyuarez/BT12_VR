using BeatSaber.Runtime.Game;
using BeatSabor;
using EzySlice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Sabor : MonoBehaviour
{
    [SerializeField] LayerMask _sliceableMask;
    [SerializeField] Transform _plane; //잘리는 기준
    [SerializeField] Vector3 _prevPos;
    [SerializeField] Material _hullMaterial;
    [SerializeField] NoteManager _noteManager;

    private void Awake()
    {
        _prevPos = transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 move = transform.position - _prevPos;
        
        if (move.sqrMagnitude > 0.005f)
        {
            //이거 right 하면 알아서 방향 설정함.
            _plane.right = move.normalized;
        }

        _prevPos = transform.position;
    }

    private void Slice(GameObject target)
    {
        Vector3 sliceNormal = _plane.up;
        Vector3 _slicePoint = _plane.position;
        SlicedHull hull = target.Slice(_slicePoint, sliceNormal, _hullMaterial);

        if (hull != null)
        {
            if (_noteManager.TryHit(target.transform))
            {
                GameObject upper = hull.CreateUpperHull(target, _hullMaterial);
                HullBehaviour upperHB = upper.AddComponent<HullBehaviour>();
                upperHB.velocity = sliceNormal + Vector3.forward;
                upperHB.normal = sliceNormal;

                GameObject lower = hull.CreateLowerHull(target, _hullMaterial);
                HullBehaviour lowerHB = lower.AddComponent<HullBehaviour>();
                lowerHB.velocity = -sliceNormal + Vector3.forward;
                lowerHB.normal = -sliceNormal;

                Destroy(target);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if((1 << other.gameObject.layer & _sliceableMask) == 0)
        {
            return;
        }

        Slice(other.gameObject);
    }
}

