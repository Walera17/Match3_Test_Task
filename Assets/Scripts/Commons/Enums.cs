namespace Commons
{
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
        Stone,
        Tile
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
}