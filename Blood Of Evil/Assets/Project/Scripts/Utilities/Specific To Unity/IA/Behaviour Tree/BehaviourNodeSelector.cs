namespace BloodOfEvil.Utilities.IA
{
    /// <summary>
    /// Noeud permettant de parcourir l'arbre de sorte à s'arrêter automatiquement si un des noeuds enfant peut lancer son action.
    /// </summary>
    public sealed class BehaviourNodeSelector<TBlackboard> : ABehaviourNodeTraversal<TBlackboard> where TBlackboard : ABlackboard
    {
        #region Constructor
        public BehaviourNodeSelector()
        {
            this.Type = EBehaviourNodeType.Selector;
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Find the first node where the decorator can perform the task.
        /// </summary>
        public override EBehaviourExecuteType Execute()
        {
            EBehaviourExecuteType executeType = EBehaviourExecuteType.Fail;

            if (null != this.Nodes)
            {
                foreach (ABehaviourNode<TBlackboard> node in this.Nodes)
                {
                    //je récupére le décorateur et si il m'autorise à lancer les sous neouds, je les lance puis je stop l'arbre.
                    if (node.IsOfType(EBehaviourNodeType.Selector) || node.IsOfType(EBehaviourNodeType.Sequence))
                    {
                        if (node.CanRetrieveADecoratorAndPerformHisTask())
                        {
                            executeType = node.Execute();

                            if (EBehaviourExecuteType.Success == executeType)
                                break;
                        }
                    }
                    //Autrement Si c'est une tâche je l'execute et stop l'arbre.
                    else if (node.IsOfType(EBehaviourNodeType.Task))
                    {
                        executeType = node.Execute();

                        if (EBehaviourExecuteType.Success == executeType)
                            break;
                    }
                }
            }

            return executeType;
        }
        #endregion
    }
}