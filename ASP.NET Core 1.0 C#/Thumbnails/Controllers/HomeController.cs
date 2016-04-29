using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;

namespace Thumbnails.Controllers
{
    public class HomeController : Controller
    {
        //_apiKey: Replace this with your own Cognitive Services Computer Vision API key, please do not use my key. I include it here so you can get up and running quickly but you can get your own key for free at https://www.microsoft.com/cognitive-services/en-us/computer-vision-api
        public const string _apiKey = "382f5abd65f74494935027f65a41a4bc";

        //_apiUrl: The base URL for the API. Find out what this is for other APIs via the API documentation
        public const string _apiUrl = "https://api.projectoxford.ai/vision/v1.0/generateThumbnail";

        public IActionResult Index()
        {
            return View();
        }

        // GET: Home/FileExample
        public IActionResult FileExample()
        {
            return View();
        }

        // POST: Home/FileExample
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FileExample(IFormFile file)
        {
            //set original file to in view data so we can compare to cropped
            using (var fileStream = file.OpenReadStream())
            {
                byte[] result;
                using (var streamReader = new MemoryStream())
                {
                    fileStream.CopyTo(streamReader);
                    result = streamReader.ToArray();
                }
                ViewData["originalImage"] = "data:image/png;base64," + Convert.ToBase64String(result);
            }
            

            //get form data
            var width = (string.IsNullOrEmpty(Request.Form["width"].ToString())) ?
                "100" :
                Request.Form["width"].ToString();

            var height = (string.IsNullOrEmpty(Request.Form["height"].ToString())) ?
                "100" :
                Request.Form["height"].ToString();

            var smartcropping = (string.IsNullOrEmpty(Request.Form["smartcropping"].ToString())) ?
                "false" :
                "true";

            using (var httpClient = new HttpClient())
            {
                //setup HttpClient
                httpClient.BaseAddress = new Uri(_apiUrl);
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);

                //setup data object
                HttpContent content = new StreamContent(file.OpenReadStream());
                content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/octet-stream");

                // Request parameters
                var uri = _apiUrl + string.Format("?width={0}&height={1}&smartCropping={2}", width, height, smartcropping);

                //make request
                var response = await httpClient.PostAsync(uri, content);

                //read response and write to view
                var responseContent = await response.Content.ReadAsByteArrayAsync();
                //ViewData["Result"] = responseContent;
                ViewData["thumbnailImage"] = "data:image/png;base64," + Convert.ToBase64String(responseContent);
            }

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
