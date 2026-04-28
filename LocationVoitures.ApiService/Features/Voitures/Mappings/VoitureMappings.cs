using LocationVoitures.ApiService.Domain;
using LocationVoitures.ApiService.Features.Voitures.Responses;

namespace LocationVoitures.ApiService.Features.Voitures.Mappings;

public static class VoitureMappings
{
    public static VoitureDto ToDto(this Voiture voiture, bool estDisponible)
    {
        return new VoitureDto
        {
            Immatriculation = voiture.Immatriculation,
            Marque = voiture.Marque,
            Modele = voiture.Modele,
            Categorie = voiture.Categorie ?? string.Empty,
            PrixParJour = voiture.PrixLocationParJour,
            EstDisponible = estDisponible
        };
    }
}
