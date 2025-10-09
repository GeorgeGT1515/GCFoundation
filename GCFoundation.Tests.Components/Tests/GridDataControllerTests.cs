using Microsoft.AspNetCore.Mvc;
using Xunit;
using GCFoundation.Web.Controllers;

namespace GCFoundation.Tests.Components.Tests
{
    /// <summary>
    /// Tests for GridDataController to verify natural sorting and data filtering.
    /// </summary>
    public class GridDataControllerTests
    {
        [Fact]
        public void GetEmployees_SortByName_Ascending_ReturnsNaturalOrder()
        {
            // Arrange
            var controller = new GridDataController();
            var query = new GridDataController.GridQuery
            {
                Page = 1,
                PageSize = 25,
                SortBy = "name",
                SortDir = "asc"
            };

            // Act
            var result = controller.GetEmployees(query) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            
            var data = result.Value;
            var itemsProperty = data?.GetType().GetProperty("items");
            var items = itemsProperty?.GetValue(data) as IEnumerable<GridDataController.Employee>;
            
            Assert.NotNull(items);
            var itemsList = items.ToList();
            
            // Verify first 15 items are in natural numeric order
            Assert.Equal("Employee 1", itemsList[0].Name);
            Assert.Equal("Employee 2", itemsList[1].Name);
            Assert.Equal("Employee 3", itemsList[2].Name);
            Assert.Equal("Employee 4", itemsList[3].Name);
            Assert.Equal("Employee 5", itemsList[4].Name);
            Assert.Equal("Employee 6", itemsList[5].Name);
            Assert.Equal("Employee 7", itemsList[6].Name);
            Assert.Equal("Employee 8", itemsList[7].Name);
            Assert.Equal("Employee 9", itemsList[8].Name);
            Assert.Equal("Employee 10", itemsList[9].Name);
            Assert.Equal("Employee 11", itemsList[10].Name);
            Assert.Equal("Employee 12", itemsList[11].Name);
            Assert.Equal("Employee 13", itemsList[12].Name);
            Assert.Equal("Employee 14", itemsList[13].Name);
            Assert.Equal("Employee 15", itemsList[14].Name);
        }

        [Fact]
        public void GetEmployees_SortByName_Descending_ReturnsReverseNaturalOrder()
        {
            // Arrange
            var controller = new GridDataController();
            var query = new GridDataController.GridQuery
            {
                Page = 1,
                PageSize = 10,
                SortBy = "name",
                SortDir = "desc"
            };

            // Act
            var result = controller.GetEmployees(query) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var data = result.Value;
            var itemsProperty = data?.GetType().GetProperty("items");
            var items = itemsProperty?.GetValue(data) as IEnumerable<GridDataController.Employee>;
            
            Assert.NotNull(items);
            var itemsList = items.ToList();
            
            // Verify items are in reverse natural numeric order (250 down to 241)
            Assert.Equal("Employee 250", itemsList[0].Name);
            Assert.Equal("Employee 249", itemsList[1].Name);
            Assert.Equal("Employee 248", itemsList[2].Name);
        }

        [Fact]
        public void GetEmployees_SortById_ReturnsNumericOrder()
        {
            // Arrange
            var controller = new GridDataController();
            var query = new GridDataController.GridQuery
            {
                Page = 1,
                PageSize = 5,
                SortBy = "id",
                SortDir = "asc"
            };

            // Act
            var result = controller.GetEmployees(query) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var data = result.Value;
            var itemsProperty = data?.GetType().GetProperty("items");
            var items = itemsProperty?.GetValue(data) as IEnumerable<GridDataController.Employee>;
            
            Assert.NotNull(items);
            var itemsList = items.ToList();
            
            // Verify IDs are in ascending numeric order
            for (int i = 0; i < itemsList.Count; i++)
            {
                Assert.Equal(i + 1, itemsList[i].Id);
            }
        }

        [Fact]
        public void GetEmployees_WithSearch_FiltersResults()
        {
            // Arrange
            var controller = new GridDataController();
            var query = new GridDataController.GridQuery
            {
                Page = 1,
                PageSize = 100,
                Q = "Finance"
            };

            // Act
            var result = controller.GetEmployees(query) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var data = result.Value;
            var itemsProperty = data?.GetType().GetProperty("items");
            var items = itemsProperty?.GetValue(data) as IEnumerable<GridDataController.Employee>;
            
            Assert.NotNull(items);
            var itemsList = items.ToList();
            
            // All results should have "Finance" in the department
            Assert.All(itemsList, item => Assert.Equal("Finance", item.Department));
        }

        [Fact]
        public void GetEmployees_InvalidSortField_ReturnsBadRequest()
        {
            // Arrange
            var controller = new GridDataController();
            var query = new GridDataController.GridQuery
            {
                Page = 1,
                PageSize = 25,
                SortBy = "invalid_field" // Not in the allowlist
            };

            // Act
            var result = controller.GetEmployees(query);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GetEmployees_InvalidSortDirection_ReturnsBadRequest()
        {
            // Arrange
            var controller = new GridDataController();
            var query = new GridDataController.GridQuery
            {
                Page = 1,
                PageSize = 25,
                SortBy = "name",
                SortDir = "invalid" // Should be "asc" or "desc"
            };

            // Act
            var result = controller.GetEmployees(query);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GetEmployees_QueryTooLong_ReturnsBadRequest()
        {
            // Arrange
            var controller = new GridDataController();
            var query = new GridDataController.GridQuery
            {
                Page = 1,
                PageSize = 25,
                Q = new string('x', 201) // Over the 200 character limit
            };

            // Act
            var result = controller.GetEmployees(query);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GetEmployees_ReturnsCorrectPaginationMetadata()
        {
            // Arrange
            var controller = new GridDataController();
            var query = new GridDataController.GridQuery
            {
                Page = 2,
                PageSize = 25
            };

            // Act
            var result = controller.GetEmployees(query) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var data = result.Value;
            
            var pageProperty = data?.GetType().GetProperty("page");
            var pageSizeProperty = data?.GetType().GetProperty("pageSize");
            var totalProperty = data?.GetType().GetProperty("total");
            
            Assert.Equal(2, pageProperty?.GetValue(data));
            Assert.Equal(25, pageSizeProperty?.GetValue(data));
            Assert.Equal(250, totalProperty?.GetValue(data));
        }

        [Fact]
        public void GetArticles_SortByTitle_ReturnsNaturalOrder()
        {
            // Arrange
            var controller = new GridDataController();
            var query = new GridDataController.GridQuery
            {
                Page = 1,
                PageSize = 15,
                SortBy = "title",
                SortDir = "asc"
            };

            // Act
            var result = controller.GetArticles(query) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var data = result.Value;
            var itemsProperty = data?.GetType().GetProperty("items");
            var items = itemsProperty?.GetValue(data) as IEnumerable<GridDataController.Article>;
            
            Assert.NotNull(items);
            var itemsList = items.ToList();
            
            // Verify articles are in natural numeric order
            Assert.Equal("Article 1", itemsList[0].Title);
            Assert.Equal("Article 2", itemsList[1].Title);
            Assert.Equal("Article 3", itemsList[2].Title);
            Assert.Equal("Article 10", itemsList[9].Title);
            Assert.Equal("Article 11", itemsList[10].Title);
        }
    }
}

