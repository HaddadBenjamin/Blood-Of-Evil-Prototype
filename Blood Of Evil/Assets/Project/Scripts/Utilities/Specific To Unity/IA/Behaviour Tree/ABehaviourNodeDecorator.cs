namespace BloodOfEvil.Utilities.IA
{
    /// <summary>
    /// Contient la condition permettant de déterminer si on peut lancer une tâche ou non.
    /// </summary>
    public abstract class ABehaviourNodeDecorator<TBlackboard> where TBlackboard : ABlackboard
    {
        #region Fields
        private TBlackboard blackboard;
        #endregion

        #region Attributes
        public TBlackboard Blackboard
        {
            get { return blackboard; }
            set { blackboard = value; }
        }
        #endregion

        #region Abstract Methods
        /// <summary>
        /// Détermine si on peut ou non lancer la tâche.
        /// </summary>
        public abstract bool CanPerformTask();
        #endregion
    }
}