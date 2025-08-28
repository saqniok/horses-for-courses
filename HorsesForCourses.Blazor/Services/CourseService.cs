using System.Net.Http.Json;
using HorsesForCourses.Blazor.Dtos;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HorsesForCourses.Blazor.Services;

public class CourseService : ICourseService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public CourseService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<List<CourseDto>> GetCoursesAsync()
    {
        var response = await _httpClient.GetAsync("courses");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<CourseDto>>(_jsonSerializerOptions);
        return result ?? new List<CourseDto>();
    }

    public async Task DeleteCourseAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"courses/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task ConfirmCourseAsync(int id)
    {
        var response = await _httpClient.PostAsync($"courses/{id}/confirm", null);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            
            // Try to parse error message from response
            try
            {
                var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent, _jsonSerializerOptions);
                
                // Check for different error message formats
                if (errorResponse.TryGetProperty("title", out var title))
                {
                    throw new HttpRequestException($"Error confirming course: {title.GetString()}");
                }
                else if (errorResponse.TryGetProperty("message", out var message))
                {
                    throw new HttpRequestException($"Error confirming course: {message.GetString()}");
                }
                else if (errorResponse.TryGetProperty("errors", out var errors))
                {
                    var errorMessages = new List<string>();
                    foreach (var error in errors.EnumerateObject())
                    {
                        if (error.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var errorMsg in error.Value.EnumerateArray())
                            {
                                errorMessages.Add($"{error.Name}: {errorMsg.GetString()}");
                            }
                        }
                    }
                    if (errorMessages.Any())
                    {
                        throw new HttpRequestException($"Validation errors: {string.Join(", ", errorMessages)}");
                    }
                }
            }
            catch (JsonException)
            {
                // If we can't parse JSON, try to extract meaningful error from plain text
                if (errorContent.Contains("InvalidOperationException"))
                {
                    // Try to extract the main error message from the exception
                    var lines = errorContent.Split('\n');
                    foreach (var line in lines)
                    {
                        if (line.Contains("System.InvalidOperationException:"))
                        {
                            var errorMsg = line.Substring(line.IndexOf("System.InvalidOperationException:") + "System.InvalidOperationException:".Length).Trim();
                            if (errorMsg.Contains(" at "))
                            {
                                errorMsg = errorMsg.Substring(0, errorMsg.IndexOf(" at ")).Trim();
                            }
                            throw new HttpRequestException($"Cannot confirm course: {errorMsg}");
                        }
                    }
                }
            }
            
            // Fallback to generic error with status code
            throw new HttpRequestException($"Error confirming course: {response.StatusCode} - {errorContent}");
        }
    }

    public async Task AddCourseAsync(CreateCourseRequestDto course)
    {
        var response = await _httpClient.PostAsJsonAsync("courses", course, _jsonSerializerOptions);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            
            // Try to parse error message from response
            try
            {
                var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent, _jsonSerializerOptions);
                
                // Check for different error message formats
                if (errorResponse.TryGetProperty("title", out var title))
                {
                    throw new HttpRequestException($"Error creating course: {title.GetString()}");
                }
                else if (errorResponse.TryGetProperty("message", out var message))
                {
                    throw new HttpRequestException($"Error creating course: {message.GetString()}");
                }
                else if (errorResponse.TryGetProperty("errors", out var errors))
                {
                    var errorMessages = new List<string>();
                    foreach (var error in errors.EnumerateObject())
                    {
                        if (error.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var errorMsg in error.Value.EnumerateArray())
                            {
                                errorMessages.Add($"{error.Name}: {errorMsg.GetString()}");
                            }
                        }
                    }
                    if (errorMessages.Any())
                    {
                        throw new HttpRequestException($"Validation errors: {string.Join(", ", errorMessages)}");
                    }
                }
            }
            catch (JsonException)
            {
                // If we can't parse JSON, use the raw content
            }
            
            // Fallback to generic error with status code
            throw new HttpRequestException($"Error creating course: {response.StatusCode} - {errorContent}");
        }
    }
    
    public async Task UpdateCourseAsync(int id, CourseDto course)
    {
        var response = await _httpClient.PutAsJsonAsync($"courses/{id}", course, _jsonSerializerOptions);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            
            // Try to parse error message from response
            try
            {
                var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent, _jsonSerializerOptions);
                
                // Check for different error message formats
                if (errorResponse.TryGetProperty("title", out var title))
                {
                    throw new HttpRequestException($"Error updating course: {title.GetString()}");
                }
                else if (errorResponse.TryGetProperty("message", out var message))
                {
                    throw new HttpRequestException($"Error updating course: {message.GetString()}");
                }
                else if (errorResponse.TryGetProperty("errors", out var errors))
                {
                    var errorMessages = new List<string>();
                    foreach (var error in errors.EnumerateObject())
                    {
                        if (error.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var errorMsg in error.Value.EnumerateArray())
                            {
                                errorMessages.Add($"{error.Name}: {errorMsg.GetString()}");
                            }
                        }
                    }
                    if (errorMessages.Any())
                    {
                        throw new HttpRequestException($"Validation errors: {string.Join(", ", errorMessages)}");
                    }
                }
            }
            catch (JsonException)
            {
                // If we can't parse JSON, use the raw content
            }
            
            // Fallback to generic error with status code
            throw new HttpRequestException($"Error updating course: {response.StatusCode} - {errorContent}");
        }
    }

    public async Task AssignCoachAsync(int courseId, int coachId)
    {
        var request = new { CoachId = coachId };
        var response = await _httpClient.PostAsJsonAsync($"courses/{courseId}/assigncoach", request, _jsonSerializerOptions);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            
            // Try to parse error message from response
            try
            {
                var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent, _jsonSerializerOptions);
                
                // Check for different error message formats
                if (errorResponse.TryGetProperty("title", out var title))
                {
                    throw new HttpRequestException($"Error assigning coach: {title.GetString()}");
                }
                else if (errorResponse.TryGetProperty("message", out var message))
                {
                    throw new HttpRequestException($"Error assigning coach: {message.GetString()}");
                }
                else if (errorResponse.TryGetProperty("errors", out var errors))
                {
                    var errorMessages = new List<string>();
                    foreach (var error in errors.EnumerateObject())
                    {
                        if (error.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var errorMsg in error.Value.EnumerateArray())
                            {
                                errorMessages.Add($"{error.Name}: {errorMsg.GetString()}");
                            }
                        }
                    }
                    if (errorMessages.Any())
                    {
                        throw new HttpRequestException($"Validation errors: {string.Join(", ", errorMessages)}");
                    }
                }
            }
            catch (JsonException)
            {
                // If we can't parse JSON, try to extract meaningful error from plain text
                if (errorContent.Contains("Coach does not have all required skills"))
                {
                    throw new HttpRequestException("Cannot assign coach: The selected coach does not have all the required skills for this course.");
                }
                else if (errorContent.Contains("InvalidOperationException"))
                {
                    // Try to extract the main error message from the exception
                    var lines = errorContent.Split('\n');
                    foreach (var line in lines)
                    {
                        if (line.Contains("System.InvalidOperationException:"))
                        {
                            var errorMsg = line.Substring(line.IndexOf("System.InvalidOperationException:") + "System.InvalidOperationException:".Length).Trim();
                            if (errorMsg.Contains(" at "))
                            {
                                errorMsg = errorMsg.Substring(0, errorMsg.IndexOf(" at ")).Trim();
                            }
                            throw new HttpRequestException($"Error assigning coach: {errorMsg}");
                        }
                    }
                }
            }
            
            // Fallback to generic error with status code
            throw new HttpRequestException($"Error assigning coach: {response.StatusCode} - {errorContent}");
        }
    }
}
