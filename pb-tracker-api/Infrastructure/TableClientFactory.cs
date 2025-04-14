using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using pb_tracker_api.Abstractions;

namespace pb_tracker_api.Infrastructure;

#region: -- Models 
public enum TableClientTable
{
    User,
    Pb,
}
#endregion: -- Models

public interface ITableClientFactory
{
    Task<Result<TableClient, IError>> GetTableClientByKey(TableClientTable table);
}

public class TableClientFactory(IConfiguration config) : ITableClientFactory
{
    private readonly IConfiguration _config = config;

    public Task<Result<TableClient, IError>> GetTableClientByKey(TableClientTable table)
        => _config["TABLE_CLIENT_CONNECTION_STRING"]
            .ToOption()
            .MapNoneErrAsync(new ConfigMissingError("Table connection not found", nameof(GetTableClientByKey)))
            .Then(connection =>
            {
                return table switch
                {
                    // --  Add more tables as needed
                    TableClientTable.User => Task.FromResult(Result<TableClient, IError>.Ok(new TableClient(connection, "User"))),
                    TableClientTable.Pb => Task.FromResult(Result<TableClient, IError>.Ok(new TableClient(connection, "Pblog"))),
                    _ => Task.FromResult(Result<TableClient, IError>.Err(new ConfigMissingError($"Table with key:{table} not found", nameof(GetTableClientByKey)))),
                };
            });
}
