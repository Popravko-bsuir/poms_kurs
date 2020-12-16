using System.Collections;
using UnityEngine;

namespace Environment
{
    public class Spawner : MonoBehaviour
    {
        public GameObject mobToSpawn;
        public void Spawn()
        {
            var spawnerTransform = transform;
            Instantiate(mobToSpawn, spawnerTransform.position, spawnerTransform.rotation);
        }
    }
}
