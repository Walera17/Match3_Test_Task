using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private Board board;
    private Camera cam;
    private Vector2 firstTouchPosition;
    private bool mousePressed;
    private Gem selectGem;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void OnEnable()
    {
        Gem.OnClick += Gem_OnClick;
    }

    private void OnDisable()
    {
        Gem.OnClick -= Gem_OnClick;
    }

    private void Update()
    {
        if (mousePressed && Input.GetMouseButtonUp(0))
        {
            mousePressed = false;

            if (board.CurrentState == BoardState.move)// && board.roundMan.roundTime > 0)
            {
                Vector2 finalTouchPosition = cam.ScreenToWorldPoint(Input.mousePosition);
                if ((finalTouchPosition - firstTouchPosition).sqrMagnitude > .25f)
                {
                    CalculateAngle(finalTouchPosition);
                }
            }
        }
    }

    private void CalculateAngle(Vector2 finalTouchPosition)
    {
        float swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x);
        swipeAngle = swipeAngle * 180 / Mathf.PI;

        board.MovePieces(swipeAngle, selectGem);
    }

    private void Gem_OnClick(Gem gem)
    {
        selectGem = gem;
        firstTouchPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePressed = true;
    }
}