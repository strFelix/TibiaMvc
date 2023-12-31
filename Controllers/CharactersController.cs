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
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("SessionIdAccount")))
                return RedirectToAction("Exit", "Accounts");

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
            TempData["MessageError"] = ex.Message;
            return RedirectToAction("Index");
        }
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

            if (c.AccountId != id)
                throw new Exception("Invalid Account");

            string serialized = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                TempData["Message"] = string.Format("Character {0} has been created!", c.Name);
                return RedirectToAction("Index", "Characters");
            }
            else
                throw new System.Exception(serialized);
        }
        catch (System.Exception ex)
        {
            TempData["MessageError"] = ex.Message;
            return RedirectToAction("Create");
        }
    }

    [HttpGet]
    public async Task<ActionResult> DetailsAsync(int? id)
    {
        try
        {
            HttpClient httpClient = new HttpClient();
            string token = HttpContext.Session.GetString("SessionTokenUsuario");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
             string uriComplementary = "skill/";
            HttpResponseMessage response = await httpClient.GetAsync(uriBase + uriComplementary + id.ToString());
            string serialized = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                SkillViewModel s = await Task.Run(() =>
                    JsonConvert.DeserializeObject<SkillViewModel>(serialized));
                return View(s);
            }
            else
                throw new System.Exception(serialized);
        }
        catch (System.Exception ex)
        {
            TempData["MessageError"] = ex.Message;
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await httpClient.DeleteAsync(uriBase + id.ToString());
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Message"] =
                        string.Format("Character {0} has been deleted!", id);
                    return RedirectToAction("Index");
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MessageError"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
}