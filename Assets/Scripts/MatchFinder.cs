using System.Collections.Generic;
using UnityEngine;

public class MatchFinder
{
    private readonly Board board;
    readonly List<Gem> currentMatches = new();

    public MatchFinder(Board board)
    {
        this.board = board;
    }

    public List<Gem> FindAllMatches()
    {
        currentMatches.Clear();

        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                Gem currentGem = board.AllGems[x, y];
                if (currentGem != null && currentGem.Type != GemType.stone)
                {
                    if (x > 0 && x < board.Width - 1)
                    {
                        Gem leftGem = board.AllGems[x - 1, y];
                        Gem rightGem = board.AllGems[x + 1, y];
                        if (leftGem != null && rightGem != null)
                        {
                            if (leftGem.Type == currentGem.Type && rightGem.Type == currentGem.Type)
                            {
                                currentGem.AddToMatches(currentMatches);
                                leftGem.AddToMatches(currentMatches);
                                rightGem.AddToMatches(currentMatches);
                            }
                        }
                    }

                    if (y > 0 && y < board.Height - 1)
                    {
                        Gem aboveGem = board.AllGems[x, y + 1];
                        Gem belowGem = board.AllGems[x, y - 1];
                        if (aboveGem != null && belowGem != null)
                        {
                            if (aboveGem.Type == currentGem.Type && belowGem.Type == currentGem.Type)
                            {
                                currentGem.AddToMatches(currentMatches);
                                aboveGem.AddToMatches(currentMatches);
                                belowGem.AddToMatches(currentMatches);
                            }
                        }
                    }
                }
            }
        }

        CheckForBombs();

        return currentMatches;
    }

    private void CheckForBombs()
    {
        for (int i = 0; i < currentMatches.Count; i++)
        {
            Gem gem = currentMatches[i];

            int x = gem.posIndex.x;
            int y = gem.posIndex.y;

            if (gem.posIndex.x > 0)
            {
                if (board.AllGems[x - 1, y] != null)
                {
                    if (board.AllGems[x - 1, y].Type == GemType.bomb)
                    {
                        MarkBombArea(new Vector2Int(x - 1, y), (Bomb)board.AllGems[x - 1, y]);
                    }
                }
            }

            if (gem.posIndex.x < board.Width - 1)
            {
                if (board.AllGems[x + 1, y] != null)
                {
                    if (board.AllGems[x + 1, y].Type == GemType.bomb)
                    {
                        MarkBombArea(new Vector2Int(x + 1, y), (Bomb)board.AllGems[x + 1, y]);
                    }
                }
            }

            if (gem.posIndex.y > 0)
            {
                if (board.AllGems[x, y - 1] != null)
                {
                    if (board.AllGems[x, y - 1].Type == GemType.bomb)
                    {
                        MarkBombArea(new Vector2Int(x, y - 1), (Bomb)board.AllGems[x, y - 1]);
                    }
                }
            }

            if (gem.posIndex.y < board.Height - 1)
            {
                if (board.AllGems[x, y + 1] != null)
                {
                    if (board.AllGems[x, y + 1].Type == GemType.bomb)
                    {
                        MarkBombArea(new Vector2Int(x, y + 1), (Bomb)board.AllGems[x, y + 1]);
                    }
                }
            }
        }
    }

    private void MarkBombArea(Vector2Int bombPos, Bomb theBomb)
    {
        for (int x = bombPos.x - theBomb.blastSize; x <= bombPos.x + theBomb.blastSize; x++)
        {
            for (int y = bombPos.y - theBomb.blastSize; y <= bombPos.y + theBomb.blastSize; y++)
            {
                if (x >= 0 && x < board.Width && y >= 0 && y < board.Height)
                {
                    if (board.AllGems[x, y] != null)
                    {
                        board.AllGems[x, y].AddToMatches(currentMatches);
                    }
                }
            }
        }
    }
}