namespace BloodOfEvil.Utilities.IA
{            /// <summary>
             /// Contient une action qui sera lancé si la condition du decorator est bonne.
             /// </summary>
    public abstract class ABehaviourNodeTask<TBlackboard> : ABehaviourNodeTraversal<TBlackboard> where TBlackboard : ABlackboard
    {
        #region Constructor
        public ABehaviourNodeTask()
        {
            this.Type = EBehaviourNodeType.Task;
        }

        public ABehaviourNodeTask(ABehaviourNodeDecorator<TBlackboard> Decorator) : this()
        {
            base.Decorator = Decorator;
        }
        #endregion
    }
}