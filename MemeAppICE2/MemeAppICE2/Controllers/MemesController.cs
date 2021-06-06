using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using memes.Models;

//using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

using System.Text.Json.Serialization;


namespace memes.Controllers
{
    public class MemesController : Controller
    {
        public MemesController()
        {

        }

        public async Task<IActionResult> GetMemes()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var msg = await client.GetStringAsync("https://api.imgflip.com/get_memes");

            Root DeserializedObject = JsonSerializer.Deserialize<Root>(msg);


            //Console.Write(msg);
            return View(DeserializedObject.data.memes);
        }

        public IActionResult Generate(string id, string url)
        {
            ViewBag.id = id;
            ViewBag.url = url;
            return View();
        }





        [HttpPost]
        public async Task<IActionResult> Generate(string caption1, string caption2, string id)
        {
            HttpClient client = new HttpClient();

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Post;

            httpRequestMessage.RequestUri = new Uri("https://api.imgflip.com/caption_image?template_id=" + id
                + "&username=[your username]&password=[your password]=" + caption1
                + "&text1=" + caption2);

            HttpResponseMessage resp = await client.SendAsync(httpRequestMessage);
            string result = resp.Content.ReadAsStringAsync().Result;
            GeneratedRoot g = JsonSerializer.Deserialize<GeneratedRoot>(result);

            return RedirectToAction("Result", new { url = g.data.url });

            // if you had to send an object across as JSON
            //Uri uri = new Uri("https://api.imgflip.com/caption_image");
            //RequestModel rm = new RequestModel("[your username]","[your password]",caption1, caption2);
            //string jsonContent = JsonSerializer.Serialize(rm);
            //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonContent);
            //ByteArrayContent b = new ByteArrayContent(buffer);
            //b.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //HttpResponseMessage resp = await client.postAsync(uri,b);




        }

        public IActionResult Result(string url)
        {
            ViewBag.url = url;
            return View();
        }

    }
}
