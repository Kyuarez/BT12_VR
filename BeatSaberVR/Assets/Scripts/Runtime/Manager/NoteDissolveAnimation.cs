using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteDissolveAnimation : MonoBehaviour
{
    private readonly int DISSOLVE_ID = Shader.PropertyToID("_Dissolve");
    MaterialPropertyBlock _block;

    [SerializeField] float _duration = 2.0f;

    private void Awake()
    {
        _block = new MaterialPropertyBlock();
    }

    public void PlayDissolve(Renderer renderer, Action onCompleted = null)
    {
        StartCoroutine(CoDissolve(renderer, onCompleted));
    }

    IEnumerator CoDissolve(Renderer renderer, Action onCompleted)
    {
        float animationSpeed = 1f / _duration;
        float dissolve = 0f;
        float elapsedTime = 0f;

        while (dissolve < 1f)
        {
            dissolve = elapsedTime + animationSpeed;
            elapsedTime += Time.deltaTime;

            renderer.GetPropertyBlock(_block);
            _block.SetFloat(DISSOLVE_ID, dissolve);
            renderer.SetPropertyBlock(_block);
            //renderer.material.SetFloat는 호출 당 새로운 인스턴스 생성
            yield return null;
        }

        onCompleted?.Invoke();

    }
}
