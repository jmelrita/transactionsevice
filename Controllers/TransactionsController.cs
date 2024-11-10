using Microsoft.AspNetCore.Mvc;
using System.Globalization;

[Route("api/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TransactionController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadCsv([FromForm] IFormFile file)
    {
        if (file.Length == 0 || file.Length > 1048576) // 1MB max size
        {
            return BadRequest("File size must be under 1MB.");
        }

        List<Transaction> transactions = new List<Transaction>();
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            var lineNumber = 0;
            while (!reader.EndOfStream)
            {
                lineNumber++;
                var line = await reader.ReadLineAsync();
                var values = line.Split(',');

                if (values.Length != 9) // Ensure all columns are present
                {
                    return BadRequest($"Invalid record at line {lineNumber}. Column count mismatch.");
                }

                // Validate each field here and add to transactions list if valid
                var transaction = new Transaction
                {
                    ReferenceNumber = values[0],
                    Quantity = long.Parse(values[1]),
                    Amount = decimal.Parse(values[2]),
                    Name = values[3],
                    TransactionDate = DateTime.ParseExact(values[4], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    Symbol = values[5],
                    OrderSide = values[6],
                    OrderStatus = values[7]
                };

                // Add validation logic for each field (e.g., unique ReferenceNumber, length checks)
                // If any record is invalid, flag the whole file as invalid and return error message
                transactions.Add(transaction);
            }
        }

        // Save to database
        _context.Transactions.AddRange(transactions);
        await _context.SaveChangesAsync();

        return Ok("File uploaded successfully.");
    }

    [HttpGet("transactions-by-symbol")]
    public IActionResult GetBySymbol([FromQuery] string symbol)
    {
        var transactions = _context.Transactions.Where(t => t.Symbol == symbol).ToList();
        return Ok(transactions);
    }

    [HttpGet("transactions-by-date-range")]
    public IActionResult GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var transactions = _context.Transactions
            .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
            .ToList();
        return Ok(transactions);
    }

    [HttpGet("transactions-by-order-side")]
    public IActionResult GetByOrderSide([FromQuery] string orderSide)
    {
        var transactions = _context.Transactions
            .Where(t => t.OrderSide.Equals(orderSide, StringComparison.OrdinalIgnoreCase))
            .ToList();
        return Ok(transactions);
    }

    [HttpGet("transactions-by-status")]
    public IActionResult GetByOrderStatus([FromQuery] string orderStatus)
    {
        var transactions = _context.Transactions
            .Where(t => t.OrderStatus.Equals(orderStatus, StringComparison.OrdinalIgnoreCase))
            .ToList();
        return Ok(transactions);
    }
}
