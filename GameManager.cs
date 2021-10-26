using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    int _score;
    const string _highScoreKey = "highscore";

    public Text _menuHighScoreText;

    public Text _scoreText;
    public Text _highScoreText1;

    public Text _finalScoreText;
    public Text _highScoreText;

    public int Score
    {
        get { return _score; }
        set
        {
            _score = value;
            _scoreText.text = _score.ToString();

            // check for high score
            if (_score > PlayerPrefs.GetInt(_highScoreKey))
            {
                _highScoreText1.text = _score.ToString();
            }
        }
    }

    public enum State { menu, init, play, gameover, howTo }
    State _state;
    public enum PopUpState { settings, pause }
    PopUpState _popUpState;

    public GameObject _menuPanel;
    public GameObject _playPanel;
    public GameObject _gameoverPanel;
    public GameObject _howToSequence;

    public GameObject _popUpCanvas;
    public GameObject _settingsPanel;
    public GameObject _pausePanel;

    public GameObject _grid;
    GamePlay _gamePlay;

    public GameObject _playItems;

    void Start()
    {
        _gamePlay = _grid.GetComponent<GamePlay>();
        float scale = _gamePlay.Initialize();

        // Anchor the play items to the grid
        float topOfGrid = (_grid.transform.GetChild(25).position.y / scale);
        _playItems.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, topOfGrid + 110f);

        BeginState(State.menu);
    }

    public void SwitchState(State state)
    {
        EndState();
        BeginState(state);
    }

    void BeginState(State state)
    {
        switch (state)
        {
            case State.menu:
                _state = State.menu;
                _menuHighScoreText.text = PlayerPrefs.GetInt(_highScoreKey).ToString();
                _menuPanel.SetActive(true);
                break;
            case State.init:
                _state = State.init;
                Score = 0;
                _highScoreText1.text = PlayerPrefs.GetInt(_highScoreKey).ToString();
                _grid.SetActive(true);
                SwitchState(State.play);
                break;
            case State.play:
                _state = State.play;
                _playPanel.SetActive(true);
                break;
            case State.gameover:
                _state = State.gameover;
                _finalScoreText.text = " " + Score;
                if (Score > PlayerPrefs.GetInt(_highScoreKey))
                {
                    PlayerPrefs.SetInt(_highScoreKey, Score);
                }
                _highScoreText.text = " " + PlayerPrefs.GetInt(_highScoreKey);
                _gameoverPanel.SetActive(true);
                break;
            case State.howTo:
                _state = State.howTo;
                _howToSequence.SetActive(true);
                break;
        }
    }

    void EndState()
    {
        switch (_state)
        {
            case State.menu:
                _menuPanel.SetActive(false);
                break;
            case State.init:
                break;
            case State.play:
                _playPanel.SetActive(false);
                _gamePlay.EndGamePlay();
                break;
            case State.gameover:
                _gameoverPanel.SetActive(false);
                break;
            case State.howTo:
                _howToSequence.SetActive(false);
                break;
        }
    }

    void SwitchPopUpState(PopUpState state)
    {
        EndPopUpState();
        BeginPopUpState(state);
    }

    // set active game play is messy, fine for now
    void BeginPopUpState(PopUpState state)
    {
        _popUpCanvas.SetActive(true);

        switch (state)
        {
            case PopUpState.settings:
                _popUpState = PopUpState.settings;
                _settingsPanel.SetActive(true);
                break;
            case PopUpState.pause:
                _popUpState = PopUpState.pause;
                _gamePlay.SetActiveGamePlay(false);
                _pausePanel.SetActive(true);
                break;
        }
    }

    void EndPopUpState()
    {
        switch (_popUpState)
        {
            case PopUpState.settings:
                _settingsPanel.SetActive(false);
                break;
            case PopUpState.pause:
                _pausePanel.SetActive(false);
                _gamePlay.SetActiveGamePlay(true);
                break;
        }

        _popUpCanvas.SetActive(false);
    }

    // Menu buttons
    public void PlayClicked()
    {
        if (_popUpCanvas.activeSelf)
        {
            EndPopUpState();
        }

        SwitchState(State.init);
    }

    public void HowToClicked()
    {
        SwitchState(State.howTo);
    }

    // Pop up canvas buttons
    public void SettingsClicked()
    {
        if (_popUpCanvas.activeSelf)
        {
            SwitchPopUpState(PopUpState.settings);
        }
        else
        {
            BeginPopUpState(PopUpState.settings);
        }
    }

    public void PauseClicked()
    {
        BeginPopUpState(PopUpState.pause);
    }

    public void BackClicked()
    {
        if (_state == State.play && _popUpState != PopUpState.pause)
        {
            SwitchPopUpState(PopUpState.pause);
        }
        else
        {
            EndPopUpState();
        }
    }

    // Gameover buttons
    public void MenuClicked()
    {
        if (_popUpCanvas.activeSelf)
        {
            EndPopUpState();
        }

        SwitchState(State.menu);
    }
}
