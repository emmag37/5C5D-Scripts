using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Vector3 _startPos;
    float _radius;
    int _x;
    int _y;

    float _lowerXBound;
    float _upperXBound;
    float _lowerYBound;
    float _upperYBound;

    Color _blue;
    Color _aqua;
    Color _green;
    Color _yellow;
    Color _orange;
    Color _pink;
    Color _wildCard;
    Color _blackOut;

    public Color _color;

    public bool _dragged;
    public bool _enabled;
    public bool _touchOnCircle;

    void Start()
    {
        _startPos = transform.position;
        _radius = transform.localScale.x / 2f;
        
        _dragged = false;
        _enabled = true;
        _touchOnCircle = false;
        _x = -1;
        _y = -1;

        float lowerGridEdge = _startPos.y + _radius * 3.75f;
        float upperGridEdge = -1 * lowerGridEdge;

        _lowerXBound = lowerGridEdge; // left side of the grid
        _upperXBound = upperGridEdge; // right side of the grid
        _lowerYBound = _startPos.y - _radius; // bottom of the player at startPos
        _upperYBound = upperGridEdge; // top side of the grid

        _blue = new Color(0.2794118f, 0.6370588f, 0.95f, 1f);
        _aqua = new Color(0.2541176f, 0.8329412f, 0.9f, 1f);
        _green = new Color(0.6730015f, 0.9019608f, 0.3607843f, 1f);
        _yellow = new Color(0.93f, 0.8658351f, 0.04650001f, 1f);
        _orange = new Color(0.95f, 0.6631373f, 0.2309804f, 1f);
        _pink = new Color(0.93f, 0.4194118f, 0.7585882f, 1f);
        _wildCard = Color.gray;
        _blackOut = Color.black;

        _color = PickColor();
        gameObject.GetComponent<SpriteRenderer>().color = _color;
    }

    void Update()
    {
        if (_enabled)
        {
            CheckUserInput();
        }
    }

    void CheckUserInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0;
            bool withinXBounds = WithinXBounds(touchPosition);
            bool withinYBounds = WithinYBounds(touchPosition);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (IsOnCircle(touchPosition, _startPos))
                    {
                        _touchOnCircle = true;
                    }
                    break;
                case TouchPhase.Stationary:
                    if (_touchOnCircle)
                    {
                        if (withinXBounds && withinYBounds)
                        {
                            transform.position = touchPosition;
                        }
                        else if (withinXBounds && !withinYBounds)
                        {
                            transform.position = new Vector3(touchPosition.x, transform.position.y, 0f);
                        }
                        else if (!withinXBounds && withinYBounds)
                        {
                            transform.position = new Vector3(transform.position.x, touchPosition.y, 0f);
                        }
                    }
                    break;
                case TouchPhase.Moved:
                    if (_touchOnCircle)
                    {
                        if (withinXBounds && withinYBounds)
                        {
                            transform.position = touchPosition;
                        }
                        else if (withinXBounds && !withinYBounds)
                        {
                            transform.position = new Vector3(touchPosition.x, transform.position.y, 0f);
                        }
                        else if (!withinXBounds && withinYBounds)
                        {
                            transform.position = new Vector3(transform.position.x, touchPosition.y, 0f);
                        }
                    }
                    break;
                case TouchPhase.Ended:
                    if (_touchOnCircle)
                    {
                        _dragged = true;
                    }
                    break;
            }
        }
    }

    public void SetX(int x)
    {
        _x = x;
    }
    public int GetX()
    {
        return _x;
    }

    public void SetY(int y)
    {
        _y = y;
    }
    public int GetY()
    {
        return _y;
    }

    public bool IsOnCircle(Vector3 pos, Vector3 circlePos)
    {
        float lowerX = circlePos.x - (_radius);
        float upperX = circlePos.x + (_radius);
        float lowerY = circlePos.y - (_radius);
        float upperY = circlePos.y + (_radius);

        if (lowerX <= pos.x && pos.x <= upperX && lowerY <= pos.y && pos.y <= upperY)
        {
            return true;
        }

        return false;
    }

    bool WithinXBounds(Vector3 pos)
    {
        if (_lowerXBound <= pos.x && pos.x <= _upperXBound)
        {
            return true;
        }

        return false;
    }

    bool WithinYBounds(Vector3 pos)
    {
        if (_lowerYBound <= pos.y && pos.y <= _upperYBound)
        {
            return true;
        }

        return false;
    }
    
    Color PickColor()
    {
        int color = Random.Range(1, 8); // figure out how to adust frequencies

        switch (color)
        {
            case 1:
                return _blue;
            case 2:
                return _aqua;
            case 3:
                return _green;
            case 4:
                return _yellow;
            case 5:
                return _orange;
            case 6:
                return _pink;
            case 7:
                int wild = Random.Range(1, 3);
                if (wild == 1)
                {
                    return _wildCard;
                }
                else
                {
                    return _blackOut;
                }
        }

        return Color.white;
    }
}
