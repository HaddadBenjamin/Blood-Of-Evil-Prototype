namespace BloodOfEvil.Utilities.IA
{
    /// <summary>
    /// Classe d'abstraction regroupant les noeuds permettant de parcourir l'arbre, c'est à dire une sequence ou un selector.
    /// </summary>
    public abstract class ABehaviourNodeTraversal<TBlackboard> : ABehaviourNode<TBlackboard> where TBlackboard : ABlackboard
    {
        #region Fields
        private ABehaviourNodeDecorator<TBlackboard> decorator;
        #endregion

        #region Attributes
        public ABehaviourNodeDecorator<TBlackboard> Decorator
        {
            get { return decorator; }
            set { decorator = value; }
        }
        #endregion

        #region Constructor
        public ABehaviourNodeTraversal()
        {
            this.decorator = new BehaviourNodeDecoratorDefault<TBlackboard>();
        }
        #endregion
    }
}