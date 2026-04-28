namespace LocationVoitures.ApiService.Services;

public class LocationService
{
    public decimal CalculerPrixTotal(decimal prixJour, DateOnly debut, DateOnly fin)
    {
        ValiderPeriode(debut, fin);

        var nombreDeJours = fin.DayNumber - debut.DayNumber + 1;
        return prixJour * nombreDeJours;
    }

    public void ValiderPeriode(DateOnly debut, DateOnly fin)
    {
        if (fin < debut)
        {
            throw new ArgumentException("La date de fin doit etre superieure ou egale a la date de debut.");
        }
    }
}
