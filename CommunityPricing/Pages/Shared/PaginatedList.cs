using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommunityPricing
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        public static async Task<PaginatedList<T>> CreateFromManyAsync(IQueryable<T> sourceOne,
            IQueryable<T> sourceTwo, int pageIndex, int pageSize)
        {
            var count = await sourceOne.CountAsync();
            var countTwo = await sourceTwo.CountAsync();
            var TotalCount = count + countTwo;
            IList<T> listOne = sourceOne.ToList();
            IList<T> listTwo = sourceTwo.ToList();

            foreach (var item in listTwo)
            {
                listOne.Add(item);
            }
            
            var items = listOne.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize).ToList();
            return new PaginatedList<T>(items, TotalCount, pageIndex, pageSize);
        }

        public static PaginatedList<T> CreateNonAsync(
            List<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count;
            var items = source.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
       
    }

}
