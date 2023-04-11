using System.Collections.Generic;
using System.Linq;
using Neo4j.Driver;
using Neo4jClient.Extensions;
using Newtonsoft.Json;

namespace Neo4jClient.ApiModels.Cypher
{
    public class PathsResultBolt
    {
        public PathsResultBolt()
        {
            Nodes = new List<PathsResultBoltNode>();
            Relationships = new List<PathsResultBoltRelationship>();
        }

        internal PathsResultBolt(IPath path)
        {
            Start = new PathsResultBoltNode(path.Start);
            End = new PathsResultBoltNode(path.End);
            Relationships = path.Relationships.Select(r => new PathsResultBoltRelationship(r)).ToList();
            Nodes = path.Nodes.Select(r => new PathsResultBoltNode(r)).ToList();
        }

        [JsonProperty("Start")]
        public PathsResultBoltNode Start { get; set; }

        [JsonProperty("End")]
        public PathsResultBoltNode End { get; set; }

        [JsonIgnore]
        public int Length => Relationships.Count();

        [JsonProperty("Nodes")]
        public List<PathsResultBoltNode> Nodes { get; set; }

        [JsonProperty("Relationships")]
        public List<PathsResultBoltRelationship> Relationships { get; set; }

        public class PathsResultBoltRelationship
        {
            [JsonProperty(nameof(Id))]
            public string Id { get; set; }
            [JsonProperty(nameof(Type))]
            public string Type { get; set; }
            [JsonProperty(nameof(StartNodeId))]
            public string StartNodeId { get; set; }
            [JsonProperty(nameof(EndNodeId))]
            public string EndNodeId { get; set; }

            public object this[string key] => Properties[key];

            [JsonProperty(nameof(Properties))]
            public Dictionary<string, object> Properties { get; set; }

            public PathsResultBoltRelationship() { Properties = new Dictionary<string, object>(); }

            public PathsResultBoltRelationship(IRelationship relationship)
            {
                Id = relationship.ElementId;
                StartNodeId = relationship.StartNodeElementId;
                EndNodeId = relationship.EndNodeElementId;
                Type = relationship.Type;
                Properties = relationship.Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            public bool Equals(PathsResultBoltRelationship other)
            {
                if (other == null)
                    return false;

                return Id == other.Id
                       && StartNodeId == other.StartNodeId
                       && EndNodeId == other.EndNodeId
                       && Type == other.Type
                       && Properties.ContentsEqual(other.Properties);
            }

            public override bool Equals(object obj) => Equals(obj as PathsResultBoltRelationship);

            public override int GetHashCode() => System.HashCode.Combine(Id, Type, StartNodeId, EndNodeId, Properties);
        }

        public class PathsResultBoltNode 
        {
            [JsonProperty(nameof(Id))]
            public string Id { get; set; }
            [JsonProperty(nameof(Labels))]
            public List<string> Labels { get; set; }
            public object this[string key] => Properties[key];
            [JsonProperty(nameof(Properties))]
            public Dictionary<string, object> Properties { get; set; }

            public PathsResultBoltNode() { Properties = new Dictionary<string, object>(); }

            internal PathsResultBoltNode(INode node)
            {
                Id = node.ElementId;
                Labels = node.Labels?.ToList();
                Properties = node.Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            public bool Equals(PathsResultBoltNode other)
            {
                if (other == null)
                    return false;

                return Id == other.Id
                       && Labels.ContentsEqual(other.Labels)
                       && Properties.ContentsEqual(other.Properties);
            }

            public override bool Equals(object obj) => Equals(obj as PathsResultBoltNode);

            public override int GetHashCode() => System.HashCode.Combine(Id, Labels, Properties);
        }
    }
}