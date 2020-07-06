using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatchValue {
    Blue,
    White,
    Pink,
    Yellow,
    Red,
    None,
};

[RequireComponent(typeof(SpriteRenderer))]
public class GamePiece : MonoBehaviour
{
    public int m_xIndex;
    public int m_yIndex;

    bool m_isMoving = false;

    Board m_board;

    public InterpType m_interpolation = InterpType.SmootherStep;

    public enum InterpType {
        Linear,
        EaseOut,
        EaseIn,
        SmoothStep,
        SmootherStep,
    };

    public MatchValue m_matchValue;

    public int m_scoreValue = 20;

    public AudioClip m_clearSound;

    // ==

    public void Init(Board _board)
    {
        m_board = _board;
    }

    public void SetCoord(int _x, int _y) {

        m_xIndex = _x;
        m_yIndex = _y;
    }

    public void Move(int _destX, int _destY, float _timeToMove) {
        if (!m_isMoving) {
            //MoveRoutine coroutine 시작
            StartCoroutine(MoveRoutine(new Vector3(_destX, _destY, 0), _timeToMove));
        }
    }

    IEnumerator MoveRoutine(Vector3 _destination, float _timeToMove) {

        Vector3 startPosition = transform.position;

        bool reachedDestination = false;

        float elapsedTime = 0f;

        m_isMoving = true;

        while(!reachedDestination) {
            // if we are close enough to destination
            if (Vector3.Distance(transform.position, _destination) < 0.01f) {
                reachedDestination = true;

                if(m_board != null) {
                    m_board.PlaceGamePiece(this, (int)_destination.x, (int)_destination.y);
                }

                break;
            }

            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp(elapsedTime / _timeToMove, 0f, 1f);

            switch(m_interpolation) { // 그래프에 따라 움직임의 매끄러움이 차이가 난다.
                case InterpType.Linear:
                    break;
                case InterpType.EaseOut: // sin(x)
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);
                    break;
                case InterpType.EaseIn: // 1 - cos(x)
                    t = 1 - Mathf.Cos(t * Mathf.PI * 0.5f);
                    break;
                case InterpType.SmoothStep: // 3x^2 - 2x^3
                    t = t * t * (3 - 2 * t);
                    break;
                case InterpType.SmootherStep: // 6x^5 - 15x^4 + 10
                    t = t * t * t * (t * (t * 6 - 15) + 10);
                    break;
            }
            // move the game piece
            transform.position = Vector3.Lerp(startPosition, _destination, t);

            // wait until next frame
            yield return null;
        }

        m_isMoving = false;
    }

    // change color of bombs
    // 입력 매개변수로 색상 뿐만 아니라 모양을 전달해야한다.
    // GamePiece의 색상을 변경할 때마다 모양도 업데이트 할 것.
    public void ChangeColor(GamePiece _pieceToMatch) {

        SpriteRenderer rendererToChange = GetComponent<SpriteRenderer>();
        Color colorToMatch = Color.clear;

        if(_pieceToMatch != null) {
            SpriteRenderer rendererToMatch = _pieceToMatch.GetComponent<SpriteRenderer>();
            if(rendererToMatch != null && rendererToChange != null) {
                rendererToChange.sprite = rendererToMatch.sprite;

            }// if2
            m_matchValue = _pieceToMatch.m_matchValue;

        }// if1
    }

    
}
