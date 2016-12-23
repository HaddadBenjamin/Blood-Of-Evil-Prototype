namespace BloodOfEvil.Utilities.IA
{            /// <summary>
             /// Noeud permettant de parcourir l'arbre de sorte à itérer sur tous ses enfants.
             /// </summary>
    public sealed class BehaviourNodeSequence<TBlackboard> : ABehaviourNodeTraversal<TBlackboard> where TBlackboard : ABlackboard
    {
        #region Constructor
        public BehaviourNodeSequence()
        {
            this.Type = EBehaviourNodeType.Sequence;
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Find all node where the decorator can perform the task and perform them.
        /// </summary>
        public override EBehaviourExecuteType Execute()
        {
            EBehaviourExecuteType executeType = EBehaviourExecuteType.Fail;

            if (null != this.Nodes)
            {
                foreach (ABehaviourNode<TBlackboard> node in this.Nodes)
                {
                    //je récupére le décorateur et si il m'autorise à lancer les sous neouds, je les lance puis je stop l'arbre.
                    //if (node.IsOfType(EBehaviourNodeType.Selector) ||
                    //    node.IsOfType(EBehaviourNodeType.Sequence) ||)
                    //{
                    if (node.CanRetrieveADecoratorAndPerformHisTask())
                    {
                        EBehaviourExecuteType executeTypeTmp = node.Execute();

                        if (EBehaviourExecuteType.Success == executeTypeTmp)
                            executeType = executeTypeTmp;
                        else if (EBehaviourExecuteType.In_Progress == executeTypeTmp &&
                                 node.IsOfType(EBehaviourNodeType.Task))
                        {
                            // l'arbre doit s'arr^éter et jouer que cette tâche jusqu'à elle soit en fail soit en in progress
                        }
                    }
                }
            }

            return executeType;
        }
        //}
        //Autrement Si c'est une tâche je l'execute et stop l'arbre.
        //else if (node.IsOfType(EBehaviourNodeType.Task))
        //{
        //EBehaviourExecuteType executeTypeTmp = node.Execute();

        //if (node.CanRetrieveADecoratorAndPerformHisTask())
        //{
        //    if (EBehaviourExecuteType.Success == executeTypeTmp)
        //        executeType = executeTypeTmp;
        //}
        //}
        //}
        #endregion
    }
}