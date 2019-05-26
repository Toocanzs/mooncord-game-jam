using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[RequireComponent(typeof(StateMachine))]
public class StateDebugger : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;
    private StateMachine stateMachine;
    void Start()
    {
        stateMachine = GetComponent<StateMachine>();
    }

    void Update()
    {
        if(stateMachine.CurrentState != null)
            text.text = stateMachine.CurrentState.ToString();
    }
}
