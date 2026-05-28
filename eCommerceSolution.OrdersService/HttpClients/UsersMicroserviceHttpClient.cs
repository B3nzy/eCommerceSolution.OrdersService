namespace eCommerceSolution.OrdersService.HttpClients;

public class UsersMicroserviceHttpClient    
{
    private readonly HttpClient _httpClient;

    public UsersMicroserviceHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<bool> UserExistsAsync(Guid userId)
    {
        HttpResponseMessage httpResponse = await _httpClient.GetAsync($"api/auth/search/{userId}");

        if (httpResponse.IsSuccessStatusCode)
        {
            return true;
        }
        return false;
    }


}
