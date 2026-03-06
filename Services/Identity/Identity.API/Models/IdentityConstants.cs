namespace Identity.API.Models;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Customer = "Customer";

    public static readonly IReadOnlyList<string> All = [Admin, Customer];

    // Permissions granted to each role at seed time
    public static readonly IReadOnlyList<string> AdminPermissions =
    [
        Permissions.Products.Read,
        Permissions.Products.Write,
        Permissions.Catalog.Read,
        Permissions.Catalog.Write,
        Permissions.Orders.Read,
        Permissions.Orders.Write,
        Permissions.Orders.Manage,
        Permissions.Discounts.Read,
        Permissions.Discounts.Write,
        Permissions.Users.Read,
        Permissions.Users.Write,
    ];

    public static readonly IReadOnlyList<string> CustomerPermissions =
    [
        Permissions.Products.Read,
        Permissions.Catalog.Read,
        Permissions.Orders.Read,
        Permissions.Orders.Write,
        Permissions.Discounts.Read,
    ];
}

public static class Permissions
{
    public sealed record ResourcePermissions(string Resource)
    {
        public string Read  => $"{Resource}:read";
        public string Write => $"{Resource}:write";
    }

    public static class Products
    {
        private static readonly ResourcePermissions _base = new("products");
        public static string Read  => _base.Read;
        public static string Write => _base.Write;
    }

    public static class Catalog
    {
        private static readonly ResourcePermissions _base = new("catalog");
        public static string Read  => _base.Read;
        public static string Write => _base.Write;
    }

    public static class Orders
    {
        private static readonly ResourcePermissions _base = new("orders");
        public static string Read   => _base.Read;
        public static string Write  => _base.Write;
        public const string Manage  = "orders:manage";
    }

    public static class Discounts
    {
        private static readonly ResourcePermissions _base = new("discounts");
        public static string Read  => _base.Read;
        public static string Write => _base.Write;
    }

    public static class Users
    {
        private static readonly ResourcePermissions _base = new("users");
        public static string Read  => _base.Read;
        public static string Write => _base.Write;
    }
}
