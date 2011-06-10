﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo4jClient
{
    public abstract class Relationship
    {
        readonly object data;
        readonly NodeReference otherNode;

        protected Relationship(NodeReference targetNode)
            : this(targetNode, null)
        {
        }

        protected internal Relationship(NodeReference targetNode, object data)
        {
            this.data = data;
            otherNode = targetNode;

            if (targetNode == null)
                throw new ArgumentNullException("targetNode");

            Direction = RelationshipDirection.Automatic;
        }

        public NodeReference OtherNode
        {
            get { return otherNode; }
        }

        public abstract string RelationshipTypeKey { get; }

        public object Data
        {
            get { return data; }
        }

        public RelationshipDirection Direction { get; set; }

        internal static RelationshipDirection DetermineRelationshipDirection<TBaseNode>(Type baseNodeType, Relationship relationship)
        {
            var allowedSourceNodeTypes = GetAllowedNodeTypes(relationship.GetType(), RelationshipEnd.SourceNode);
            var allowedTargetNodeTypes = GetAllowedNodeTypes(relationship.GetType(), RelationshipEnd.TargetNode);

            return RelationshipDirection.Automatic;
        }

        internal static IEnumerable<Type> GetAllowedNodeTypes(Type relationshipType, RelationshipEnd end)
        {
            Type interfaceType;
            switch (end)
            {
                case RelationshipEnd.SourceNode:
                    interfaceType = typeof (IRelationshipAllowingSourceNode<>);
                    break;
                case RelationshipEnd.TargetNode:
                    interfaceType = typeof (IRelationshipAllowingTargetNode<>);
                    break;
                default:
                    throw new NotSupportedException(string.Format(
                        "The specified relationship end is not supported: {0}", end));
            }

            return relationshipType
                .GetInterfaces()
                .Where(i => i.GetGenericTypeDefinition() == interfaceType)
                .Select(i => i.GetGenericArguments()[0])
                .ToArray();
        }
    }
}