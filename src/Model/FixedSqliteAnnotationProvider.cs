using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Sqlite.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Sqlite.Storage.Internal;

namespace Refrase.Model;

// custom implementation to fix a bug
// https://github.com/dotnet/efcore/issues/30699
// https://github.com/dotnet/efcore/issues/29519
// https://github.com/dotnet/efcore/issues/10228
#pragma warning disable EF1001
internal class FixedSqliteAnnotationProvider(RelationalAnnotationProviderDependencies dependencies) : RelationalAnnotationProvider(dependencies)
{
	public override IEnumerable<IAnnotation> For(IRelationalModel model, bool designTime)
	{
		if (!designTime)
			yield break;

		if (model.Tables.SelectMany(t => t.Columns).Any(c => SqliteTypeMappingSource.IsSpatialiteType(c.StoreType)))
			yield return new Annotation(SqliteAnnotationNames.InitSpatialMetaData, true);
	}

	public override IEnumerable<IAnnotation> For(IColumn column, bool designTime)
	{
		if (!designTime)
			yield break;

		if (column is JsonColumn)
			yield break;

		var property = column.PropertyMappings[0].Property;
		var primaryKey = property.DeclaringType.ContainingEntityType.FindPrimaryKey();
		if (primaryKey is { Properties.Count: 1 }
			&& primaryKey.Properties[0] == property
			&& property.ValueGenerated == ValueGenerated.OnAdd)
			yield return new Annotation(SqliteAnnotationNames.Autoincrement, true);

		int? srid = property.GetSrid();
		if (srid != null)
			yield return new Annotation(SqliteAnnotationNames.Srid, srid);
	}
}
#pragma warning restore EF1001
