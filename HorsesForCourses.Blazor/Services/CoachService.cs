using System.Net.Http.Json;
using HorsesForCourses.Blazor.Dtos;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HorsesForCourses.Blazor.Services;

public class CoachService : ICoachService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public CoachService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<List<CoachSummaryResponse>> GetCoachesAsync()
    {
        var response = await _httpClient.GetAsync("coaches");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<CoachSummaryResponse>>(_jsonSerializerOptions);
        return result ?? new List<CoachSummaryResponse>();
    }

    public async Task<CoachDetailsDto> GetCoachDetailsAsync(int id)
    {
        var response = await _httpClient.GetAsync($"coaches/{id}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CoachDetailsDto>(_jsonSerializerOptions);
        return result ?? throw new InvalidOperationException("Coach not found");
    }

    public async Task AddCoachAsync(CreateCoachRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("coaches", request, _jsonSerializerOptions);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateCoachAsync(int id, CoachDetailsDto coach)
    {
        var response = await _httpClient.PutAsJsonAsync($"coaches/{id}", coach, _jsonSerializerOptions);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateCoachSkillsAsync(int id, UpdateCoachSkillsDto skills)
    {
        var response = await _httpClient.PostAsJsonAsync($"coaches/{id}/skills", skills, _jsonSerializerOptions);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteCoachAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"coaches/{id}");
        response.EnsureSuccessStatusCode();
    }
}