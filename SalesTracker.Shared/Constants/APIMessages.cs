
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

    }
}
