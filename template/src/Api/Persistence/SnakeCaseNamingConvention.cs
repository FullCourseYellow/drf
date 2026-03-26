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
            if (entity.GetTableName() is { } tableName)
                entity.SetTableName(ToSnakeCase(tableName));

            foreach (var property in entity.GetProperties())
            {
                var storeObject = StoreObjectIdentifier.Table(
                    entity.GetTableName()!, entity.GetSchema());
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

    private static string ToSnakeCase(string name) =>
        Regex.Replace(name, "([a-z0-9])([A-Z])", "$1_$2").ToLowerInvariant();
}
