/// <summary>
/// Типы кристаллов
/// </summary>
public enum GemType
{
    Blue,
    Green,
    Red,
    Yellow,
    Purple,
    Bomb,
    Stone
}

/// <summary>
/// Режим доски(можно двигать кристалл или ожидать)
/// </summary>
public enum BoardState
{
    /// <summary>
    /// Ожидать завершения операции
    /// </summary>
    Wait,
    /// <summary>
    /// Можно перемещать кристалл
    /// </summary>
    Move
}