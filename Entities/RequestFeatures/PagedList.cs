
namespace Entities.RequestFeatures
{
    public class PagedList<T> : List<T>
    {
        public MetaData MetaData { get; set; }

        public PagedList(List<T> items, int count, RequestParameters requestParameters)
        {
            MetaData = new MetaData()
            {
                TotalCount = count,
                PageSize = requestParameters.PageSize,
                CurrentPage = requestParameters.PageNumber,
                TotalPage = (int)Math.Ceiling(count / (double)requestParameters.PageSize)
            };
            AddRange(items);
        }

        public static PagedList<T> ToPagedList(IEnumerable<T> source, RequestParameters requestParameters)
        {
            var count = source.Count();
            var items = source
                .Skip((requestParameters.PageNumber - 1) * requestParameters.PageSize)
                .Take(requestParameters.PageSize)
                .ToList();
            return new PagedList<T>(items, count, requestParameters);
        }

    }
}
