// Type: C#
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

// ทำการอ่านไฟล์ appsettings.json
var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
var config = configuration.Build();

// ดึงค่า configuration จาก appsettings.json
string connectionString = config["CosmosDB:ConnectionString"];
string databaseId = config["CosmosDB:DatabaseId"];
string containerId = config["CosmosDB:ContainerId"];
string partitionKeyPath = config["CosmosDB:PartitionKeyPath"];

// เชื่อมต่อ Cosmos DB ด้วย Connection String
CosmosClient client = new CosmosClient(connectionString);

// สร้าง database ด้วย Id ที่กำหนด
Database database = await client.CreateDatabaseIfNotExistsAsync(databaseId);

// สร้าง container ด้วย Id ที่กำหนด พร้อมกำหนดว่า property ไหนจะเป็น partition key
Container container = await database.CreateContainerIfNotExistsAsync(containerId, partitionKeyPath);


// สร้าง object ของ Product record และส่งข้อมูลไปยัง Cosmos DB
Product product = new(
    Guid.NewGuid().ToString(),
    "Test"
);

// ส่งข้อมูลไปยัง Cosmos DB และกำหนด partition key ด้วย Id ของ object
var createItemResponse = await container.CreateItemAsync<Product>(product, new PartitionKey(product.id));


// ประกาศ Product record เพื่อเก็บข้อมูลที่จะส่งไปยัง Cosmos DB
public record Product
(
     string id ,
     string title
);