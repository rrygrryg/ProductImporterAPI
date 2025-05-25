using Microsoft.Data.SqlClient;
using Dapper;

namespace ProductImporterAPI.Services
{
    public class DatabaseInitializer
    {
        private readonly string _masterConnectionString;
        private readonly string _dbConnectionString;
        private readonly string _dbName;

        public DatabaseInitializer(IConfiguration configuration)
        {
            _dbConnectionString = configuration.GetConnectionString("DefaultConnection")!;
            var builder = new SqlConnectionStringBuilder(_dbConnectionString);
            _dbName = builder.InitialCatalog!;
            builder.InitialCatalog = "master";
            _masterConnectionString = builder.ConnectionString;
        }

        public void EnsureDatabaseExists()
        {
            using var connection = new SqlConnection(_masterConnectionString);
            connection.Open();

            var dbExists = connection.ExecuteScalar<int>($@"
                SELECT COUNT(*) FROM sys.databases WHERE name = '{_dbName}'
            ") > 0;

            if (!dbExists)
            {
                connection.Execute($"CREATE DATABASE [{_dbName}]");
            }

            // Czekaj do całkowitego stworzenia bazy danych
            Thread.Sleep(1000);

            using var dbConnection = new SqlConnection(_dbConnectionString);
            dbConnection.Open();

            dbConnection.Execute(@"
                IF OBJECT_ID('Products') IS NULL
                CREATE TABLE Products (
                    ID INT PRIMARY KEY,
                    SKU NVARCHAR(100),
                    Name NVARCHAR(255),
                    EAN NVARCHAR(100),
                    ProducerName NVARCHAR(255),
                    Category NVARCHAR(255),
                    DefaultImage NVARCHAR(1000)
                );

                IF OBJECT_ID('Inventory') IS NULL
                CREATE TABLE Inventory (
                    ProductID INT PRIMARY KEY,
                    SKU NVARCHAR(100),
                    Unit NVARCHAR(50),
                    Qty INT,
                    Shipping INT,
                    ShippingCost DECIMAL(18,2)
                );

                IF OBJECT_ID('Prices') IS NULL
                CREATE TABLE Prices (
                    SKU NVARCHAR(100) PRIMARY KEY,
                    NetPrice DECIMAL(18,2),
                    DiscountedPrice DECIMAL(18,2),
                    VatRate DECIMAL(5,2),
                    UnitPrice DECIMAL(18,2)
                );
            ");
        }
    }
}

