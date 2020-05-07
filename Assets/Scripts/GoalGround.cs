using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalGround : MonoBehaviour
{
    [SerializeField] private bool m_isGoalGround = true;

    public bool IsGoalGround {
        get { return m_isGoalGround; }
        set { m_isGoalGround = value; }
    }
}
