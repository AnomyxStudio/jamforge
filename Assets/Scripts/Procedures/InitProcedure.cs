using UnityEngine.Scripting;

namespace JamForge.Samples
{
    [Preserve]
    public class InitProcedure : ProcedureBase
    {
        protected override void OnEnter()
        {
            Jam.Messages.Publish(new GameStartMessage("Game Sample"));
            Jam.Logger.Debug($"Initial procedure entered.");
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (ElapsedTime >= 2f)
            {
                Jam.Logger.Debug($"Switching to main procedure.");
                GameProcedures.SwitchProcedure("MainProcedure");
            }
        }

        protected override void OnExit()
        {
            Jam.Logger.Debug($"Initial procedure exited.");
        }
    }
}
