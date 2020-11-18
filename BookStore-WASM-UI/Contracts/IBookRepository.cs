using BookStore_WASM_UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_WASM_UI.Contracts
{
    public interface IBookRepository : IBaseRepository<Book>
    {
    }
}
