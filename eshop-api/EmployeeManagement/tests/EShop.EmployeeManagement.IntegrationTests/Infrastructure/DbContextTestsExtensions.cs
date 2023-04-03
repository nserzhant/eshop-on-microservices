using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace EShop.EmployeeManagement.Core.IntegrationTests.Infrastructure;

public static class DbContextTestsExtensions
{
    public static async Task ClearDb(this DbContext dbContext, params string[] tablesToPreserve)
    {
        var schema = dbContext.Model.GetDefaultSchema();
        var tablesToClear = dbContext.Model.GetEntityTypes()
            .Select(t => t.GetTableName())
            .Where(t => tablesToPreserve == null || !tablesToPreserve.Contains(t))
            .Distinct()
            .ToList();

        if (tablesToClear == null || tablesToClear.Count == 0)
            return;

        var clearSql = getClearTablesCommandText(schema ?? "dbo", tablesToClear);

        using var connection = new SqlConnection(dbContext.Database.GetConnectionString());

        connection.Open();

        using var command = connection.CreateCommand();

        command.CommandType = CommandType.Text;
        command.CommandText = clearSql;
        await command.ExecuteNonQueryAsync();
    }

    public static void RecreateDb(this DbContext dbContext)
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();
    }

    private static string getClearTablesCommandText(string schemaName, List<string?> tablesToClear)
    {
        var tables = tablesToClear.Select(t => $"'{t}'").Aggregate((a, i) => a + ',' + i);

        var clearDbSql = @$"DECLARE @tableNames NVARCHAR(MAX) = '';
                SELECT
	                @tableNames = @tableNames + ',' + val 
                FROM 
                (
	                SELECT 
		                CONVERT(NVARCHAR(50), t.object_id) val
	                FROM sys.tables t
	                INNER JOIN sys.schemas s 
		                ON (t.schema_id = s.schema_id)
	                INNER JOIN sys.partitions p 
		                ON (t.object_id = p.object_id)
	                WHERE p.index_id in (0,1)
		                AND p.rows > 0
						AND s.name = '{schemaName}'
		                AND t.name IN ({tables})
                ) t;

                IF @tableNames != ''
                BEGIN
	                SET @tableNames = ' And Object_id In (' + SUBSTRING(@tableNames, 2, LEN(@tableNames)) + ')'
	                EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL', @whereand = @tableNames
	                EXEC sp_MSForEachTable 'SET QUOTED_IDENTIFIER ON; DELETE FROM ?', @whereand = @tableNames
	                EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL', @whereand = @tableNames
                END";

        return clearDbSql;
    }
}
