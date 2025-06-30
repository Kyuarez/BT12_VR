#if UNITY_EDITOR
using BeatSaber.Runtime.Game;
using UnityEditor;
using UnityEngine;

public class GameSessionWindow : EditorWindow
{
    GameSessionSO _cachedTarget;
    SerializedObject _cachedTargetSerialized;

    [MenuItem("Window/GameSession")]
    static void Open() => GetWindow<GameSessionWindow>("Game Session");

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    void OnPlayModeStateChanged(PlayModeStateChange change)
    {
        _cachedTarget = GameManager.gameSession;
        _cachedTargetSerialized = new SerializedObject( _cachedTarget);
        _cachedTargetSerialized.Update();
        Repaint();
    }

    private void OnGUI()
    {
        //�÷��� ��忡���� ���� ���
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Only can monitor game session on playmode", MessageType.Warning);
        }

        //���� ���� ��Ȯ�� ����� ������ Ȯ��
        if(_cachedTarget == null)
        {
            if (GameManager.gameSession != null) 
            {
                _cachedTarget = GameManager.gameSession;
                _cachedTargetSerialized = new SerializedObject(_cachedTarget);
            }
            else
            {
                EditorGUILayout.HelpBox("GameSession is not instantiated yet", MessageType.Warning);
                return;
            }

            _cachedTargetSerialized.Update();
            EditorGUILayout.PropertyField(_cachedTargetSerialized.FindProperty("SelectedSongSpec"));
            _cachedTargetSerialized.ApplyModifiedProperties();
        }
    }
}
#endif