namespace Shopping_Web.Models
{
    public class Paginate
    {
        public int TotalItems { get; private set; }
        public int PageSize { get; private set; } //total items/ 1 page
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
        public Paginate()
        {

        }
        public Paginate(int totalItems, int page, int pageSize = 10)
        {
            int totalPage = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            int currentPage = page;
            int startPage = currentPage - 5; // trang hiện tại -5 sẽ ra nút trang trước trang hiện tại 
            int endPage = currentPage + 4; // trang kết thúc hiẻne thị sau trang hiện tai 4 nút 
            if (startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }
            if (endPage > totalPage)
            {
                endPage = totalPage;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }
            TotalItems = totalPage;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = totalPage;
            StartPage = startPage;
            EndPage = endPage;
        }
    }
}
