using System;

namespace Actions
{
    public class OnShootEventArgs : EventArgs
    {
        public Unit TargetUnit;
        public Action HitCallback;
    }
}