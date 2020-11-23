using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;

namespace CodeReader.Controllers
{
    public class UploadFileController : ApiController
    {
        public HttpResponseMessage Post()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            using (var client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    if (httpRequest.Files.Count > 0)
                    {
                        foreach (string file in httpRequest.Files)
                        {
                            var postedFile = httpRequest.Files[file];
                            byte[] imgData;
                            using (var reader = new BinaryReader(postedFile.InputStream))
                            {
                                imgData = reader.ReadBytes(postedFile.ContentLength);
                            }
                            var filename = Path.GetFileName(postedFile.FileName);
                            formData.Add(new StreamContent(new MemoryStream(imgData)), "image", filename);
                        }
                        
                        var response = client.PostAsync("http://api.qrserver.com/v1/read-qr-code/", formData,new JsonMediaTypeFormatter()).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            result = Request.CreateResponse(HttpStatusCode.OK,response.Content.ReadAsStringAsync());
                        }
                    }
                    else
                    {
                        result = Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                }
            }
            return result;
        }




    }
}
