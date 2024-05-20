using Commons;
using UnityEngine;

namespace Gems
{
    public class PollElement : MonoBehaviour
    {
        [SerializeField] protected GemType type;

        public GemType Type => type;
    }
}