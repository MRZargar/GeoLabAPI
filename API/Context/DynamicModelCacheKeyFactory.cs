using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace GeoLabAPI
{
    public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
            => context is geolabContext dynamicContext
                ? (context.GetType(), dynamicContext.tableName)
                : (object)context.GetType();
    }
}