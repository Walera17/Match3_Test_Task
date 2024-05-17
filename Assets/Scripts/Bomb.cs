using UnityEngine;

public class Bomb : Gem 
{
    [SerializeField] private int blastSize = 2;

    public int BlastSize => blastSize;
}