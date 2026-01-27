using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net;
using Survey.DTOs.Auth;
using Survey.DTOs.Common;

namespace Survey.Services.Api
{
    public class AuthApiService : IAuthApiService
    {
        private readonly HttpClient _http;

        public AuthApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(dto);
            Console.WriteLine("SEND JSON: " + json);

            var content = JsonContent.Create(dto);

            var resp = await _http.PostAsync("api/auth/login", content);

            var raw = await resp.Content.ReadAsStringAsync();
            Console.WriteLine("RAW RESP: " + raw);

            //var resp = await _http.PostAsJsonAsync("api/auth/login", dto);

            //var raw = await resp.Content.ReadAsStringAsync();

            // Kalau response bukan JSON (HTML error page), ini bakal kelihatan
            if (string.IsNullOrWhiteSpace(raw))
                throw new Exception($"Empty response. Status={(int)resp.StatusCode} {resp.ReasonPhrase}");

            ApiResponse<LoginResponseDto>? api;
            try
            {
                api = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<LoginResponseDto>>(
                    raw,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Response bukan ApiResponse JSON. Status={(int)resp.StatusCode} {resp.ReasonPhrase}. Raw={raw}",
                    ex
                );
            }

            if (api is null)
                throw new Exception($"ApiResponse null. Status={(int)resp.StatusCode}. Raw={raw}");

            if (resp.IsSuccessStatusCode && api.Success)
            {
                if (api.Data is null)
                    throw new Exception($"Login sukses tapi data null. Raw={raw}");
                return api.Data;
            }

            // error dari server
            var msg = string.IsNullOrWhiteSpace(api.Message)
                ? $"Login gagal. Status={(int)resp.StatusCode} {resp.ReasonPhrase}. Raw={raw}"
                : api.Message;

            if (resp.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException(msg);

            throw new Exception(msg);
        }
    }
}