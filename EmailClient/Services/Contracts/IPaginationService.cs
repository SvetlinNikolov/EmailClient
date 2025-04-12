namespace EmailClient.Services.Contracts;

public interface IPaginationService
{
    IEnumerable<T> Paginate<T>(IEnumerable<T> items, int page, int pageSize);
}
