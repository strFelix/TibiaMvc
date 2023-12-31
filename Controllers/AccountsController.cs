using Microsoft.AspNetCore.Mvc;
using TibiaMvc.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace TibiaMvc.Controllers;

public class AccountsController : Controller
{
    public string uriBase = "http://myprojects.somee.com/TibiaApi/Accounts/";

    [HttpGet]
    public ActionResult Index()
    {
        return View("RegisterAccounts");
    }

    public ActionResult Login()
    {
        return View("ValidateAccounts");
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(AccountViewModel a)
    {
        try
        {
            HttpClient httpClient = new HttpClient();
            string uriComplementary = "Create";

            var content = new StringContent(JsonConvert.SerializeObject(a));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementary, content);

            string serialized = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                TempData["Message"] =
                    string.Format("Account {0} successfully registered! Log in to access.", a.Username);
                return View("ValidateAccounts");
            }
            else
            {
                throw new SystemException(serialized);
            }

        }
        catch (SystemException ex)
        {
            TempData["MessageError"] = ex.Message;
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public async Task<ActionResult> ManageAsync()
    {
        try
        {
            HttpClient httpClient = new HttpClient();

            int login = int.Parse(HttpContext.Session.GetString("SessionIdAccount"));
            string uriComplementary = $"{login}";
            HttpResponseMessage response = await httpClient.GetAsync(uriBase + uriComplementary);
            string serialized = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                AccountViewModel a = await Task.Run(() =>
                    JsonConvert.DeserializeObject<AccountViewModel>(serialized));
                return View(a);
            }
            else
            {
                throw new System.Exception(serialized);
            }
        }
        catch (System.Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<ActionResult> ValidateAsync(AccountViewModel a)
    {
        try
        {
            HttpClient httpClient = new HttpClient();
            string uriComplementary = "Validate";

            var content = new StringContent(JsonConvert.SerializeObject(a));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementary, content);

            string serialized = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                AccountViewModel aLogin = JsonConvert.DeserializeObject<AccountViewModel>(serialized);

                HttpContext.Session.SetString("SessionTokenAccount", aLogin.Token);
                HttpContext.Session.SetString("SessionIdAccount", aLogin.Id.ToString());
                HttpContext.Session.SetString("SessionUsernameAccount", aLogin.Username);

                TempData["Message"] = string.Format("Welcome {0} your id is {1}!", aLogin.Username, aLogin.Id);
                return RedirectToAction("Index", "Characters");
            }
            else
            {
                throw new System.Exception(serialized);
            }
        }
        catch (System.Exception ex)
        {
            TempData["MessageError"] = ex.Message;
            return RedirectToAction("Login", "Accounts");
        }
    }

    public async Task<ActionResult> UpdateEmail(AccountViewModel a)
    {
        try
        {
            HttpClient httpClient = new HttpClient();
            string uriComplementary = "UpdateEmail";
            var content = new StringContent(JsonConvert.SerializeObject(a));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await httpClient.PutAsync(uriBase + uriComplementary, content);
            string serialized = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                TempData["Message"] = "Email changed successfully";
            }
            else
                throw new System.Exception(serialized);
        }
        catch (System.Exception ex)
        {
            TempData["MessageError"] = ex.Message;
        }
        return RedirectToAction("Manage");
    }

    [HttpPost]
    public async Task<ActionResult> UpdatePassword(AccountViewModel a)
    {
        try
        {
            HttpClient httpClient = new HttpClient();
            string token = HttpContext.Session.GetString("SessionTokenAccount");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string uriComplementar = "UpdatePassword";
            string username = HttpContext.Session.GetString("SessionUsernameAccount");
            a.Username = username;
            var content = new StringContent(JsonConvert.SerializeObject(a));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PutAsync(uriBase + uriComplementar, content);

            string serialized = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                TempData["Message"] = "Password changed successfully";
                return RedirectToAction("Manage");
            }
            else
                throw new System.Exception(serialized);
        }
        catch (System.Exception ex)
        {
            TempData["MessageError"] = ex.Message;
        }
        return RedirectToAction("Manage");
    }

    [HttpGet]
    public ActionResult Exit()
    {
        try
        {
            HttpContext.Session.Remove("SessionTokenAccount");
            HttpContext.Session.Remove("SessionIdAccount");
            HttpContext.Session.Remove("SessionUsernameAccount");

            return RedirectToAction("Index", "Home");
        }
        catch (System.Exception ex)
        {
            TempData["MessageError"] = ex.Message;
            return RedirectToAction("AccountInfo");
        }
    }
}

