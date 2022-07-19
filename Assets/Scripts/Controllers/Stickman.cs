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
    public bool _attacking;
    private IDisposable _disposable;
    private Timer _attackTimer;
    private KdTree<Targetable> _nearTargets;
    public Targetable _closestTarget;

    override public Player PlayerOwner
    {
        get { return _owner; }
    }

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _skin = GetComponentInChildren<SkinnedMeshRenderer>();
        _navAgent = GetComponent<NavMeshAgent>();
        _attackTimer = TimersPool.Instance.Pool.Get();
        _attackTimer.Duration = 0.5f;
        _attackTimer.AddTimerFinishedEventListener(Hit);
    }
    private void Update()
    {
        if (_nearTargets == null)
            return;
        _nearTargets.UpdatePositions();
        if (_attacking)
        {
            if (!_closestTarget.gameObject.activeSelf || _closestTarget.PlayerOwner.number == _owner.number)
            {
                StopAttack();
            }
            else
                transform.LookAt(_closestTarget.transform.position);
        }
        else
        {
            if (_nearTargets.Count > 0)
                _closestTarget = _nearTargets.FindClosest(transform.position);
            else
                _closestTarget = WorldManager.Instance.ClosestFlag(this);

            if (_closestTarget == null)
                return;
            else if (!_closestTarget.gameObject.activeSelf)
            {
                TargetOut(_closestTarget);
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
        if(_closestTarget != null)
        {
            Debug.DrawLine(transform.position, _closestTarget.transform.position, _owner.mainColor);
        }
    }

    public void Initialize(Player owner, float health, Vector3 destination)
    {
        _animator.Rebind();
        _animator.Update(0f);
        _owner = owner;
        name = owner.number.ToString();
        _damage = owner.damage;
        _skin.material.color = _owner.mainColor;

        _attacking = false;
        _currentHealth = health;
        _nearTargets = new KdTree<Targetable>();

        _closestTarget = WorldManager.Instance.ClosestFlag(this);
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
    public void TargetDetected(Targetable target)
    {
        if (_closestTarget == null)
            return;
        if (!Physics.Linecast(transform.position, target.transform.position, StaticValues.StickmanLayer))
        {
            //there is something in the way
            _nearTargets.Add(target);
        }
    }
    public void TargetOut(Targetable target)
    {
        _nearTargets.RemoveAll((x) => x == target);
    }
    private void Dispose()
    {
        if(_disposable == null)
        {
            _disposable = GetComponent<IDisposable>();
        }
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
        TargetOut(_closestTarget);
        _closestTarget = null;
    }
    private void Hit()
    {
        if (!_attacking || !gameObject.activeSelf)
            return;
        _closestTarget.GetHit(this);
        if (_closestTarget.PlayerOwner.number == _owner.number)
            Dispose();
        else
            _attackTimer.Run();

    }
}
