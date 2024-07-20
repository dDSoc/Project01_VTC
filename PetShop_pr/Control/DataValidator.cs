public static class DataValidator
{
    // Method to validate year (must be a number between 1900 and current year)
    public static bool ValidateYear(string yearStr)
    {
        if (int.TryParse(yearStr, out int year))
        {
            int currentYear = DateTime.Now.Year;
            return year >= 1900 && year <= currentYear;
        }
        return false;
    }

    // Method to validate quantity (must be a positive integer)
    public static bool ValidateQuantity(string quantityStr)
    {
        if (int.TryParse(quantityStr, out int quantity))
        {
            return quantity > 0;
        }
        return false;
    }

    // Method to validate price (must be a positive decimal)
    public static bool ValidatePrice(string priceStr)
    {
        if (decimal.TryParse(priceStr, out decimal price))
        {
            return price > 0;
        }
        return false;
    }

    // Method to validate username (must be alphanumeric and length between 5 and 15 characters)
    public static bool ValidateUsername(string username)
    {
        if (!string.IsNullOrWhiteSpace(username) && username.Length >= 5 && username.Length <= 15)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9]+$");
        }
        return false;
    }

    // Method to validate full name (must not be empty and length should be within a reasonable range)
    public static bool ValidateFullName(string fullName)
    {
        return !string.IsNullOrWhiteSpace(fullName) && fullName.Length <= 100;
    }

    // Method to validate category name (must not be empty and must be alphanumeric)
    public static bool ValidateCategoryName(string categoryName)
    {
        // Check if the category name is not null, not empty, and has a length of 100 characters or less
        if (!string.IsNullOrWhiteSpace(categoryName) && categoryName.Length <= 100)
        {
            // Use a regular expression to check if the category name contains only letters and spaces
            return System.Text.RegularExpressions.Regex.IsMatch(categoryName, @"^[a-zA-Z\s]+$");
        }
        // Return false if the category name does not meet the above conditions
        return false;
    }

    // Method to validate description for category (must not be empty and length should be within a reasonable range)
    public static bool ValidateDescriptionCategory(string descriptionCategory)
    {
        return !string.IsNullOrWhiteSpace(descriptionCategory) && descriptionCategory.Length <= 250;
    }

    // Method to validate product name (must not be empty, must be alphanumeric and special characters, but must start with a letter)
    public static bool ValidateProductName(string productName)
    {
        // Check if the product name is not null, not empty, and has a length of 100 characters or less
        if (!string.IsNullOrWhiteSpace(productName) && productName.Length <= 100)
        {
            // Use a regular expression to check if the product name starts with a letter and contains letters, numbers, and special characters
            return System.Text.RegularExpressions.Regex.IsMatch(productName, @"^[a-zA-Z][a-zA-Z0-9\s\W]*$");
        }
        // Return false if the product name does not meet the above conditions
        return false;
    }


    // Method to validate description for product (must not be empty and length should be within a reasonable range)
    public static bool ValidateDescriptionProduct(string descriptionProduct)
    {
        return !string.IsNullOrWhiteSpace(descriptionProduct) && descriptionProduct.Length <= 250;
    }

    // Method to validate ID (must be an integer between 0 and 1,000,000)
    public static bool ValidateID(string idStr)
    {
        if (int.TryParse(idStr, out int id))
        {
            return id >= 0 && id <= 1000000;
        }
        return false;
    }

    public static bool ValidateEmail(string email)
    {
        // Check if the email is not null, not empty, and matches the regular expression pattern for a valid email address
        if (!string.IsNullOrWhiteSpace(email))
        {
            // Use a regular expression to validate the email format
            return System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
        // Return false if the email is null, empty, or does not match the pattern
        return false;
    }

    // Method to validate address (must not be empty and length should be within a reasonable range)
    public static bool ValidateAddress(string address)
    {
        return !string.IsNullOrWhiteSpace(address) && address.Length <= 200;
    }
}
