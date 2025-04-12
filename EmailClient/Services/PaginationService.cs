namespace EmailClient.Services;

using EmailClient.Services.Contracts;

public class PaginationService : IPaginationService
{
    public IEnumerable<T> Paginate<T>(IEnumerable<T> items, int page, int pageSize)
    {
        return items.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }
}
