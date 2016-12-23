using UnityEngine;

namespace BloodOfEvil.Utilities.IA
{
    using ObjectInScene;

            /// <summary>
            /// On ne peut qu'hériter de cette classe pour pouvoir redéfinir notre classe de donnée.
            /// </summary>
            public abstract class ABlackboard : AInitializableComponent
            {
                #region Abstract Methods
                /// <summary>
                /// Permet de mettre à jour les valeurs du blackboard.
                /// </summary>
                public virtual void UpdateBlackboardAtEachFrame() { }
                #endregion
            }

    /// <summary>
    /// Classe dont chaque noeud hérite, cette dernière est composé d'un tableau de noeuds et du blackboard.
    /// </summary>
    public abstract class ABehaviourNode<TBlackboard> where TBlackboard : ABlackboard
    {
        #region Fields
        private EBehaviourNodeType type;
        private ABehaviourNode<TBlackboard>[] nodes;
        //Les nodes traversal ne devrait en avoir besoin, cependant ceci obligerai de reset tous les blackboard de chaque noeud.
        private TBlackboard blackboard;
        #endregion

        #region Properties
        protected EBehaviourNodeType Type
        {
            get { return type; }
            set { type = value; }
        }

        public ABehaviourNode<TBlackboard>[] Nodes
        {
            protected get { return nodes; }
            set
            {
                nodes = value;

                Blackboard = blackboard;
            }
        }

        public TBlackboard Blackboard
        {
            protected get { return blackboard; }
            set
            {
                blackboard = value;

                if (null != nodes)
                {
                    for (byte i = 0; i < nodes.Length; i++)
                    {
                        nodes[i].Blackboard = blackboard;
                        Debug.Log(i);

                        ABehaviourNodeTraversal<TBlackboard> nodeTraversal = nodes[i] as ABehaviourNodeTraversal<TBlackboard>;

                        if (null != nodeTraversal)
                        {
                            ABehaviourNodeDecorator<TBlackboard> nodeDecorator = nodeTraversal.Decorator;

                            if (null != nodeDecorator)
                                nodeDecorator.Blackboard = blackboard;
                        }
                    }
                }
            }
        }
        #endregion

        #region Constructor
        public ABehaviourNode()
        {
            this.type = EBehaviourNodeType.NotDefine;
        }
        #endregion

        #region Abstract Methods
        //Execute : pour les taches, PerformTask pour les taches ?
        public abstract EBehaviourExecuteType Execute();
        #endregion

        #region Behaviour Methods
        public bool IsOfType(EBehaviourNodeType type)
        {
            return this.type == type;
        }

        //Ne devrait pas être ici mais c'est plus pratique.
        public bool CanRetrieveADecoratorAndPerformHisTask()
        {
            ABehaviourNodeTraversal<TBlackboard> nodeTraversal = this as ABehaviourNodeTraversal<TBlackboard>;

            if (null != nodeTraversal)
            {
                ABehaviourNodeDecorator<TBlackboard> nodeDecorator = nodeTraversal.Decorator;

                if (null != nodeDecorator)
                {
                    if (nodeDecorator.CanPerformTask())
                        return true;
                }
            }

            return false;
        }
        #endregion
    }
}