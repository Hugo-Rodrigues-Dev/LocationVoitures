namespace LocationVoitures.ApiService.Features.Voitures;

public record VoitureDto(
    string Immatriculation,
    string Marque,
    string Modele,
    string Categorie,
    decimal PrixParJour,
    bool EstDisponible);
