using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Task2_updated.Models;

namespace Task2_updated.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        /*
        [HttpGet("DownloadFile")]
        public IActionResult DownloadFile(string fileName )
        {
            var filePath = Path.Combine("Files", fileName);

            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileContentType = "application/octet-stream"; // You can set the appropriate content type for your file here
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                return Ok(fileStream);
            }

            return NotFound("File not found");
        }*/



        [HttpPost("operations")]
        public IActionResult Main([FromForm] FileContent model)
        {
          

            var fileExtension = model.File != null && model.File.ContentType == "image/jpeg" ? "jpg" : "mp4";

            switch (model.OperationType)
            {
                case QueryType.Create:
                    if (model.File == null || model.FileName == null || model.Owner==null || model.OperationType==null)
                    {
                        return BadRequest("all fields required");
                    }
                    else
                    {
                        bool FileExistsc(string fileName, string fileExtension)
                        {

                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", $"{fileName}.{fileExtension}");
                            return System.IO.File.Exists(filePath);
                        }
                        if (FileExistsc(model.FileName, fileExtension))
                        {
                            return BadRequest("File already exist.");
                        }
                        if (model.File.ContentType != "image/jpeg" && model.File.ContentType != "video/mp4")
                        {
                            return BadRequest("File type not supported. Please upload a jpg or mp4 file.");
                        }


                        var filePath = Path.Combine("Files", $"{model.FileName}.{fileExtension}");
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            model.File.CopyTo(stream);
                        }


                        var jsonContent = new
                        {
                            Filename = $"{model.FileName}.{fileExtension}",
                            Owner = model.Owner,
                            Description = model.Description,

                        };
                        var jsonFilePath = Path.Combine("Files", $"{model.FileName}.json");
                        System.IO.File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(jsonContent));




                        return Ok(new
                        {
                            Message = "File created successfully.",
                            FileName = $"{model.FileName}.{fileExtension}",
                            Owner = model.Owner,
                            Description = model.Description
                        });
                    }

                   
                   

                case QueryType.Update:
                    var jsonFilePathToUpdate = Path.Combine("Files", $"{model.FileName}.json");
                    if (System.IO.File.Exists(jsonFilePathToUpdate))
                    {
                        var existingJsonContent = System.IO.File.ReadAllText(jsonFilePathToUpdate);
                        var existingMetadata = JsonConvert.DeserializeObject<FileContent>(existingJsonContent);


                        try
                        {




                            if (model.File != null && model.Description == null)
                            {



                                var filePathu = Path.Combine("Files", $"{model.FileName}.{fileExtension}");
                                using (var stream = new FileStream(filePathu, FileMode.Create))
                                {
                                    model.File.CopyTo(stream);
                                }

                                var existingFileExtension = existingMetadata.FileName.Split('.').Last();
                                var newFileExtension = model.File.FileName.Split('.').Last();


                                var oldFilePath = Path.Combine("Files", $"{model.FileName}.{existingFileExtension}");
                                if (System.IO.File.Exists(oldFilePath))
                                {
                                    System.IO.File.Delete(oldFilePath);
                                }

                                existingMetadata.FileName = $"{model.FileName}.{fileExtension}";

                                System.IO.File.WriteAllText(jsonFilePathToUpdate, JsonConvert.SerializeObject(existingMetadata));
                            }
                            else if (model.File == null && model.Description != null)
                            {
                                existingMetadata.Description = model.Description;

                                System.IO.File.WriteAllText(jsonFilePathToUpdate, JsonConvert.SerializeObject(existingMetadata));
                            }
                            else if (model.File != null && model.Description != null)
                            {
                                var filePathu = Path.Combine("Files", $"{model.FileName}.{fileExtension}");
                                using (var stream = new FileStream(filePathu, FileMode.Create))
                                {
                                    model.File.CopyTo(stream);
                                }

                                var existingFileExtension = existingMetadata.FileName.Split('.').Last();
                                var newFileExtension = model.File.FileName.Split('.').Last();


                                var oldFilePath = Path.Combine("Files", $"{model.FileName}.{existingFileExtension}");
                                if (System.IO.File.Exists(oldFilePath))
                                {
                                    System.IO.File.Delete(oldFilePath);
                                }

                                existingMetadata.FileName = $"{model.FileName}.{fileExtension}";
                                existingMetadata.Description = model.Description;

                                System.IO.File.WriteAllText(jsonFilePathToUpdate, JsonConvert.SerializeObject(existingMetadata));
                            }
                            else
                            {
                                return Ok("choose to change description or file.");
                            }
                        }


                        catch (Exception)
                        {

                            throw;
                        }

                    }
                    else
                    {
                        return Ok("file doesn't exist");
                    }

                   

                

                    return Ok("File Updated");

                case QueryType.Delete:
                    var jsonFilePathTodelete = Path.Combine("Files", $"{model.FileName}.json");
                    if (System.IO.File.Exists(jsonFilePathTodelete))
                    {
                        if (model.File == null && model.Description == null)
                        {
                            var jsonFilePathTod1 = Path.Combine("Files", $"{model.FileName}.json");
                            var existingJsond1 = System.IO.File.ReadAllText(jsonFilePathTod1);
                            var existingMetad1 = JsonConvert.DeserializeObject<FileContent>(existingJsond1);

                            var fileToDelete = Path.Combine("Files", $"{model.FileName}.{existingMetad1.FileName.Split('.').Last()}");
                            var jsonToDelete = Path.Combine("Files", $"{model.FileName}.json");
                            System.IO.File.Delete(fileToDelete);
                            System.IO.File.Delete(jsonToDelete);
                            return Ok("File deleted successfully.");

                        }
                        else
                        {
                            return Ok("only file name and owner");
                        }

                    }
                    else
                    {
                        return Ok("file doesn't exist");
                    }



                case QueryType.Retrieve:

                    var jsonFilePathToret = Path.Combine("Files", $"{model.FileName}.json");
                    if (System.IO.File.Exists(jsonFilePathToret))
                    {
                        if (model.File == null && model.Description == null)
                        {

                            var jsonFilePathTor = Path.Combine("Files", $"{model.FileName}.json");
                            var existingJsonr = System.IO.File.ReadAllText(jsonFilePathTor);
                            var existingMetad = JsonConvert.DeserializeObject<FileContent>(existingJsonr);
                            var dd = existingMetad.FileName.Split('.').Last();

                            var filePath1 = Path.Combine("Files", $"{model.FileName}.{dd}");
                            var jsonFilePath1 = Path.Combine("Files", $"{model.FileName}.json");

                            if (!System.IO.File.Exists(filePath1) || !System.IO.File.Exists(jsonFilePath1))
                            {
                                return BadRequest("File or metadata not found.");
                            }

                            var fileContent = System.IO.File.ReadAllBytes(filePath1);
                            var jsonContent1 = System.IO.File.ReadAllText(jsonFilePath1);

                            var fileStream = new FileStream(filePath1, FileMode.Open, FileAccess.Read);



                            var fileResponse = new
                            {
                                FileContent = Convert.ToBase64String(fileContent),
                                FileName = model.FileName,
                                FileOwner = model.Owner,
                                Description = JsonConvert.DeserializeObject<FileContent>(jsonContent1).Description
                            };

                            /*var downloadLink = Url.Action("DownloadFile", new { fileName = $"{model.FileName}.{fileExtension}" });

                            var structuredResponse = new
                            {
                                FileResponse = fileResponse,
                                DownloadLink = downloadLink
                            };*/

                            Response.Headers.Add("File Name", $"{model.FileName}.{dd}");
                            Response.Headers.Add("File-Owner", model.Owner);
                            Response.Headers.Add("File-Description", JsonConvert.DeserializeObject<FileContent>(jsonContent1).Description);
                            return Ok(fileStream);
                        }
                        else
                        {
                            return Ok("only filname and owner");
                        }

                        //return Ok(structuredResponse);
                        //return Ok(fileResponse);
                        //return Ok(fileStream);

                    }
                    else
                    {
                        return Ok("File doesn't exist");
                    }

                default:
                    return Ok("Invalid operation type.");
            }

        }
        
    }
}
