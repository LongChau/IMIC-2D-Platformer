using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adventure2D
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] int _hp;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void TakeDamage(int damg)
        {
            _hp -= damg;
        }
    }
}
