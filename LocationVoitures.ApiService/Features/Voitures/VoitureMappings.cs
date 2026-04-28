using LocationVoitures.ApiService.Domain;

namespace LocationVoitures.ApiService.Features.Voitures;

public static class VoitureMappings
{
    public static VoitureDto ToDto(this Voiture voiture, bool estDisponible)
    {
        return new VoitureDto(
            voiture.Immatriculation,
            voiture.Marque,
            voiture.Modele,
            voiture.Categorie ?? string.Empty,
            voiture.PrixLocationParJour,
            estDisponible);
    }
}
