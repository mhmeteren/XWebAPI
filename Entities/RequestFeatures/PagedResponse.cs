
namespace Entities.RequestFeatures
{
    public class PagedResponse<T>(IEnumerable<T> items, MetaData metaData) where T : class
    {
        public IEnumerable<T> Items { get; set; } = items;
        public MetaData MetaData { get; set; } = metaData;
    }
}
