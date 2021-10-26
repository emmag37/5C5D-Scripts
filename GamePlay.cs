using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// add something to game play to make it more dynamic -- special wildcard that covers up a space

public class GamePlay : MonoBehaviour
{
    GameObject[] _circles;
    GameObject[,] _players;
    Color[,] _colors;

    GameObject _scaledPlayerPrefab;
    Player _currentPlayer;
    Vector3 _startPos;

    float _newScale;

    public GameObject _playerPrefab;

    public float Initialize()
    {
        _scaledPlayerPrefab = Instantiate(_playerPrefab);
        _scaledPlayerPrefab.SetActive(false);

        // scale the grid and player prefab
        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        _newScale = canvas.GetComponent<RectTransform>().localScale.x;
        transform.localScale *= _newScale;
        _scaledPlayerPrefab.transform.position *= _newScale;
        _scaledPlayerPrefab.transform.localScale *= _newScale;

        _players = new GameObject[5, 5];
        _colors = new Color[5, 5];

        _startPos = _scaledPlayerPrefab.transform.position;

        return _newScale;
    }

    void OnEnable()
    {
        _circles = GameObject.FindGameObjectsWithTag("Circle");// this is weird, fine for now
        SpawnNextPlayer();
    }

    public void SetActiveGamePlay(bool active)
    {
        _currentPlayer._enabled = active;
    }

    public void EndGamePlay()
    {
        DestroyAllPlayers();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (_currentPlayer._dragged)
        {
            SetFinalLocation();
        }
    }

    void SetFinalLocation()
    {
        bool onGrid = PutOnGrid();

        if (onGrid)
        {
            _currentPlayer._enabled = false;
            int x = _currentPlayer.GetX();
            int y = _currentPlayer.GetY();

            bool gameover = CheckGrid(x, y);

            if (gameover)
            {
                FindObjectOfType<GameManager>().SwitchState(GameManager.State.gameover);
            }
            else
            {
                SpawnNextPlayer();
            }

        }
        else
        {
            _currentPlayer._dragged = false;
            _currentPlayer._touchOnCircle = false;
        }
    }

    public void SpawnNextPlayer()
    {
        GameObject newPlayer = Instantiate(_scaledPlayerPrefab);
        newPlayer.SetActive(true);

        _currentPlayer = newPlayer.GetComponent<Player>();
    }

    bool PutOnGrid()
    {
        Vector3 pos = _currentPlayer.transform.position;

        for (int i = 0; i < _circles.Length; i++)
        {
            GameObject currCircle = _circles[i];
            Vector3 localCirclePos = currCircle.transform.localPosition;
            Vector3 circlePos = currCircle.transform.position;

            int x = GetXGridPos(localCirclePos.x);
            int y = GetYGridPos(localCirclePos.y);

            if (_currentPlayer.IsOnCircle(pos, circlePos) &&
                (_players[y, x] == null || _currentPlayer._color == Color.black))
            {
                _currentPlayer.transform.position = circlePos;
                _currentPlayer.SetX(y);
                _currentPlayer.SetY(x);

                if (_currentPlayer._color == Color.black)
                {
                    Destroy(_players[y, x]);
                }

                _players[y, x] = _currentPlayer.gameObject;
                _colors[y, x] = _currentPlayer._color;

                return true;
            }
        }

        _currentPlayer.transform.position = _startPos;

        return false;
    }

    int GetXGridPos(float xPos)
    {
        // get the local position
        xPos += 3f;
        if (xPos != 0f)
        {
            return (int)(xPos / 1.5f);
        }
        return 0;
    }

    int GetYGridPos(float yPos)
    {
        yPos += 3f;
        if (yPos != 0f)
        {
            return (int)(yPos / 1.5f);
        }
        return 0;
    }

    // Returns whether or not there is a gameover 
    bool CheckGrid(int x, int y)
    {
        int points = 0;

        bool destroyDiagonal1 = false;
        bool destroyDiagonal2 = false;

        bool destroyRow = Check5InRow(x);
        bool destroyColumn = Check5InColumn(y);
        if (x == y)
        {
            destroyDiagonal1 = Check5InDiagonal1();
        }
        if ((4 - x) == y)
        {
            destroyDiagonal2 = Check5InDiagonal2();
        }

        if (destroyRow)
        {
            points += 50;
            DestroyRow(x);
        }
        if (destroyColumn)
        {
            points += 50;
            DestroyColumn(y);
        }
        if (destroyDiagonal1)
        {
            points += 50;
            DestroyDiagonal1();
        }
        if (destroyDiagonal2)
        {
            points += 50;
            DestroyDiagonal2();
        }

        // add increasing high score to this
        // No gameover if objects were destroyed
        if (points > 0)
        {
            FindObjectOfType<GameManager>().Score += points;
            return false;
        }
        else
        {
            FindObjectOfType<GameManager>().Score += 10;
        }

        return CheckGridIsFull();
    }

    bool Check5InRow(int x)
    {
        Color wildCard = Color.gray;
        Color rowColor = wildCard;

        for (int k = 0; k < 5; k++)
        {
            if (_players[x, k] == null)
            {
                return false;
            }
            Color color = _colors[x, k];
            if (color == Color.black)
            {
                color = wildCard;
            }

            if (color != wildCard && rowColor != wildCard && rowColor != color)
            {
                return false;
            }
            else if (rowColor == wildCard)
            {
                rowColor = color;
            }
        }
        return true;
    }
    bool Check5InColumn(int y)
    {
        Color wildCard = Color.gray;
        Color rowColor = wildCard;

        for (int k = 0; k < 5; k++)
        {
            if (_players[k, y] == null)
            {
                return false;
            }
            Color color = _colors[k, y];
            if (color == Color.black)
            {
                color = wildCard;
            }

            if (color != wildCard && rowColor != wildCard && rowColor != color)
            {
                return false;
            }
            else if (rowColor == wildCard)
            {
                rowColor = color;
            }
        }
        return true;
    }
    bool Check5InDiagonal1()
    {
        Color wildCard = Color.gray;
        Color rowColor = wildCard;

        for (int k = 0; k < 5; k++)
        {
            if (_players[k, k] == null)
            {
                return false;
            }
            Color color = _colors[k, k];
            if (color == Color.black)
            {
                color = wildCard;
            }

            if (color != wildCard && rowColor != wildCard && rowColor != color)
            {
                return false;
            }
            else if (rowColor == wildCard)
            {
                rowColor = color;
            }
        }
        return true;
    }
    bool Check5InDiagonal2()
    {
        Color wildCard = Color.gray;
        Color rowColor = wildCard;

        for (int k = 0; k < 5; k++)
        {
            if (_players[k, 4 - k] == null)
            {
                return false;
            }
            Color color = _colors[k, 4 - k];
            if (color == Color.black)
            {
                color = wildCard;
            }

            if (color != wildCard && rowColor != wildCard && rowColor != color)
            {
                return false;
            }
            else if (rowColor == wildCard)
            {
                rowColor = color;
            }
        }
        return true;
    }

    void DestroyRow(int x)
    {
        for (int k = 0; k < 5; k++)
        {
            Destroy(_players[x, k]);
        }
    }
    void DestroyColumn(int y)
    {
        for (int k = 0; k < 5; k++)
        {
            Destroy(_players[k, y]);
        }
    }
    void DestroyDiagonal1()
    {
        for (int i = 0; i < 5; i++)
        {
            Destroy(_players[i, i]);
        }
    }
    void DestroyDiagonal2()
    {
        for (int i = 0; i < 5; i++)
        {
            Destroy(_players[i, 4 - i]);
        }
    }
    void DestroyAllPlayers()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int k = 0; k < 5; k++)
            {
                Destroy(_players[i, k]);
            }
        }

        Destroy(_currentPlayer.gameObject);
    }

    bool CheckGridIsFull()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int k = 0; k < 5; k++)
            {
                if (_players[i, k] == null)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
