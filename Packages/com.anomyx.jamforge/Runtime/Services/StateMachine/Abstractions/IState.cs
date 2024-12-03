using System;

namespace JamForge.StateMachine
{
    public interface IState
    {
        #region Lifetime

        void Enter();

        void Update(float deltaTime);

        void Exit();

        float ElapsedTime { get; }

        #endregion

        #region Properties

        bool Active { get; }

        #endregion

        #region Events

        event Action<IState> StateEnterEvent;

        event Action<IState> StateExitEvent;

        event Action<IState, float> StateUpdateEvent;

        #endregion
    }
}
