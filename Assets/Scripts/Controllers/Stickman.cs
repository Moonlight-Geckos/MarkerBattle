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

    override public Player PlayerOwner
    {
        get { return _owner; }
    }

    private void Awake()
    {
        _worldManager = WorldManager.Instance;
        _animator = GetComponentInChildren<Animator>();
        _skin = GetComponentInChildren<SkinnedMeshRenderer>();
        _navAgent = GetComponent<NavMeshAgent>();
        _attackTimer = TimersPool.Instance.Pool.Get();
        _attackTimer.AddTimerFinishedEventListener(Hit);
    }
    private void Update()
    {
        if (_attacking)
        {
            if (!_closestTarget.gameObject.activeSelf || _closestTarget.PlayerOwner.number == _owner.number)
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
            _skin.material.color = _owner.mainColor;
        }

        _attacking = false;
        _currentHealth = health;
        _worldManager.AddTarget(this);
        _closestTarget = _worldManager.ClosestTarget(this);
        _navAgent.Warp(position);
        if(_closestTarget != null)
            GoTowards(_closestTarget.transform.position);
    }
    override public void GetHit(Stickman stick)
    {
        if (_currentHealth <= 0 || !gameObject.activeSelf)
            return;

        _currentHealth-=stick._damage;
        if (_currentHealth <= 0)
            Dispose();
    }
    private void Dispose()
    {
        if(_disposable == null)
        {
            _disposable = GetComponent<IDisposable>();
        }
        if(_closestTarget != null)
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
                Dispose();
            else
                _attackTimer.Run();
        }
    }
}
