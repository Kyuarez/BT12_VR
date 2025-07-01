#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// SongSpec SO �� ���� Ŀ���� ������
/// * ���ø� ���
/// * ��� ����
/// </summary>
[CustomEditor(typeof(SongSpecSO))]
public class SongSpecEditor : Editor
{
    AudioClip _cachedAudioClip;
    private const float THRESHOLD_GAIN = 2.2f; //�Ӱ谪

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SongSpecSO songSpec = (SongSpecSO)target; //CustomEditor �ڽ�

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
    /// ����� ���� ������ ���� �۾�
    /// (���� ä�� ���� ��� ����)
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    private List<float> ExtractPeaks(AudioClip clip, float bpm)
    {
        int sampleCount = clip.samples;
        int channelCount = clip.channels;
        float[] raw = new float[sampleCount * channelCount];
        clip.GetData(raw, 0);

        //ä�ε� ��� (��밡 �ƴ� ���� ������)
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

        //���� �� Ȯ�ο� RMS(Root Mean Square)
        float windowSize = 24 * clip.frequency / bpm; //�߰� �������� �׽�Ʈ�ϸ鼭 ã��
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
                acc += s * s; //���� �ø���
            }

            float rms = Mathf.Sqrt((float)(acc / windowSize));
            rmsArr[w] = rms;
            meanRms += rms;
        }

        meanRms /= windowCount;

        //ǥ�� ����
        double stdSum = 0;
        for (int w = 0; w < windowCount; w++)
        {
            float d = rmsArr[w] - meanRms;
            stdSum = d * d;
        }
        stdSum /= (double)windowCount;
        float stdRms = Mathf.Sqrt((float)stdSum);

        //�Ӱ谪 ���� (��հ� + �Ӱ��� * ǥ������)
        float threshold = meanRms + THRESHOLD_GAIN * stdRms;
        
        //�Ӱ谪 �̻� ����
        List<float> peaks = new List<float>();
        float windowSec = windowSize / (float)clip.frequency; //�� ������ �� �ð�

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