namespace Indigo.Customers
{
    public enum OrderType
    {
        Espresso,
        Cappuccino,
        Latte,
        Americano,
    }

    public enum Satisfaction
    {
        Happy,
        Neutral,
        Annoyed,
        Angry,
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

        public static int BasePrice(this OrderType t)
        {
            switch (t)
            {
                case OrderType.Espresso:   return 3;
                case OrderType.Americano:  return 4;
                case OrderType.Cappuccino: return 5;
                case OrderType.Latte:      return 6;
                default: return 3;
            }
        }
    }

    public static class SatisfactionExtensions
    {
        public static float TipMultiplier(this Satisfaction s)
        {
            switch (s)
            {
                case Satisfaction.Happy:   return 1.6f;
                case Satisfaction.Neutral: return 1.0f;
                case Satisfaction.Annoyed: return 0.5f;
                case Satisfaction.Angry:   return 0f;
                default: return 1f;
            }
        }

        public static UnityEngine.Color Color(this Satisfaction s)
        {
            switch (s)
            {
                case Satisfaction.Happy:   return new UnityEngine.Color(0.40f, 0.85f, 0.55f, 1f);
                case Satisfaction.Neutral: return new UnityEngine.Color(0.36f, 0.78f, 0.95f, 1f);
                case Satisfaction.Annoyed: return new UnityEngine.Color(0.98f, 0.74f, 0.30f, 1f);
                case Satisfaction.Angry:   return new UnityEngine.Color(0.92f, 0.36f, 0.32f, 1f);
                default: return UnityEngine.Color.white;
            }
        }
    }
}
