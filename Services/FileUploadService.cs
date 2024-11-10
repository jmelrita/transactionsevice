using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TransactionService.Models;

namespace TransactionService.Services
{
    public class FileUploadService
    {
        public List<string> ValidateFile(IFormFile file)
        {
            List<string> errorMessages = new List<string>();
            if (file.Length > 1024 * 1024) // 1MB limit
            {
                errorMessages.Add("File size exceeds the maximum limit of 1MB.");
            }

            var supportedExtensions = new[] { ".csv" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!supportedExtensions.Contains(fileExtension))
            {
                errorMessages.Add("Invalid file format. Only CSV files are allowed.");
            }

            return errorMessages;
        }

        public List<Transaction> ParseFile(IFormFile file, out List<string> validationErrors)
        {
            validationErrors = new List<string>();
            var transactions = new List<Transaction>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var lineNumber = 0;
                while (!reader.EndOfStream)
                {
                    lineNumber++;
                    var line = reader.ReadLine();
                    var fields = line.Split(',');

                    if (fields.Length != 8)
                    {
                        validationErrors.Add($"Line {lineNumber} has an invalid number of fields.");
                        continue;
                    }

                    // Validate individual fields here (length, format, etc.)
                    if (fields[0].Length > 20) validationErrors.Add($"Line {lineNumber}: Reference Number is too long.");
                    if (fields[5].Length < 3 || fields[5].Length > 5) validationErrors.Add($"Line {lineNumber}: Symbol length must be between 3 and 5 characters.");

                    var transaction = new Transaction
                    {
                        ReferenceNumber = fields[0],
                        Quantity = long.Parse(fields[1]),
                        Amount = decimal.Parse(fields[2]),
                        Name = fields[3],
                        TransactionDate = DateTime.ParseExact(fields[4], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                        Symbol = fields[5],
                        OrderSide = fields[6],
                        OrderStatus = fields[7]
                    };

                    transactions.Add(transaction);
                }
            }

            return transactions;
        }
    }
}
