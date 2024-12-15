// using System.Net.Http;
// using System.Text;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Newtonsoft.Json;
// using Xunit;

// public class BaseEmailControllerTest : IClassFixture<WebApplicationFactory<Program>>
// {
//     private readonly HttpClient _client;

//     public BaseEmailControllerTest(WebApplicationFactory<Program> factory)
//     {
//         _client = factory.CreateClient();
//     }

//     [Fact]
//     public async Task TestAddAndRemoveUserFromCompany()
//     {
//         // Log in user
//         var loginData = new { Email = "john.doe@CarAndAll.com", Password = "password" };
//         var loginContent = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
//         var loginResponse = await _client.PostAsync("/api/auth/login", loginContent);
//         loginResponse.EnsureSuccessStatusCode();
//         var loginResult = JsonConvert.DeserializeObject<dynamic>(await loginResponse.Content.ReadAsStringAsync());
//         string token = loginResult.token;

//         // Add user to company
//         var addUserData = new { Email = "newuser@CarAndAll.com" };
//         var addUserContent = new StringContent(JsonConvert.SerializeObject(addUserData), Encoding.UTF8, "application/json");
//         _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
//         var addUserResponse = await _client.PostAsync("/api/BaseEmailController/addUserToCompany", addUserContent);
//         addUserResponse.EnsureSuccessStatusCode();

//         // Remove user from company
//         var removeUserData = new { Email = "newuser@CarAndAll.com" };
//         var removeUserContent = new StringContent(JsonConvert.SerializeObject(removeUserData), Encoding.UTF8, "application/json");
//         var removeUserResponse = await _client.PostAsync("/api/BaseEmailController/removeUserFromCompany", removeUserContent);
//         removeUserResponse.EnsureSuccessStatusCode();
//     }
// }
