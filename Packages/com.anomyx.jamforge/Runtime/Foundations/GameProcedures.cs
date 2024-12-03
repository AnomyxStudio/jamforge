using System;
using System.Collections.Generic;
using JamForge.StateMachine;
using JetBrains.Annotations;
using UnityEngine;

namespace JamForge
{
    public abstract class ProcedureBase : State
    {
        [UsedImplicitly]
        public GameProcedures GameProcedures { get; internal set; }
    }

    public class GameProcedures : MonoBehaviour
    {
        [SerializeField] private string defaultProcedure;
        [SerializeField] private List<string> procedureTypes;

        public IStateMachineRunner<string, ProcedureBase> StateMachineRunner { get; private set; }

        public Dictionary<string, ProcedureBase> Procedures { get; } = new();

        public string CurrentIndex => StateMachineRunner.ActiveStateIndex;
        public ProcedureBase CurrentProcedure => StateMachineRunner?.ActiveState;

        private void Start()
        {
            StateMachineRunner = new StateMachineRunner<string, ProcedureBase>();

            for (var i = 0; i < procedureTypes.Count; i++)
            {
                try
                {
                    var procedureType = TypeFinder.Get(procedureTypes[i]);
                    if (procedureType == null) { continue; }

                    if (Jam.Resolver.Create(procedureType) is not ProcedureBase procedure) { continue; }

                    procedure.GameProcedures = this;

                    var procedureName = procedureType.FullName;
                    if (string.IsNullOrEmpty(procedureName))
                    {
                        // This should not happen
                        InternalLog.Error($"Procedure {procedureType.FullName} has no full name");
                        continue;
                    }
                    
                    StateMachineRunner.AddState(procedureName, procedure);
                    Procedures.Add(procedureName, procedure);
                } catch (Exception e)
                {
                    InternalLog.Error(e.Message);
                }
            }

            if (string.IsNullOrEmpty(defaultProcedure))
            {
                defaultProcedure = procedureTypes[0];
            }

            if (string.IsNullOrEmpty(defaultProcedure))
            {
                InternalLog.Warn($"No default procedure was found.");
                return;
            }

            StateMachineRunner.Start(defaultProcedure);
        }

        private void Update()
        {
            StateMachineRunner.Run(Time.deltaTime);
        }

        public void SwitchProcedure(string stateIndex)
        {
            StateMachineRunner.SwitchState(stateIndex);
        }
    }
}
