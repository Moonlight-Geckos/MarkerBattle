using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Stickman : Targetable
{
    private Animator _animator;
    private SkinnedMeshRenderer _skin;
    private NavMeshAgent _navAgent;
    private Player _owner;

    public float _currentHealth;
    private float _damage;
    private bool _attacking;
    private IDisposable _disposable;
    private Timer _attackTimer;
    private Targetable _closestTarget;
    private WorldManager _worldManager;
    private ParticlesPool _splashPool;

    //ADDED THIS VARIABLE FOR FUNCTIONALITY COMMENTED BELOW
    private StickmenSpawner _stickmenSpawner;

    // Event to track when a strickman dies
    //public Action OnDeath;
    //public event Action OnDeathEvent;

    override public Player PlayerOwner
    {
        get { return _owner; }
    }

    private void Awake()
    {
        _worldManager = WorldManager.Instance;
        _stickmenSpawner = _worldManager.gameObject.GetComponent<StickmenSpawner>();
        _animator = GetComponentInChildren<Animator>();
        _skin = GetComponentInChildren<SkinnedMeshRenderer>();
        _navAgent = GetComponent<NavMeshAgent>();

        _splashPool = PoolsPool.Instance.SplashPool;

        _attackTimer = TimersPool.Instance.Pool.Get();
        _attackTimer.AddTimerFinishedEventListener(Hit);

    }
    private void Update()
    {
        if (_attacking)
        {
            if (_closestTarget == null || !_closestTarget.gameObject.activeSelf || _closestTarget.PlayerOwner.number == _owner.number)
            {
                StopAttack();
            }
        }
        else
        {
            _closestTarget = _worldManager.ClosestTarget(this);

            if (_closestTarget == null || !_closestTarget.gameObject.activeSelf)
            {
                return;
            }

            if (Vector3.Distance(transform.position, _closestTarget.transform.position) < 0.3f)
            {
                StartAttack();
            }
            else
            {
                GoTowards(_closestTarget.transform.position);
            }
        }
    }

    public void Initialize(Player owner, float health, Vector3 position, Vector3 destination)
    {

        _animator.Rebind();
        _animator.Update(0f);
        if (_owner == null || _owner.number != owner.number)
        {
            _owner = owner;
            name = owner.number.ToString();
            _damage = owner.damage;

            _attackTimer.Duration = owner.attackCooldown;

            // ADDED THIS HERE SO THAT MATERIAL COLOR CHANGES ONLY IF THE POOLS ARE THE SAME
            // MEANING SAME MODELS FOR ALLY AND ENEMY, ORIGINALLY WAS AS BELOW
            //_skin.material.color = _owner.mainColor;

            if (_stickmenSpawner.stickmenPool == _stickmenSpawner.stickmenPoolEnemy)
            {
                _skin.material.color = _owner.mainColor;
            }


        }

        _attacking = false;
        _currentHealth = health;
        _worldManager.AddTarget(this);
        _closestTarget = _worldManager.ClosestTarget(this);
        _navAgent.Warp(position);
        if (_closestTarget != null)
            GoTowards(_closestTarget.transform.position);
    }
    override public void GetHit(Stickman stick)
    {
        if (_currentHealth <= 0 || !gameObject.activeSelf)
            return;

        _currentHealth -= stick._damage;
        if (_currentHealth <= 0)
        {
            Explode();
            Dispose();
        }
    }
    private void Dispose()
    {
        if (_disposable == null)
        {
            _disposable = GetComponent<IDisposable>();
        }
        if (_closestTarget != null)
            _worldManager.RemoveTarget(this);
        _disposable.Dispose();
        _attackTimer.Stop();
    }
    private void GoTowards(Vector3 pos)
    {
        _navAgent.destination = pos;
    }
    private void StartAttack()
    {
        _attacking = true;
        _animator.SetBool("Attack", true);
        Hit();
    }
    private void StopAttack()
    {
        _attacking = false;
        _attackTimer.Stop();
        _animator.SetBool("Attack", false);
        _closestTarget = null;
    }
    private void Hit()
    {
        if (!_attacking || !gameObject.activeSelf)
            return;
        if (Vector3.Distance(_closestTarget.transform.position, transform.position) > 0.3f)
        {
            StopAttack();
        }
        else
        {
            transform.LookAt(_closestTarget.transform.position);
            _closestTarget.GetHit(this);
            if (_closestTarget.name[0] == 'F')
            {
                Explode();
                Dispose();

                //sound effects go here
                GameManager.Instance.PlayDeathSound();
            }
            else
                _attackTimer.Run();
        }
    }
    private void Explode()
    {
        var ps = _splashPool.Pool.Get();
        ps.transform.position = transform.position;
        ps.Initialize(PlayerOwner.mainColor);
    }
}