using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISongSelector : MonoBehaviour
{
    [Serializable]
    class SongInfo
    {
        public string songTitle;
        public GameObject songCard;
    }

    [SerializeField] SongInfo[] _songInfos;
    [SerializeField] private TextMeshProUGUI _songTitle;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _previousButton;
    [SerializeField] private Button _playButton;

    private int _selectedIndex;

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            _songInfos[_selectedIndex].songCard.SetActive(false);
            _selectedIndex = value;
            _songInfos[_selectedIndex].songCard.SetActive(true);
            _songTitle.text = _songInfos[_selectedIndex].songTitle;
        }
    }

    private void Start()
    {
        for (int i = 0; i < _songInfos.Length; i++)
        {
            _songInfos[i].songCard.SetActive(i == _selectedIndex);
        }

        _songTitle.text = _songInfos[_selectedIndex].songTitle;
    }

    private void OnEnable()
    {
        _previousButton.onClick.AddListener(OnClickPreviousButton);
        _nextButton.onClick.AddListener(OnClickNextButton);
        _playButton.onClick.AddListener(OnClickPlayButton);
    }

    private void OnDisable()
    {
        _previousButton.onClick.RemoveListener(OnClickPreviousButton);
        _nextButton.onClick.RemoveListener(OnClickNextButton);
        _playButton.onClick.RemoveListener(OnClickPlayButton);
    }

    public void OnClickPreviousButton()
    {
        SelectedIndex = (_selectedIndex + _songInfos.Length - 1) % _songInfos.Length;
    }
    public void OnClickNextButton()
    {
        SelectedIndex = (_selectedIndex + 1) % _songInfos.Length;
    }
    public void OnClickPlayButton()
    {
        SceneManager.LoadScene("InGame");
    }
}
