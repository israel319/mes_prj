namespace AppPlusPlus.Services;

/// <summary>
/// Convertit un montant décimal en lettres (français).
/// Exemple : 12 000,50 → "douze mille francs et cinquante centimes"
/// </summary>
public static class NumberToWordsService
{
    private static readonly string[] Unites =
    {
        "", "un", "deux", "trois", "quatre", "cinq", "six", "sept", "huit", "neuf",
        "dix", "onze", "douze", "treize", "quatorze", "quinze", "seize",
        "dix-sept", "dix-huit", "dix-neuf"
    };

    private static readonly string[] Dizaines =
    {
        "", "dix", "vingt", "trente", "quarante", "cinquante",
        "soixante", "soixante", "quatre-vingt", "quatre-vingt"
    };

    /// <summary>
    /// Convertit un montant en lettres avec devise.
    /// </summary>
    /// <param name="montant">Montant décimal</param>
    /// <param name="devise">Nom de la devise (ex: "francs")</param>
    /// <param name="centime">Nom des centimes (ex: "centimes")</param>
    public static string Convert(decimal montant, string devise = "francs", string centime = "centimes")
    {
        if (montant == 0) return $"zéro {devise}";

        var abs = Math.Abs(montant);
        var partieEntiere = (long)Math.Truncate(abs);
        var partieDecimale = (int)Math.Round((abs - partieEntiere) * 100);

        var result = "";

        if (partieEntiere > 0)
        {
            result = ConvertirNombre(partieEntiere) + " " + devise;
        }
        else
        {
            result = $"zéro {devise}";
        }

        if (partieDecimale > 0)
        {
            result += " et " + ConvertirNombre(partieDecimale) + " " + centime;
        }

        if (montant < 0)
            result = "moins " + result;

        // Première lettre en majuscule
        return char.ToUpper(result[0]) + result[1..];
    }

    private static string ConvertirNombre(long n)
    {
        if (n == 0) return "zéro";
        if (n < 0) return "moins " + ConvertirNombre(-n);

        var parties = new List<string>();

        if (n >= 1_000_000_000_000)
        {
            var trillions = n / 1_000_000_000_000;
            if (trillions == 1)
                parties.Add("un billion");
            else
                parties.Add(ConvertirNombre(trillions) + " billions");
            n %= 1_000_000_000_000;
        }

        if (n >= 1_000_000_000)
        {
            var milliards = n / 1_000_000_000;
            if (milliards == 1)
                parties.Add("un milliard");
            else
                parties.Add(ConvertirNombre(milliards) + " milliards");
            n %= 1_000_000_000;
        }

        if (n >= 1_000_000)
        {
            var millions = n / 1_000_000;
            if (millions == 1)
                parties.Add("un million");
            else
                parties.Add(ConvertirNombre(millions) + " millions");
            n %= 1_000_000;
        }

        if (n >= 1000)
        {
            var milliers = n / 1000;
            if (milliers == 1)
                parties.Add("mille");
            else
                parties.Add(ConvertirMoinsQueMille(milliers) + " mille");
            n %= 1000;
        }

        if (n > 0)
        {
            parties.Add(ConvertirMoinsQueMille(n));
        }

        return string.Join(" ", parties).Trim();
    }

    private static string ConvertirMoinsQueMille(long n)
    {
        if (n >= 100)
        {
            var centaines = n / 100;
            var reste = n % 100;

            string result;
            if (centaines == 1)
                result = "cent";
            else
                result = Unites[centaines] + " cent";

            // Accord de "cent" : prend un "s" si multiple de 100 exact
            if (reste == 0 && centaines > 1)
                result += "s";
            else if (reste > 0)
                result += " " + ConvertirMoinsQueCent(reste);

            return result;
        }

        return ConvertirMoinsQueCent(n);
    }

    private static string ConvertirMoinsQueCent(long n)
    {
        if (n < 20)
            return Unites[n];

        var dizaine = n / 10;
        var unite = n % 10;

        // Cas spéciaux : 70-79, 90-99
        switch (dizaine)
        {
            case 7: // soixante-dix, soixante-et-onze ...
                if (unite == 1)
                    return "soixante-et-onze";
                return "soixante-" + Unites[10 + unite];

            case 9: // quatre-vingt-dix ...
                return "quatre-vingt-" + Unites[10 + unite];

            case 8: // quatre-vingt, quatre-vingt-un ...
                if (unite == 0)
                    return "quatre-vingts"; // avec "s"
                return "quatre-vingt-" + Unites[unite];

            default:
                if (unite == 0)
                    return Dizaines[dizaine];
                if (unite == 1 && dizaine <= 6)
                    return Dizaines[dizaine] + "-et-un";
                return Dizaines[dizaine] + "-" + Unites[unite];
        }
    }
}
