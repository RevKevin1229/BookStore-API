﻿using Blazored.LocalStorage;
using BookStore_WASM_UI.Contracts;
using BookStore_WASM_UI.Models;
using BookStore_WASM_UI.Providers;
using BookStore_WASM_UI.Static;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BookStore_WASM_UI.Service
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationRepository(HttpClient client
            , ILocalStorageService localStorage
            , AuthenticationStateProvider authenticationStateProvider)
        {
            _client = client;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> Login(LoginModel user)
        {
            var response = await _client.PostAsJsonAsync(Endpoints.LoginEndpoint, user);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var content = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenResponse>(content);

            // Store Token
            await _localStorage.SetItemAsync("authToken", token.Token);

            // Change Authentication State of App
            await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedIn();

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", token.Token);

            return true;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedOut();
        }

        public async Task<bool> Register(RegistrationModel user)
        {
            var response = await _client.PostAsJsonAsync(Endpoints.RegisterEndpoint, user);

            return response.IsSuccessStatusCode;
        }
    }
}
