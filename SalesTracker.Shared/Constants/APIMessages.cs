
namespace SalesTracker.Shared.Constants
{
    public static class APIMessages
    {
        public const string ProductNotFound = "Product not found";
        public const string NoProduct = "No Product";
        public const string ProductsRetrieved = "Products retrieved successfully";
        public const string ProductRetrieved = "Product retrieved successfully";
        public const string ProductCreated = "Product created successfully";
        public const string ProductUpdated = "Product updated successfully";
        public const string ProductDeleted = "Product deleted successfully";
        public const string StockUpdated = "Stock updated successfully";
        public const string LowStockRetrieved = "Low stock products retrieved successfully";
        public const String NoLowStock = "All products have sufficient stock";
        public const string SearchResultsRetrieved = "Search results retrieved successfully";
        public const string CategorieRetrieved = "Product categories retrieved successfully.";
        public const string ProductReportGenerated = "Product sales report generated successfully.";

        public static string SearchKeywordNotFound(string keyword) => $"No products found matching '{keyword}'.";

        public const string SalesRetreived = "Sales retrieved successfully";
        public const string SaleRetreived = "Sale retrieved successfully";
        public const string SaleNotFound = "Sale not found.";
        public const string SaleNotCompleted = "Unable to mark sale as completed.";
        public const string SaleNotCancelled = "Unable to cancel the sale.";
        public const string SaleNotReturned = "Unable to return the sale.";
        public const string SaleCompleted = "Sale marked as completed.";
        public const string SaleCancelled = "Sale has been cancelled.";
        public const string SaleReturned = "Sale returned successfully.";
        public const string SaleCreated = "Sale created successfully.";
        public const string InvalidSale = "Invalid sale data.";
        public const string SalesReportGenerated = "Daily sales report generated successfully.";


        public const string TokenRefreshed = "Access token successfully refreshed.";
        public const string ExpiredOrInvalidToken = "Your session has expired. Please log in again.";
        public const string RefreshTokenMissing = "Refresh token is missing from the request.";
        public const string InvalidCredentials = "Invalid username or password.";
        public const string LoginSuccess = "User successfully logged in.";
        public const string RegisterSuccess = "User registered successfully.";
        public const string UsernameTaken = "This username is already taken.";
        public const string UnauthorizedAccess = "You do not have permission to access this resource.";
        public const string ServerError = "An unexpected error occurred.";
    }
}
