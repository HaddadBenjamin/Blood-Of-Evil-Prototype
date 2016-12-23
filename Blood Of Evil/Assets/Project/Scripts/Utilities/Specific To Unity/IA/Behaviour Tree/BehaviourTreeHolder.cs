namespace BloodOfEvil.Utilities.IA
{            /// <summary>
             /// Contient un behaviournode root permettant de lancer une action de façon récursive.
             /// </summary>
    public sealed class BehaviourTreeHolder<TBlackboard> where TBlackboard : ABlackboard
    {
        #region Fields
        private ABehaviourNode<TBlackboard> root;
        private ABlackboard blackboard;
        #endregion

        #region Constructor
        public BehaviourTreeHolder(ABehaviourNode<TBlackboard> root, TBlackboard blackboard)
        {
            this.root = root;
            this.blackboard = blackboard;

            this.root.Blackboard = blackboard;
        }
        #endregion

        #region Behaviour
        public void Execute()
        {
            if (null != this.blackboard)
                this.blackboard.UpdateBlackboardAtEachFrame();

            if (null != this.root)
                this.root.Execute();
        }
        #endregion
    }
}