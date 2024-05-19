using Commons;
using System;
using System.Collections.Generic;
using Gems;
using UnityEngine;
using UnityEngine.Pool;

namespace Controllers
{
    [Serializable]
    public class PoolingData
    {
        public Gem prefab;
        public int defaultCapacity;
        public int maxSize;
    }

    public class PoolingController : MonoBehaviour
    {
        [SerializeField] PoolingData[] poolingDates;
        private readonly Dictionary<GemType, ObjectPool<Gem>> poolItems = new();

        private void Awake()
        {
            foreach (PoolingData pooling in poolingDates)
            {
                CreatePooling(pooling.prefab, pooling.defaultCapacity, pooling.maxSize);
            }
        }

        private void OnEnable()
        {
            ParticleController.OnGemDestroy += ParticleController_OnGemDestroy;
        }

        private void OnDisable()
        {
            ParticleController.OnGemDestroy -= ParticleController_OnGemDestroy;
        }

        private void ParticleController_OnGemDestroy(Gem gem)
        {
            Release(gem);
        }

        public Gem Get(GemType type)
        {
            if (poolItems.TryGetValue(type, out ObjectPool<Gem> pool))
                return pool.Get();

            return null;
        }

        public void Release(Gem gem)
        {
            if (poolItems.ContainsKey(gem.Type))
                poolItems[gem.Type].Release(gem);
        }

        void CreatePooling(Gem prefab, int defaultCapacity, int maxSize)
        {
            Transform poolTransform = CreatePoolPatent(prefab.Type + "Pool");

            ObjectPool<Gem> pool = new ObjectPool<Gem>(() => Instantiate(prefab, poolTransform),
                t => t.gameObject.SetActive(true), t => t.gameObject.SetActive(false), t => Destroy(t.gameObject),
                false, defaultCapacity, maxSize);

            poolItems.Add(prefab.Type, pool);
        }

        private Transform CreatePoolPatent(string poolName)
        {
            Transform poolTransform = new GameObject(poolName).transform;
            poolTransform.SetParent(transform);
            return poolTransform;
        }
    }
}