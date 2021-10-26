using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToSequence : MonoBehaviour
{
    public GameObject _panel1;
    public GameObject _panel2;
    public GameObject _panel3;

    public enum State { screen1, screen2, screen3 }
    State _state;

    void OnEnable()
    {
        BeginState(State.screen1);
    }

    void BeginState(State state)
    {
        switch (state)
        {
            case State.screen1:
                _state = State.screen1;
                _panel1.SetActive(true);
                break;
            case State.screen2:
                _state = State.screen2;
                _panel2.SetActive(true);
                break;
            case State.screen3:
                _state = State.screen3;
                _panel3.SetActive(true);
                break;
        }
    }

    void EndState()
    {
        switch (_state)
        {
            case State.screen1:
                _panel1.SetActive(false);
                BeginState(State.screen2);
                break;
            case State.screen2:
                _panel2.SetActive(false);
                BeginState(State.screen3);
                break;
            case State.screen3:
                _panel3.SetActive(false);
                break;
        }
    }

    public void NextClicked()
    {
        EndState();
    }
}
