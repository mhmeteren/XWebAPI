
namespace Entities.RequestFeatures
{
    public abstract class RequestParameters
    {

        const int maxPageSize = 20;
        //public int PageNumber { get; set; }

        private int _pageNumber;

        public int PageNumber
        {
            get { return _pageNumber; }
            set { _pageNumber = value > 0 ? value : 1; }
        }


        //private int _pageSize = 20;

        //public int PageSize
        //{
        //    get { return _pageSize; }
        //    set { _pageSize = value > maxPageSize ? maxPageSize : value; }
        //}

        public readonly int PageSize = maxPageSize;


    }
}
