namespace Indigo.Customers
{
    public enum OrderType
    {
        Espresso,
        Cappuccino,
        Latte,
        Americano,
    }

    public static class OrderTypeExtensions
    {
        public static string DisplayName(this OrderType t)
        {
            switch (t)
            {
                case OrderType.Espresso:   return "Espresso";
                case OrderType.Cappuccino: return "Cappuccino";
                case OrderType.Latte:      return "Latte";
                case OrderType.Americano:  return "Americano";
                default: return t.ToString();
            }
        }
    }
}
