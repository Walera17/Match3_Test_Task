using UnityEngine;
using UnityEngine.Events;

public class Gem : MonoBehaviour
{
    public static event UnityAction<Gem> OnClick;

    [SerializeField] private GemType type;
    [SerializeField] private int scoreValue = 10;

    [HideInInspector] public Vector2Int posIndex;

    public GemType Type => type;

    public int ScoreValue => scoreValue;

    private bool isMove;
    
    void Update()
    {
        if (isMove)
            Move();
    }

    private void OnMouseDown()
    {
        OnClick?.Invoke(this);
    }

    public void SetupMovePosition(Vector2Int pos)
    {
        posIndex = pos;
        isMove = true;
    }

    private void Move()
    {
        if ((posIndex - (Vector2)transform.position).sqrMagnitude > .0001f)
        {
            transform.position = Vector2.Lerp(transform.position, posIndex, Board.SpeedGem * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(posIndex.x, posIndex.y, 0f);
            isMove = false;
        }
    }
}