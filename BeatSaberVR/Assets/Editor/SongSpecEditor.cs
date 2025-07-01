#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// SongSpec SO 에 대한 커스텀 에디터
/// * 샘플링 기능
/// * 블록 생성
/// </summary>
[CustomEditor(typeof(SongSpecSO))]
public class SongSpecEditor : Editor
{
    AudioClip _cachedAudioClip;
    private const float THRESHOLD_GAIN = 2.2f; //임계값

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SongSpecSO songSpec = (SongSpecSO)target; //CustomEditor 자신

        using (new EditorGUILayout.VerticalScope())
        {
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Audio spectrum sampling", EditorStyles.boldLabel);
            }

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                _cachedAudioClip = (AudioClip)EditorGUILayout.ObjectField("Audio clip", _cachedAudioClip, typeof(AudioClip), false);
            }

            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                if(_cachedAudioClip != null)
                {
                    if (GUILayout.Button("Bake peaks"))
                    {
                        List<float> peaks = ExtractPeaks(_cachedAudioClip, songSpec.BPM);
                        songSpec.BakePeaks(_cachedAudioClip, peaks);

                        EditorUtility.SetDirty(songSpec);
                        AssetDatabase.SaveAssets();
                    }
                }
            }

        }
    }

    /// <summary>
    /// 오디오 파형 데이터 추출 작업
    /// (음원 채널 별로 블록 추출)
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    private List<float> ExtractPeaks(AudioClip clip, float bpm)
    {
        int sampleCount = clip.samples;
        int channelCount = clip.channels;
        float[] raw = new float[sampleCount * channelCount];
        clip.GetData(raw, 0);

        //채널들 평균 (모노가 아닐 수도 있으니)
        float[] mono = new float[sampleCount];
        for (int s = 0; s < sampleCount; s++)
        {
            float sum = 0f;
            for (int c = 0; c < channelCount; c++)
            {
                sum += raw[s * channelCount + c];
            }
            mono[s] = sum / channelCount;
        }

        //볼륨 값 확인용 RMS(Root Mean Square)
        float windowSize = 24 * clip.frequency / bpm; //추가 보정값은 테스트하면서 찾기
        int windowCount = sampleCount / Mathf.FloorToInt(windowSize);
        float[] rmsArr = new float[windowCount];
        float meanRms = 0f;

        for (int w = 0; w < windowCount; w++)
        {
            double acc = 0;
            int start = w * Mathf.FloorToInt(windowSize);

            for (int i = 0; i < windowSize; i++)
            {
                float s = mono[start + i];
                acc += s * s; //진폭 늘리기
            }

            float rms = Mathf.Sqrt((float)(acc / windowSize));
            rmsArr[w] = rms;
            meanRms += rms;
        }

        meanRms /= windowCount;

        //표준 편차
        double stdSum = 0;
        for (int w = 0; w < windowCount; w++)
        {
            float d = rmsArr[w] - meanRms;
            stdSum = d * d;
        }
        stdSum /= (double)windowCount;
        float stdRms = Mathf.Sqrt((float)stdSum);

        //임계값 설정 (평균값 + 임계계수 * 표준편차)
        float threshold = meanRms + THRESHOLD_GAIN * stdRms;
        
        //임계값 이상만 추출
        List<float> peaks = new List<float>();
        float windowSec = windowSize / (float)clip.frequency; //한 윈도우 당 시간

        for (int w = 0; w < windowCount; w++)
        {
            if (rmsArr[w] >= threshold)
            {
                float time = (w + 0.5f) * windowSec;
                peaks.Add(time);
            }
        }

        return peaks;
    }
}
#endif