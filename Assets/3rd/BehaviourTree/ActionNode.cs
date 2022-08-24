
    public abstract class ActionNode : Node
    {
        protected abstract override void Onstart();
        protected abstract override void OnStop();
        protected abstract override State OnUpdate();
    }
