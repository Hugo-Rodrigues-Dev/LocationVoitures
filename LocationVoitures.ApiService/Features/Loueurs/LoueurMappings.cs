using LocationVoitures.ApiService.Domain;

namespace LocationVoitures.ApiService.Features.Loueurs;

public static class LoueurMappings
{
    public static LoueurDto ToDto(this Loueur loueur)
    {
        return new LoueurDto
        {
            Id = loueur.Id,
            Nom = loueur.Nom,
            Prenom = loueur.Prenom,
            Mobile = loueur.Mobile,
            Email = loueur.Email,
            Pays = loueur.Pays,
            EstBlacklist = loueur.EstBlacklist
        };
    }
}
