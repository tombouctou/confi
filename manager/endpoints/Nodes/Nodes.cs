using System.Collections;
using Confi.Manager;
using MongoDB.Bson;
using MongoDB.Driver;
using Nist;
using Persic;

namespace Confi;

public static class NodeHelper
{
    public static IEndpointRouteBuilder MapNodes(this IEndpointRouteBuilder endpoints) 
    {
        endpoints.MapPut(Uris.Node("{appId}", "{nodeId}"), PutNode);
        endpoints.MapGet(Uris.Node("{appId}", "{nodeId}"), GetNode);
        endpoints.MapDelete(Uris.Node("{appId}", "{nodeId}"), DeleteNode);

        // backward-compatibility
        endpoints.MapPut("{appId}/nodes/{nodeId}", PutNode);
        endpoints.MapGet("{appId}/nodes/{nodeId}", GetNode);
        endpoints.MapDelete("{appId}/nodes/{nodeId}", DeleteNode);

        return endpoints;
    }

    public static async Task<Node> GetNode(
        string appId, 
        string nodeId, 
        IMongoCollection<NodeRecord> mongoCollection)
    {
        var mongoRecord = await mongoCollection
            .Find(x => x.Id == nodeId && x.AppId == appId)
            .FirstOrDefaultAsync() ?? throw new NodeNotFoundException(appId, nodeId);

        return mongoRecord.ToProtocol();
    }

    public static async Task<Node> PutNode(
        string appId, 
        string nodeId, 
        NodeCandidate candidate, 
        IMongoCollection<NodeRecord> nodeCollection,
        IMongoCollection<SchemeRecord> schemasCollection,
        IMongoCollection<ConfigurationRecord> configurationCollection)
    {
        var record = NodeRecord.From(
            appId: appId, 
            nodeId: nodeId, 
            candidate: candidate
        );

        await nodeCollection.Put(record);

        await schemasCollection.EnsureSchemaIsUpToDate(candidate, appId);
        await configurationCollection.GetOrSetAppConfiguration(appId, candidate.Configuration);

        return record.ToProtocol();
    }

    public static async Task<Node> DeleteNode(
        string appId,
        string nodeId,
        IMongoCollection<NodeRecord> nodeCollection
    )
    {
        var node = await nodeCollection.Search(nodeId) ?? throw new NodeNotFoundException(appId, nodeId);

        await nodeCollection.DeleteOneAsync(x => x.Id == nodeId && x.AppId == appId);

        return node.ToProtocol();
    }

    public static Error? MapNodesErrors(Exception exception)
    {
        return exception switch
        {
            NodeNotFoundException e => e.ToError(),
            _ => null
        };
    }
    
    public static string DetermineStatus(this NodeRecord node, BsonDocument configuration)
    {
        return node.Configuration.Equivalent(configuration)
            ? NodeStatus.Synced : NodeStatus.NotSynced;
    }
}

public static class VersionComparer
{
    public static async Task<List<NodeRecord>> ListBy(this IMongoCollection<NodeRecord> mongoCollection, string appId)
    {
        return await mongoCollection
            .Find(x => x.AppId == appId)
            .ToListAsync();
    }
}

public class NodeNotFoundException(string appId, string nodeId) : Exception
{
    public override IDictionary Data => new Dictionary<string, string> {
        [ "appId" ] = appId,
        [ "nodeId" ] = nodeId
    };

    public Error ToError() => Errors.NodeNotFound;
}