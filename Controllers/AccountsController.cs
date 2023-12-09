using Microsoft.AspNetCore.Mvc;
using TibiaMvc.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

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
                TempData["Mensagem"] =
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
            string uriComplementar = "Validate";

            var content = new StringContent(JsonConvert.SerializeObject(a));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementar, content);

            string serialized = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                AccountViewModel aLogin = JsonConvert.DeserializeObject<AccountViewModel>(serialized);

                HttpContext.Session.SetString("SessionTokenAccount", aLogin.Token);
                HttpContext.Session.SetString("SessionUsername", aLogin.Username);
                
                TempData["Mensagem"] = string.Format("Welcome {0}!", aLogin.Username);
                return RedirectToAction("Login", "Accounts");
            }
            else
            {
                throw new System.Exception(serialized);
            }
        }
        catch (System.Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
            return RedirectToAction("Login", "Accounts");
        }
    }
}
