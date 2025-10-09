using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace GCFoundation.Web.Controllers
{
    [ApiController]
    [Route("api/grid")]
    public class GridDataController : Controller
    {
        private static readonly HashSet<string> EmployeeSortAllowList = new(StringComparer.OrdinalIgnoreCase)
        {
            nameof(Employee.Id), nameof(Employee.Name), nameof(Employee.Department)
        };

        private static readonly HashSet<string> ArticleSortAllowList = new(StringComparer.OrdinalIgnoreCase)
        {
            nameof(Article.Title), nameof(Article.Author), nameof(Article.Summary)
        };
        [HttpGet("employees")]
        [Produces("application/json")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult GetEmployees([FromQuery] GridQuery query)
        {
            var all = Enumerable.Range(1, 250).Select(i => new Employee
            {
                Id = i,
                Name = $"Employee {i}",
                Department = i % 3 == 0 ? "Finance" : i % 3 == 1 ? "HR" : "IT"
            });

            var pd = ValidateQuery(query, EmployeeSortAllowList);
            if (pd != null) return BadRequest(pd);

            if (!string.IsNullOrWhiteSpace(query.Q))
            {
                var q = query.Q.Trim();
                all = all.Where(e =>
                    e.Id.ToString().Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    (e.Name?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.Department?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }
            all = ApplySort(all, query.SortBy, query.SortDir);

            return Ok(BuildResponse(all, query));
        }

        [HttpGet("articles")]
        [Produces("application/json")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult GetArticles([FromQuery] GridQuery query)
        {
            var all = Enumerable.Range(1, 120).Select(i => new Article
            {
                Title = $"Article {i}",
                Author = i % 2 == 0 ? "Jane Doe" : "John Smith",
                Summary = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer posuere."
            });

            var pd = ValidateQuery(query, ArticleSortAllowList);
            if (pd != null) return BadRequest(pd);

            if (!string.IsNullOrWhiteSpace(query.Q))
            {
                var q = query.Q.Trim();
                all = all.Where(a =>
                    (a.Title?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.Author?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (a.Summary?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }
            all = ApplySort(all, query.SortBy, query.SortDir);

            return Ok(BuildResponse(all, query));
        }

        private static object BuildResponse<T>(IEnumerable<T> source, GridQuery query)
        {
            var page = Math.Max(1, query.Page);
            var pageSize = Math.Clamp(query.PageSize <= 0 ? 25 : query.PageSize, 1, 100);

            var total = source.Count();
            var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new
            {
                items,
                total,
                page,
                pageSize
            };
        }

        private static IEnumerable<Employee> ApplySort(IEnumerable<Employee> src, string? sortBy, string? dir)
        {
            var d = string.Equals(dir, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
            return sortBy?.ToLowerInvariant() switch
            {
                "id" => d == "desc" ? src.OrderByDescending(e => e.Id) : src.OrderBy(e => e.Id),
                "name" => d == "desc" 
                    ? src.OrderByDescending(e => ExtractNumberFromString(e.Name)).ThenByDescending(e => e.Name) 
                    : src.OrderBy(e => ExtractNumberFromString(e.Name)).ThenBy(e => e.Name),
                "department" => d == "desc" ? src.OrderByDescending(e => e.Department) : src.OrderBy(e => e.Department),
                _ => src
            };
        }

        private static IEnumerable<Article> ApplySort(IEnumerable<Article> src, string? sortBy, string? dir)
        {
            var d = string.Equals(dir, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
            return sortBy?.ToLowerInvariant() switch
            {
                "title" => d == "desc" 
                    ? src.OrderByDescending(e => ExtractNumberFromString(e.Title)).ThenByDescending(e => e.Title) 
                    : src.OrderBy(e => ExtractNumberFromString(e.Title)).ThenBy(e => e.Title),
                "author" => d == "desc" ? src.OrderByDescending(e => e.Author) : src.OrderBy(e => e.Author),
                "summary" => d == "desc" ? src.OrderByDescending(e => e.Summary) : src.OrderBy(e => e.Summary),
                _ => src
            };
        }

        private static int ExtractNumberFromString(string? value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            
            // Extract first sequence of digits from the string
            var match = System.Text.RegularExpressions.Regex.Match(value, @"\d+");
            return match.Success && int.TryParse(match.Value, out var number) ? number : 0;
        }

        private static ProblemDetails? ValidateQuery(GridQuery query, HashSet<string> allowList)
        {
            if (!string.IsNullOrEmpty(query.SortBy) && !allowList.Contains(query.SortBy))
            {
                return new ProblemDetails
                {
                    Title = "Invalid sortBy",
                    Detail = $"Field '{query.SortBy}' is not allowed.",
                    Status = StatusCodes.Status400BadRequest
                };
            }
            if (!string.IsNullOrEmpty(query.SortDir) && !(string.Equals(query.SortDir, "asc", StringComparison.OrdinalIgnoreCase) || string.Equals(query.SortDir, "desc", StringComparison.OrdinalIgnoreCase)))
            {
                return new ProblemDetails
                {
                    Title = "Invalid sortDir",
                    Detail = "sortDir must be 'asc' or 'desc'.",
                    Status = StatusCodes.Status400BadRequest
                };
            }
            if (!string.IsNullOrEmpty(query.Q) && query.Q!.Length > 200)
            {
                return new ProblemDetails
                {
                    Title = "Query too long",
                    Detail = "q must be 200 characters or fewer.",
                    Status = StatusCodes.Status400BadRequest
                };
            }
            return null;
        }

        public sealed class GridQuery
        {
            [FromQuery(Name = "page")] public int Page { get; set; } = 1;
            [FromQuery(Name = "pageSize")] public int PageSize { get; set; } = 25;
            [FromQuery(Name = "sortBy")] public string? SortBy { get; set; }
            [FromQuery(Name = "sortDir")] public string? SortDir { get; set; }
            [FromQuery(Name = "q")] public string? Q { get; set; }
        }

        public sealed class Employee
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
        }

        public sealed class Article
        {
            public string Title { get; set; } = string.Empty;
            public string Author { get; set; } = string.Empty;
            public string Summary { get; set; } = string.Empty;
        }
    }
}


