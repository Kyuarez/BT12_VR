using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Sabor : MonoBehaviour
{
    [SerializeField] LayerMask _sliceableMask;

    private void OnTriggerEnter(Collider other)
    {
        if((1 << other.gameObject.layer & _sliceableMask) == 0)
        {
            return;
        }


    }
}

