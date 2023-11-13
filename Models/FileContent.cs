using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Task2_updated.Models
{
   public enum QueryType
    {
        Create,
        Update,
        Delete,
        Retrieve
    }
    public class FileContent
    {
       
        public string FileName { get; set; }

        public string Owner { get; set; }

        public IFormFile? File { get; set; }
        public string? Description { get; set; }

        public QueryType OperationType { get; set; }

    }

    

}
