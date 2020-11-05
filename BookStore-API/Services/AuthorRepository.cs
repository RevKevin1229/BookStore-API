using BookStore_API.Contracts;
using BookStore_API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Services
{
    public class AuthorRepository : IAuthorRepository
    {
#pragma warning disable IDE0051 // Remove unused private members
        private readonly ApplicationDbContext _db;
#pragma warning restore IDE0051 // Remove unused private members

        public Task<bool> Create(Author entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(Author entity)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Author>> FindAll()
        {
            throw new NotImplementedException();
        }

        public Task<Author> FindById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Save()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Author entity)
        {
            throw new NotImplementedException();
        }
    }
}
