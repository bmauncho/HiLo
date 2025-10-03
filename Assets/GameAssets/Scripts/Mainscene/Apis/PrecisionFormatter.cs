using System;
using System.Globalization;
using UnityEngine;

public static class PrecisionFormatter
{

    private static double precisionFormat ( double amount )
    {
        double truncatedValue = Math.Truncate(amount * 100) / 100.0;
        return truncatedValue;
    }

    public static string culturedFormat ( string amount , int decimalplaces = 0 )
    {
        // Parse the input amount (assumes it's in English/Invariant format like "151,586.62")
        double newAmount = double.Parse(amount , CultureInfo.InvariantCulture);

        // Apply precision logic (truncate)
        newAmount = precisionFormat(newAmount);

        // Determine effective decimal places
        bool hasDecimalPart = newAmount % 1 != 0;
        int effectiveDecimalPlaces = ( decimalplaces == 0 && hasDecimalPart ) ? 1 : decimalplaces;

        // Format to current culture (e.g., German users will get "151.586,62")
        string finalAmount = newAmount.ToString($"n{effectiveDecimalPlaces}" , CultureInfo.CurrentCulture);
        return finalAmount;
    }
}
