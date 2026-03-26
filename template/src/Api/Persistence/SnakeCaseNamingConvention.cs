using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Text.RegularExpressions;

namespace Company.ProjectName.Api.Persistence;

public sealed class SnakeCaseNamingConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(
        IConventionModelBuilder modelBuilder,
        IConventionContext<IConventionModelBuilder> context)
    {
        foreach (var entity in modelBuilder.Metadata.GetEntityTypes())
        {
            var originalTableName = entity.GetTableName();

            if (originalTableName is { } tableName)
                entity.SetTableName(ToSnakeCase(tableName));

            foreach (var property in entity.GetProperties())
            {
                var storeObject = StoreObjectIdentifier.Table(
                    originalTableName ?? entity.GetTableName()!, entity.GetSchema());
                if (property.GetColumnName(storeObject) is { } columnName)
                    property.SetColumnName(ToSnakeCase(columnName));
            }

            foreach (var key in entity.GetKeys())
                if (key.GetName() is { } name) key.SetName(ToSnakeCase(name));

            foreach (var fk in entity.GetForeignKeys())
                if (fk.GetConstraintName() is { } name) fk.SetConstraintName(ToSnakeCase(name));

            foreach (var index in entity.GetIndexes())
                if (index.GetDatabaseName() is { } name) index.SetDatabaseName(ToSnakeCase(name));
        }
    }

    private static string ToSnakeCase(string name)
    {
        var result = Regex.Replace(name, "([A-Z]+)([A-Z][a-z])", "$1_$2");
        result = Regex.Replace(result, "([a-z0-9])([A-Z])", "$1_$2");
        return result.ToLowerInvariant();
    }
}
