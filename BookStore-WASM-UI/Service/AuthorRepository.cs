using Blazored.LocalStorage;
using BookStore_WASM_UI.Contracts;
using BookStore_WASM_UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookStore_WASM_UI.Service
{
    public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly HttpClient _client;
#pragma warning restore IDE0052 // Remove unread private members

#pragma warning disable IDE0052 // Remove unread private members
        private readonly ILocalStorageService _localStorage;
#pragma warning restore IDE0052 // Remove unread private members

        public AuthorRepository(HttpClient client, ILocalStorageService localStorage) : base(client, localStorage)
        {
            _client = client;
            _localStorage = localStorage;
        }
    }
}
