using UnityEngine;

namespace Gems
{
    public class Bomb : Gem 
    {
        [SerializeField] private int blastSize = 2;

        public int BlastSize => blastSize;
    }
}