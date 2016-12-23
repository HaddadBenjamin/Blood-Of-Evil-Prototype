namespace BloodOfEvil.Utilities.IA
{            /// <summary>
             /// Le décorateur par défault autorise à chaque fois de lancer la tâche.
             /// </summary>
    public sealed class BehaviourNodeDecoratorDefault<TBlackboard> : ABehaviourNodeDecorator<TBlackboard> where TBlackboard : ABlackboard
    {
        #region Override Methods
        public override bool CanPerformTask()
        {
            return true;
        }
        #endregion
    }
}