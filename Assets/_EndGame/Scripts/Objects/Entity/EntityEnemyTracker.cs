using System;
using Pathfinding;
using UnityEngine;

[Serializable]
public class EntityEnemyTracker
{
    private Transform ourEntityTransform;
    private AIPath aStar;

    public float AttackRange = 2f;
    
    private Transform _trackedEnemyTransform;
    public Transform TrackedEnemyTransform
    {
        get { return _trackedEnemyTransform; }
        set
        {
            if (value == _trackedEnemyTransform) return;
            _trackedEnemyTransform = value;

            if (_trackedEnemyTransform == null) return;
            aStar.destination = TrackedEnemyTransform.position;
            aStar.SearchPath();
        }
    }
    
    public EntityEnemyTracker(Transform ourTransform, AIPath astarAI)
    {
        ourEntityTransform = ourTransform;
        aStar = astarAI;
    }

    private float DistanceToEnemy()
    {
        return Vector3.Distance(TrackedEnemyTransform.position, ourEntityTransform.position);
    }

    public bool IsInRange()
    {
       return DistanceToEnemy() <= AttackRange;
    }

    public void OnUpdate()
    {
        if (!TrackedEnemyTransform) return;
        
        // clear tracked as we're too far away
        if (DistanceToEnemy() > 10f)
        {
            TrackedEnemyTransform = null;
            aStar.destination = ourEntityTransform.forward + ourEntityTransform.position;
            aStar.SearchPath();
            return;
        }
        
        aStar.isStopped = IsInRange();

        if (IsInRange())
        {
            aStar.ClearCurrentPath();
            return;
        }

        aStar.destination = TrackedEnemyTransform.position;

    }

}
