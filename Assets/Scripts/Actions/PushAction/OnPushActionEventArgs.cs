using GridSystems;
using UnityEngine;

namespace Actions
{
    public class OnAnyPushActionEventArgs
    {
        public GridPosition pushedFromGridPosition;
    }
    
    public class OnPushActionEventArgs
    {
        public Animator pushedUnitAnimator;
    }
    
}