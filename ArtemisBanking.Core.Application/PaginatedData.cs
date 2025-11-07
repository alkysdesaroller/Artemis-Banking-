using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Core.Application;

public class PaginatedData<T>
{
    public IEnumerable<T> Items { get; set; }
    public Pagination Pagination { get; set; }

    public PaginatedData(IEnumerable<T> items, int totalCount, int currentPage, int pageSize)
    {
        Items = items;
        Pagination = new Pagination(totalCount, currentPage, pageSize);
    }

    public static async Task<PaginatedData<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var totalCount = await source.CountAsync();
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedData<T>(items, totalCount, pageNumber, pageSize);
    }
}

public class Pagination
{
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }

    public Pagination(int totalCount, int currentPage, int pageSize)
    {
        TotalCount = totalCount;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public Pagination() { }
}
