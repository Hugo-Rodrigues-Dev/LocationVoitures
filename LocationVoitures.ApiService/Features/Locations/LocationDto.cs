namespace LocationVoitures.ApiService.Features.Locations;

public record LocationDto(
    int Id,
    string Immatriculation,
    int LoueurId,
    DateOnly DateDebut,
    DateOnly DateFin,
    bool Annule);
