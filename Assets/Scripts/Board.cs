using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private GameObject bgTilePrefab;
    [SerializeField] private Gem[] gems;
    [SerializeField, Tooltip("Скорость перемещения кристаллов")] private float gemSpeed;
    [Header("Bomb"), SerializeField] private Gem bomb;
    [SerializeField, Tooltip("Вероятность создания бомбы")] private int bombChance = 10;
    [SerializeField, Tooltip("Максимальное количество бомб на доске")] private int maxBomb = 1;
    [Header("Test Mode"), SerializeField, Tooltip("Показать все совпадения(только для режима тестирования, в игре не нужно)")] private bool isSelectMatches;

    private Gem[,] allGems;
    private BoardState currentState;
    private Gem otherGem;
    private Vector2Int posIndex;
    private Vector2Int previousPos;
    private MatchFinder matchFinder;
    private int countBomb;
    private WaitForSeconds operationDelay;
    private WaitForSeconds smallDelay;
    public static float SpeedGem;

    public int Width => width;
    public int Height => height;
    public Gem[,] AllGems => allGems;
    public BoardState CurrentState => currentState;

    void Start()
    {
        currentState = BoardState.Move;
        operationDelay = new WaitForSeconds(.5f);
        smallDelay = new WaitForSeconds(.2f);
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
    /// Генерация кристалла при старте
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void GenerateStartGem(int x, int y)
    {
        int gemToUse = Random.Range(0, gems.Length);

        int iterations = 0;
        posIndex = new Vector2Int(x, y);
        while (MatchesAt(posIndex, gems[gemToUse]) && iterations < 100)
        {
            gemToUse = Random.Range(0, gems.Length);
            iterations++;
        }

        SpawnGem(posIndex, gems[gemToUse], false);
    }

    /// <summary>
    /// Создание кристалла в заданной позиции
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="gemToSpawn"></param>
    /// <param name="createBomb"></param>
    private void SpawnGem(Vector2Int pos, Gem gemToSpawn, bool createBomb = true)
    {
        if (createBomb && Random.Range(0, 100) < bombChance && countBomb < maxBomb)
        {
            gemToSpawn = bomb;
            countBomb++;
        }

        Gem gem = Instantiate(gemToSpawn, new Vector3(pos.x, pos.y + height, 0f), Quaternion.identity, transform);
        gem.name = "Gem - " + pos.x + ", " + pos.y;

        SetupGem(pos, gem);
    }

    /// <summary>
    /// Проверка на совпадения(чтобы при старте не было сразу совпадений)
    /// </summary>
    /// <param name="posToCheck">позиция</param>
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
    /// Перемещение фигур(выбранный кристалл и кристалл-цель)
    /// </summary>
    /// <param name="swipeAngle">Угол наклона</param>
    /// <param name="selectGem">Выбранный кристалл</param>
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
    /// Проверить перемещение кристаллов
    /// </summary>
    /// <param name="selectGem">Выбранный кристалл</param>
    /// <returns></returns>
    private IEnumerator CheckMove(Gem selectGem)
    {
        currentState = BoardState.Wait;

        yield return operationDelay;

        List<Gem> allMatches = matchFinder.FindAllMatchesAndCheckForBombs();

        if (otherGem != null)
        {
            if (allMatches.Count == 0)
            {
                otherGem.posIndex = posIndex;
                posIndex = previousPos;

                SetupGem(posIndex, selectGem);
                SetupGem(otherGem.posIndex, otherGem);

                yield return operationDelay;

                currentState = BoardState.Move;
            }
            else
            {
                if (isSelectMatches)                                            // Показать все совпадения(только для режима тестирования, в игре не нужно)
                {
                    SelectAllMatches(allMatches);
                    yield return new WaitForSeconds(3f);
                }

                DestroyAllMatches(allMatches);
            }
        }
    }

    /// <summary>
    /// Предварительно(перед уничтожением) показать все совпадения(только для режима тестирования, в игре не нужно)
    /// </summary>
    /// <param name="allMatches">все совпадения</param>
    private void SelectAllMatches(List<Gem> allMatches)
    {
        foreach (Gem gem in allMatches)
        {
            gem.transform.localScale = Vector3.one * 0.5f;
        }
    }

    /// <summary>
    /// Уничтожить все совпадения
    /// </summary>
    /// <param name="allMatches">все совпадения</param>
    private void DestroyAllMatches(List<Gem> allMatches)
    {
        for (int i = 0; i < allMatches.Count; i++)
        {
            if (allMatches[i] != null)
            {
                DestroyMatchedGemAt(allMatches[i].posIndex);
            }
        }

        StartCoroutine(DecreaseRow());
    }

    /// <summary>
    /// Уничтожьте кристалл в заданной позиции
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

            //Destroy(allGems[pos.x, pos.y].gameObject);
            allGems[pos.x, pos.y].RemoveGem();
            allGems[pos.x, pos.y] = null;
        }
    }

    /// <summary>
    /// Снижение по рядам(где есть пустоты)
    /// </summary>
    /// <returns></returns>
    private IEnumerator DecreaseRow()
    {
        yield return smallDelay;

        for (int x = 0; x < width; x++)                                                 // проход по всем столбцам
        {
            int nullCounter = 0;                                                        // счетчик пустоты

            for (int y = 0; y < height; y++)                                            // проход по текущему столбцу
            {
                if (allGems[x, y] == null)
                {
                    nullCounter++;
                }
                else if (nullCounter > 0)
                {
                    allGems[x, y].posIndex.y -= nullCounter;                            // устанавливаем новую позицию кристалла
                    allGems[x, y].SetupMovePosition(allGems[x, y].posIndex);            // запускаем движение кристалла
                    allGems[x, y - nullCounter] = allGems[x, y];                        // присваиваем текущему кристаллу новое положение в таблице 
                    allGems[x, y] = null;                                               // очищаем текущее положение в таблице
                }
            }
        }

        StartCoroutine(FillBoard());
    }

    /// <summary>
    /// Заполнение и контроль доски
    /// </summary>
    /// <returns></returns>
    private IEnumerator FillBoard()
    {
        yield return operationDelay;
        RefillBoard();

        yield return operationDelay;

        List<Gem> allMatches = matchFinder.FindAllMatchesAndCheckForBombs();

        if (allMatches.Count > 0)
        {
            yield return operationDelay;

            if (isSelectMatches)                                                        // Показать все совпадения(только для режима тестирования, в игре не нужно)
            {
                SelectAllMatches(allMatches);
                yield return new WaitForSeconds(3f);
            }

            DestroyAllMatches(allMatches);
        }
        else
        {
            yield return operationDelay;
            currentState = BoardState.Move;
        }
    }

    /// <summary>
    /// Пополнение доски
    /// </summary>
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
    ///  Перетасовка доски
    /// </summary>
    public void ShuffleBoard()
    {
        if (currentState != BoardState.Wait)
        {
            currentState = BoardState.Wait;

            List<Gem> gemsFromBoard = new List<Gem>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gemsFromBoard.Add(allGems[x, y]);
                    allGems[x, y] = null;
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int gemToUse = Random.Range(0, gemsFromBoard.Count);

                    int iterations = 0;
                    posIndex = new Vector2Int(x, y);
                    while (MatchesAt(posIndex, gemsFromBoard[gemToUse]) && iterations < 100 && gemsFromBoard.Count > 1)
                    {
                        gemToUse = Random.Range(0, gemsFromBoard.Count);
                        iterations++;
                    }

                    SetupGem(posIndex, gemsFromBoard[gemToUse]);
                    allGems[x, y] = gemsFromBoard[gemToUse];
                    gemsFromBoard.RemoveAt(gemToUse);
                }
            }

            StartCoroutine(FillBoard());
        }
    }

    /// <summary>
    /// Настройка кристалла и задание позиции перемещения
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="gem"></param>
    private void SetupGem(Vector2Int pos, Gem gem)
    {
        allGems[pos.x, pos.y] = gem;
        gem.SetupMovePosition(pos);
    }
}