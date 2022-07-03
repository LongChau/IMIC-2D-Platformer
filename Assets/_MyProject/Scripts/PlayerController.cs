using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Adventure2D
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] Animator _anim;
        [SerializeField] Rigidbody2D _rigid2D;
        [SerializeField] float _movementSpeed;
        [SerializeField] EPlayerState _state;
        [SerializeField] CheckGround _checkGround;
        [SerializeField] float _jumpCooldown;
        [SerializeField] EJumpState _jumpState;
        [SerializeField] LayerMask _targetMask;
        [SerializeField] Transform _damageObj;
        [SerializeField] Vector2 _attackSize;
        [SerializeField] int _damage;

        float _axis;
        bool _canAttack;
        float _jumpTime;

        public EPlayerState State 
        { 
            get => _state;
            set
            {
                _state = value;
                UpdateAnimation();
            }
        }

        private void UpdateAnimation()
        {
            switch (_state)
            {
                case EPlayerState.Idle:
                    _anim.SetBool("IsIdle", true);
                    _canAttack = true;
                    break;
                case EPlayerState.MoveRight:
                    _anim.SetBool("IsIdle", false);
                    if (_jumpState == EJumpState.OnAir)
                    {
                        _anim.Play("Player_Jump");
                    }
                    _canAttack = true;
                    break;
                case EPlayerState.MoveLeft:
                    _anim.SetBool("IsIdle", false);
                    if (_jumpState == EJumpState.OnAir)
                    {
                        _anim.Play("Player_Jump");
                    }
                    _canAttack = true;
                    break;
                case EPlayerState.Jump:
                    _anim.SetBool("IsIdle", false);
                    _canAttack = false;
                    break;
                case EPlayerState.Attack:
                    _anim.SetBool("IsIdle", false);
                    _anim.SetTrigger("Attack");
                    _canAttack = false;
                    break;
                default:
                    break;
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            _axis = Input.GetAxis("Horizontal");

            if (State != EPlayerState.Attack)
            {
                if (_axis > 0)
                {
                    FaceScaling(1);
                    State = EPlayerState.MoveRight;
                }
                else if (_axis < 0)
                {
                    FaceScaling(-1);
                    State = EPlayerState.MoveLeft;
                }
                else
                {
                    if (_jumpState == EJumpState.OnGround)
                        State = EPlayerState.Idle;
                    else if (_jumpState == EJumpState.OnAir)
                        _anim.Play("Player_Jump");
                }
            }

            bool isJump = Input.GetKeyDown(KeyCode.Space);
            if (isJump && _checkGround.IsGrounded && _jumpTime <= 0)
            {
                State = EPlayerState.Jump;
            }

            bool isAttack = Input.GetKeyDown(KeyCode.L);
            if (isAttack && _canAttack)
            {
                State = EPlayerState.Attack;
            }

            if (_jumpTime > 0) _jumpTime -= Time.deltaTime;

            _anim.SetBool("IsGrounded", _checkGround.IsGrounded);
            if (_checkGround.IsGrounded)
            {
                _jumpState = EJumpState.OnGround;
            }
            else
            {
                _jumpState = EJumpState.OnAir;
            }
        }

        private void FaceScaling(int value)
        {
            var scale = transform.localScale;
            scale.x = value;
            transform.localScale = scale;
        }

        private void FixedUpdate()
        {
            UpdateMovement();
        }

        private void UpdateMovement()
        {
            switch (State)
            {
                case EPlayerState.Idle:
                    var vel = _rigid2D.velocity;
                    if (vel.x != 0f)
                        vel.x = Mathf.Lerp(vel.x, 0f, Time.fixedDeltaTime);
                    _rigid2D.velocity = vel;
                    break;
                case EPlayerState.MoveRight:
                    _rigid2D.velocity = new Vector2(_movementSpeed * _axis * Time.fixedDeltaTime, _rigid2D.velocity.y);
                    break;
                case EPlayerState.MoveLeft:
                    _rigid2D.velocity = new Vector2(_movementSpeed * _axis * Time.fixedDeltaTime, _rigid2D.velocity.y);
                    break;
                case EPlayerState.Jump:
                    _rigid2D.velocity = Vector2.zero;
                    _rigid2D.AddForce(new Vector2(0f, 3f), ForceMode2D.Impulse);
                    _jumpTime = _jumpCooldown;
                    break;
                case EPlayerState.Attack:

                    break;
                default:
                    break;
            }
        }

        private void Handle_EndAttack()
        {
            State = EPlayerState.Idle;
        }

        private void Handle_StartAttack()
        {
            Debug.Log("Attack");
            var hit = Physics2D.BoxCast(_damageObj.position, _attackSize, 0f, Vector2.zero, 0f, _targetMask);
            if (hit.collider != null)
            {
                Debug.Log($"{hit.collider.name} take damage...");
                if (hit.collider.CompareTag("Enemy/Skeleton"))
                {
                    var enemy = hit.collider.GetComponent<EnemyController>();
                    enemy.TakeDamage(_damage);
                    Debug.Log("Bla blabla");
                }
            }
        }

        // Implement this OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(_damageObj.position, _attackSize);

        }

        public enum EPlayerState
        {
            Idle = 0,
            MoveRight = 1,
            MoveLeft = 2,
            Jump = 3,
            Attack = 4,
        }

        public enum EJumpState
        {
            OnGround = 0,
            OnAir = 1,
        }
    }
}
