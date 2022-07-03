using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adventure2D
{
    public class CheckGround : MonoBehaviour
    {
        [SerializeField] LayerMask _groundMask;
        [SerializeField] float _distance;
        private bool _isGrounded;

        public bool IsGrounded => _isGrounded;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Debug.DrawRay(transform.position, -transform.up * _distance, Color.cyan);
            var hit = Physics2D.Raycast(transform.position, -transform.up, _distance, _groundMask);
            if (hit.collider != null)
            {
                _isGrounded = true;
            }
            else
            {
                _isGrounded = false;
            }
        }
    }
}
