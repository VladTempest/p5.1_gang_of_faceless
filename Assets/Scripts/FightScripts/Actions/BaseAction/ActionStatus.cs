using Unity.VisualScripting;

namespace Editor.Scripts.Actions.BaseAction
{
    public enum ActionStatus
    {
        Discharged = 0,
        ReadyToUse = 1,
        OnCoolDown = 2,
        InProgress = 3
    }
}