
# Example .NET console application that loads XML from Google Alert and put them into Azure Cosmos DB
Please provide the following files to make the project work

### 1. `appsettings.json` at the project's root

```json
{
    "CosmosDB": {
        "Endpoint":"",
        "PrimaryKey":"",
        "DatabaseId":"",
        "ContainerId":"",
        "PartitionKeyPath":""
    }
}
```