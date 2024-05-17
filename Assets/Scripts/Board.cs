using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private GameObject bgTilePrefab;
    [SerializeField] private Gem[] gems;
    [SerializeField] private Gem bomb;
    [SerializeField, Tooltip("¬еро€тность создани€ бомбы")] private float bombChance = 2f;
    [SerializeField, Tooltip("ћаксимальное количество бомб на доске")] private int maxBomb = 1;
    [SerializeField, Tooltip("—корость перемещени€ кристаллов")] private float gemSpeed;
    [SerializeField, Tooltip("ќграничение итераций \"проверки совпадени€ при старте игры\" при генерации кристаллов")] private int maxIterations;
    [Header("Test Mode"), SerializeField, Tooltip("ѕоказать все совпадени€(только дл€ режима тестировани€, в игре не нужно)")] private bool isSelectMatches; 

    private Gem[,] allGems;
    private BoardState currentState = BoardState.Move;
    private Gem otherGem;
    private Vector2Int posIndex;
    private Vector2Int previousPos;
    private MatchFinder matchFinder;
    private int countBomb;

    public static float SpeedGem;

    public int Width => width;
    public int Height => height;
    public Gem[,] AllGems => allGems;
    public BoardState CurrentState => currentState;

    void Start()
    {
        matchFinder = new MatchFinder(this);
        SpeedGem = gemSpeed;
        allGems = new Gem[width, height];

        CreateBoard();
    }

    private void CreateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject bgTile = Instantiate(bgTilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
                bgTile.name = "BG Tile - " + x + ", " + y;

                GenerateStartGem(x, y);
            }
        }
    }

    /// <summary>
    /// √енераци€ кристалла при старте
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void GenerateStartGem(int x, int y)
    {
        int gemToUse = Random.Range(0, gems.Length);

        int iterations = 0;
        posIndex = new Vector2Int(x, y);
        while (MatchesAt(posIndex, gems[gemToUse]) && iterations < maxIterations)
        {
            gemToUse = Random.Range(0, gems.Length);
            iterations++;
        }

        SpawnGem(posIndex, gems[gemToUse], false);
    }

    /// <summary>
    /// —оздание кристалла в заданной позиции
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="gemToSpawn"></param>
    /// <param name="createBomb"></param>
    private void SpawnGem(Vector2Int pos, Gem gemToSpawn, bool createBomb = true)
    {
        if (createBomb && Random.Range(0f, 100f) < bombChance && countBomb < maxBomb)
        {
            gemToSpawn = bomb;
            countBomb++;
        }

        Gem gem = Instantiate(gemToSpawn, new Vector3(pos.x, pos.y + height, 0f), Quaternion.identity, transform);
        gem.name = "Gem - " + pos.x + ", " + pos.y;

        SetupGem(pos, gem);
    }

    /// <summary>
    /// ѕроверка на совпадени€(чтобы при старте не было сразу совпадений)
    /// </summary>
    /// <param name="posToCheck">позици€</param>
    /// <param name="gemToCheck">кристалл</param>
    /// <returns></returns>
    bool MatchesAt(Vector2Int posToCheck, Gem gemToCheck)
    {
        if (posToCheck.x > 1)
        {
            if (allGems[posToCheck.x - 1, posToCheck.y].Type == gemToCheck.Type && allGems[posToCheck.x - 2, posToCheck.y].Type == gemToCheck.Type)
            {
                return true;
            }
        }

        if (posToCheck.y > 1)
        {
            if (allGems[posToCheck.x, posToCheck.y - 1].Type == gemToCheck.Type && allGems[posToCheck.x, posToCheck.y - 2].Type == gemToCheck.Type)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// ѕеремещение фигур(выбранный кристалл и кристалл-цель)
    /// </summary>
    /// <param name="swipeAngle">”гол наклона</param>
    /// <param name="selectGem">¬ыбранный кристалл</param>
    public void MovePieces(float swipeAngle, Gem selectGem)
    {
        previousPos = posIndex = selectGem.posIndex;

        if (swipeAngle is < 45 and > -45 && posIndex.x < width - 1)
        {
            otherGem = allGems[posIndex.x + 1, posIndex.y];
            otherGem.posIndex.x--;
            posIndex.x++;
        }
        else if (swipeAngle is > 45 and <= 135 && posIndex.y < height - 1)
        {
            otherGem = allGems[posIndex.x, posIndex.y + 1];
            otherGem.posIndex.y--;
            posIndex.y++;
        }
        else if (swipeAngle is < -45 and >= -135 && posIndex.y > 0)
        {
            otherGem = allGems[posIndex.x, posIndex.y - 1];
            otherGem.posIndex.y++;
            posIndex.y--;
        }
        else if (swipeAngle > 135 || swipeAngle < -135 && posIndex.x > 0)
        {
            otherGem = allGems[posIndex.x - 1, posIndex.y];
            otherGem.posIndex.x++;
            posIndex.x--;
        }

        SetupGem(posIndex, selectGem);
        SetupGem(otherGem.posIndex, otherGem);

        StartCoroutine(CheckMove(selectGem));
    }

    /// <summary>
    /// ѕроверить перемещение кристаллов
    /// </summary>
    /// <param name="selectGem">¬ыбранный кристалл</param>
    /// <returns></returns>
    private IEnumerator CheckMove(Gem selectGem)
    {
        SetState(BoardState.Wait);

        yield return new WaitForSeconds(.5f);

        List<Gem> allMatches = matchFinder.FindAllMatchesAndCheckForBombs();

        if (otherGem != null)
        {
            if (allMatches.Count == 0)
            {
                otherGem.posIndex = posIndex;
                posIndex = previousPos;

                SetupGem(posIndex, selectGem);
                SetupGem(otherGem.posIndex, otherGem);

                yield return new WaitForSeconds(.5f);

                SetState(BoardState.Move);
            }
            else
            {
                if (isSelectMatches)
                {
                    SelectAllMatches(allMatches);
                    yield return new WaitForSeconds(3f);
                }

                DestroyAllMatches(allMatches);
            }
        }
    }

    /// <summary>
    /// ѕредварительно(перед уничтожением) показать все совпадени€(только дл€ режима тестировани€, в игре не нужно)
    /// </summary>
    /// <param name="allMatches">все совпадени€</param>
    private void SelectAllMatches(List<Gem> allMatches)
    {
        foreach (Gem gem in allMatches)
        {
            gem.transform.localScale = Vector3.one * 0.5f;
        }
    }

    /// <summary>
    /// ”ничтожить все совпадени€
    /// </summary>
    /// <param name="allMatches">все совпадени€</param>
    private void DestroyAllMatches(List<Gem> allMatches)
    {
        for (int i = 0; i < allMatches.Count; i++)
        {
            if (allMatches[i] != null)
            {
                DestroyMatchedGemAt(allMatches[i].posIndex);
            }
        }

        StartCoroutine(DecreaseRowCo());
    }

    /// <summary>
    /// ”ничтожьте кристалл в заданной позиции
    /// </summary>
    /// <param name="pos"></param>
    private void DestroyMatchedGemAt(Vector2Int pos)
    {
        if (allGems[pos.x, pos.y] != null)
        {
            if (allGems[pos.x, pos.y].Type == GemType.Bomb)
            {
                countBomb--;
            }

            Destroy(allGems[pos.x, pos.y].gameObject);
            allGems[pos.x, pos.y] = null;
        }
    }

    private IEnumerator DecreaseRowCo()
    {
        yield return new WaitForSeconds(.2f);

        int nullCounter = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null)
                {
                    nullCounter++;
                }
                else if (nullCounter > 0)
                {
                    allGems[x, y].posIndex.y -= nullCounter;
                    allGems[x, y].SetupMovePosition(allGems[x, y].posIndex);            // запускаем движение кристалла
                    allGems[x, y - nullCounter] = allGems[x, y];
                    allGems[x, y] = null;
                }
            }

            nullCounter = 0;
        }

        StartCoroutine(FillBoardCo());
    }

    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(.5f);
        RefillBoard();

        yield return new WaitForSeconds(.5f);

        List<Gem> allMatches = matchFinder.FindAllMatchesAndCheckForBombs();

        if (allMatches.Count > 0)
        {
            yield return new WaitForSeconds(.5f);

            if (isSelectMatches)
            {
                SelectAllMatches(allMatches);
                yield return new WaitForSeconds(3f);
            }

            DestroyAllMatches(allMatches);
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            currentState = BoardState.Move;
        }
    }

    private void RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null)
                {
                    int gemToUse = Random.Range(0, gems.Length);

                    SpawnGem(new Vector2Int(x, y), gems[gemToUse]);
                }
            }
        }
    }

    /// <summary>
    /// Ќастройка кристалла и задание позиции перемещени€
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="gem"></param>
    private void SetupGem(Vector2Int pos, Gem gem)
    {
        allGems[pos.x, pos.y] = gem;
        gem.SetupMovePosition(pos);
    }

    private void SetState(BoardState state)
    {
        currentState = state;
    }
}