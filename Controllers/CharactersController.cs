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

public class CharactersController : Controller
{
    public string uriBase = "http://myprojects.somee.com/TibiaApi/Characters/";

    public ActionResult Create()
    {
        return View("Create");
    }

    public async Task<ActionResult> IndexAsync()
    {
        try
        {
            string uriComplementary = "";
            HttpClient httpClient = new HttpClient();
            string token = HttpContext.Session.GetString("SessionTokenAccount");
            int id = int.Parse(HttpContext.Session.GetString("SessionIdAccount"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await httpClient.GetAsync(uriBase + uriComplementary);
            string serialized = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<CharacterViewModel> charactersSearch = await Task.Run(() =>
                    JsonConvert.DeserializeObject<List<CharacterViewModel>>(serialized));

                List<CharacterViewModel> characters = charactersSearch.Where(c => c.AccountId == id).ToList();

                return View(characters);
            }
            else
                throw new System.Exception(serialized);
        }
        catch (System.Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
            return RedirectToAction("Index");
        }
    }

    public async Task<int> SessionId()
    {
        int id = int.Parse(HttpContext.Session.GetString("SessionIdAccount"));
        return id;
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(CharacterViewModel c)
    {
        try
        {

            HttpClient httpClient = new HttpClient();
            string uriComplementary = "create";

            var content = new StringContent(JsonConvert.SerializeObject(c));
            int id = int.Parse(HttpContext.Session.GetString("SessionIdAccount"));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplementary, content);
            
            if(c.AccountId != id)
                throw new Exception("Invalid Account");

            string serialized = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                TempData["Mensagem"] = string.Format("Character {0} has been created successfully!", c.Name);
                return RedirectToAction("Index", "Characters");
            }
            else
                throw new System.Exception(serialized);
        }
        catch (System.Exception ex)
        {
            TempData["MensagemErro"] = ex.Message;
            return RedirectToAction("Create");
        }
    }
}