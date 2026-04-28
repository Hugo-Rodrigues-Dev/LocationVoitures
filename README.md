# TP1 - Compte rendu complet

## 1. Objectif du TP

Le but de ce TP est de construire une API REST de gestion de location de voitures avec :

- une base PostgreSQL
- Entity Framework Core en Code First
- une orchestration .NET Aspire
- une documentation OpenAPI consultable dans Scalar
- des tests unitaires et des tests de validation

Le projet final contient :

- `LocationVoitures.AppHost` : orchestrateur Aspire
- `LocationVoitures.ApiService` : API Minimal API
- `LocationVoitures.Web` : front fourni par le template Aspire
- `LocationVoitures.ServiceDefaults` : configuration partagée Aspire
- `LocationVoitures.Tests` : projet de tests NUnit

---

## 2. Prerequis

Pour refaire ce TP dans de bonnes conditions, il faut :

- .NET SDK 10
- Docker ou Podman
- un IDE C# correct
- PostgreSQL gere par Aspire

Si le projet est ouvert dans WSL, il faut installer le SDK .NET 10 aussi dans WSL.

Verification utile :

```bash
dotnet --list-sdks
dotnet --info
```

Le projet force le SDK avec `global.json` :

```json
{
  "sdk": {
    "version": "10.0.203"
  }
}
```

Si l'IDE ne trouve pas le SDK dans WSL, installation type sur Ubuntu 22.04 :

```bash
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-sdk-10.0
```

Puis verification :

```bash
dotnet --list-sdks
dotnet --info
```

---

## 3. Creation de la solution Aspire

Le projet a ete cree comme une solution Aspire Starter App.

Resultat attendu dans la solution :

- `LocationVoitures.AppHost`
- `LocationVoitures.ApiService`
- `LocationVoitures.Web`
- `LocationVoitures.ServiceDefaults`

Le projet de tests a ensuite ete ajoute :

```bash
dotnet new nunit -n LocationVoitures.Tests -f net10.0
dotnet sln LocationVoitures.sln add LocationVoitures.Tests/LocationVoitures.Tests.csproj
dotnet add LocationVoitures.Tests/LocationVoitures.Tests.csproj reference LocationVoitures.ApiService/LocationVoitures.ApiService.csproj
```

Packages principaux du projet de tests :

- `NUnit`
- `NUnit3TestAdapter`
- `Microsoft.NET.Test.Sdk`
- `FluentValidation`

---

## 4. Premier lancement Aspire

Commande principale :

```bash
dotnet run --project LocationVoitures.AppHost
```

URLs utiles :

- Dashboard Aspire : `https://localhost:17030`
- Dashboard Aspire HTTP : `http://localhost:15124`
- API : `https://localhost:7491`
- API HTTP : `http://localhost:5346`
- Web : `https://localhost:7130`
- Web HTTP : `http://localhost:5209`
- Scalar : `https://localhost:7491/scalar/v1`
- OpenAPI : `https://localhost:7491/openapi/v1.json`

### Point important sur WSL / proxy

Le dashboard Aspire a pose probleme a cause d'un proxy global.  
La correction a ete faite dans `LocationVoitures.AppHost/Properties/launchSettings.json` avec :

- `ASPIRE_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true`
- `NO_PROXY=localhost,127.0.0.1,::1`
- `no_proxy=localhost,127.0.0.1,::1`

Cela permet au dashboard et aux echanges gRPC internes de fonctionner correctement.

---

## 5. Structure de l'API

Le projet `LocationVoitures.ApiService` a ete structure de facon verticale et lisible.

Dossiers importants :

- `Data`
- `Domain`
- `Features`
- `Migrations`
- `Services`

Sous `Features`, l'API est rangee par fonctionnalite :

- `Features/Voitures`
- `Features/Locations`
- `Features/Loueurs`
- `Features/OpenApi`
- `Features/Validation`

Cette organisation evite d'avoir tous les controllers d'un cote, tous les DTOs d'un autre, et rend chaque fonctionnalite autonome.

---

## 6. Couche base de donnees

### 6.1. Packages installes

Dans l'API :

```bash
dotnet add LocationVoitures.ApiService package Microsoft.EntityFrameworkCore.Design
dotnet add LocationVoitures.ApiService package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add LocationVoitures.ApiService package Scalar.AspNetCore
dotnet add LocationVoitures.ApiService package FluentValidation.DependencyInjectionExtensions
```

Dans l'AppHost :

```bash
dotnet add LocationVoitures.AppHost package Aspire.Hosting.PostgreSQL
```

### 6.2. Modele de donnees

Trois entites metier ont ete creees dans `Domain` :

- `Voiture`
- `Loueur`
- `Location`

Points importants :

- `Voiture.Immatriculation` est unique
- `Loueur.Mobile` est unique
- `Loueur.Pays` vaut `France` par defaut
- `Location` relie un `Loueur` et une `Voiture`
- `Location.Annule` permet d'annuler une reservation sans la supprimer
- `Loueur.EstBlacklist` a ete ajoute dans la derniere partie pour interdire certaines reservations

### 6.3. DbContext

Le contexte EF est `RentalDbContext` dans `Data/RentalDbContext.cs`.

Il expose :

- `DbSet<Loueur> Loueurs`
- `DbSet<Voiture> Voitures`
- `DbSet<Location> Locations`

### 6.4. PostgreSQL dans Aspire

Dans `LocationVoitures.AppHost/AppHost.cs`, PostgreSQL est declare comme ressource Aspire :

```csharp
var postgres = builder.AddPostgres("postgres");
var rentalDb = postgres.AddDatabase("RentalDb");
```

L'API reference ensuite cette base :

```csharp
var apiService = builder.AddProject<Projects.LocationVoitures_ApiService>("apiservice")
    .WithReference(rentalDb)
    .WaitFor(rentalDb)
    .WithHttpHealthCheck("/health");
```

### 6.5. Connexion du DbContext dans l'API

Dans `Program.cs` :

```csharp
builder.AddNpgsqlDbContext<RentalDbContext>("RentalDb");
```

### 6.6. Migrations EF

Migration initiale :

```bash
dotnet ef migrations add InitialCreate --project LocationVoitures.ApiService --startup-project LocationVoitures.ApiService
```

Migration de la derniere partie pour les loueurs :

```bash
dotnet ef migrations add AddLoueurBlacklist --project LocationVoitures.ApiService --startup-project LocationVoitures.ApiService
```

### 6.7. Application automatique des migrations

Dans `Program.cs`, la migration est appliquee automatiquement au demarrage :

```csharp
await app.MigrateDatabaseAsync();
```

### 6.8. Seed de donnees

Un seed a ete ajoute dans `Data/SeedData.cs`.

Le seed cree :

- 2 loueurs
- 3 voitures
- 2 locations

Cela permet de tester tout de suite :

- le catalogue
- la disponibilite
- les reservations
- les annulations

---

## 7. Partie 4 - Code de test intermediaire

Le sujet proposait au debut un petit code de test temporaire dans `Program.cs`.  
Cette etape a ete depassee rapidement et remplacee par de vrais endpoints minimal API, ce qui est plus propre.

---

## 8. Partie 5.1 - Catalogue des voitures

### Ce qui a ete fait

- creation de `VoitureDto`
- ajout du endpoint `GET /voitures`
- ajout du filtre optionnel `categorie`
- calcul de `EstDisponible`

### Principe

Le endpoint liste les voitures et calcule leur disponibilite a la date du jour en verifiant si une location non annulee recouvre aujourd'hui.

### Test utile

Dans Scalar :

- `GET /voitures`
- `GET /voitures?categorie=Citadine`

---

## 9. Partie 5.2 - Reservation d'une location

### Ce qui a ete fait

- creation de `ReserverRequest`
- creation de `ReserverResponse`
- creation de `LocationService`
- ajout du endpoint `POST /locations`

### Logique metier

Le endpoint :

1. valide la requete
2. verifie que la voiture existe
3. verifie que le loueur existe
4. verifie que le loueur n'est pas blackliste
5. verifie que la voiture est libre sur la periode
6. calcule le prix total
7. cree la location

### Exemple de payload

```json
{
  "immatriculation": "AA-123-BB",
  "loueurId": 1,
  "dateDebut": "2026-05-10",
  "dateFin": "2026-05-12"
}
```

### Point de conception

La logique de calcul et les controles metier critiques ont ete mis dans `LocationService` au lieu d'etre ecrits directement dans l'endpoint.

---

## 10. Partie 5.3 - Autres fonctionnalites

### 10.1. Endpoints Voitures

Ajouts :

- `GET /voitures/{id}`
- `GET /voitures/immatriculation/{immatriculation}`
- `POST /voitures`
- `PUT /voitures/{id}`
- `PATCH /voitures/{id}/prix`

### 10.2. Endpoints Locations

Ajouts :

- `GET /locations`
- `GET /locations/voiture/{immatriculation}`
- `PATCH /locations/{id}/annulation`

### Ce qu'il faut comprendre

- `POST` cree une ressource
- `PUT` remplace l'etat complet
- `PATCH` modifie partiellement
- les annulations passent par un flag `Annule`, ce qui preserve l'historique

---

## 11. Partie 6 - Tests unitaires et validation

### 11.1. Projet de tests

Le projet `LocationVoitures.Tests` a ete ajoute a la solution.

Commandes utiles :

```bash
dotnet sln LocationVoitures.sln add LocationVoitures.Tests/LocationVoitures.Tests.csproj
dotnet add LocationVoitures.Tests/LocationVoitures.Tests.csproj reference LocationVoitures.ApiService/LocationVoitures.ApiService.csproj
```

Organisation retenue pour garder les tests lisibles :

- `LocationVoitures.Tests/Services`
- `LocationVoitures.Tests/Features/Voitures`
- `LocationVoitures.Tests/Features/Locations`
- `LocationVoitures.Tests/Features/Loueurs`

L'idee est de refléter la meme logique que dans l'API :

- les tests de logique metier vont dans `Services`
- les tests de validation et de contrats vont dans `Features/<FeatureName>`

### 11.2. Tests du service metier

Fichier :

- `LocationVoitures.Tests/Services/LocationServiceTests.cs`

Cas testes :

- `120 EUR/jour` sur `3 jours` donne `360`
- `DateFin = DateDebut`
- passage a l'annee suivante
- calcul avec prix decimal
- exception si `DateFin < DateDebut`
- exception si le loueur est blackliste
- absence d'exception si le loueur n'est pas blackliste

Lancement :

```bash
dotnet test LocationVoitures.Tests/LocationVoitures.Tests.csproj
```

### 11.3. Validation avec FluentValidation

Validateurs ajoutes pour :

- `CreateVoitureRequest`
- `UpdatePrixVoitureRequest`
- `ReserverRequest`
- `CreateLoueurRequest`

### 11.4. Tests de validation

Fichier :

- `LocationVoitures.Tests/Features/Voitures/CreateVoitureRequestValidatorTests.cs`
- `LocationVoitures.Tests/Features/Voitures/UpdatePrixVoitureRequestValidatorTests.cs`
- `LocationVoitures.Tests/Features/Locations/ReserverRequestValidatorTests.cs`
- `LocationVoitures.Tests/Features/Loueurs/CreateLoueurRequestValidatorTests.cs`

Regles testees :

- format immatriculation
- marque obligatoire
- `LoueurId` obligatoire
- dates coherentes
- prix positif
- mobile valide pour un loueur
- email valide pour un loueur
- pays obligatoire pour un loueur

---

## 12. Partie 7 - Fonctionnalites des loueurs

Cette partie a ete implementee proprement et reliee au reste de l'application.

### 12.1. Nouveaux DTOs et validateurs

Ajouts dans `Features/Loueurs` :

- `LoueurDto`
- `CreateLoueurRequest`
- `UpdateBlacklistLoueurRequest`
- `CreateLoueurRequestValidator`
- `LoueurMappings`

### 12.2. Endpoints Loueurs

Ajouts :

- `GET /loueurs`
- `GET /loueurs/{id}`
- `GET /loueurs/mobile/{mobile}`
- `POST /loueurs`
- `PUT /loueurs/{id}`
- `PATCH /loueurs/{id}/blacklist`

### 12.3. Regle metier de blacklist

Un loueur peut etre blacklist via :

- `PATCH /loueurs/{id}/blacklist`

Une fois blackliste :

- il ne peut plus reserver de voiture
- la regle est appliquee dans `LocationService`
- l'endpoint `POST /locations` renvoie un `409 Conflict`

### Exemple de payload pour blacklister

```json
{
  "estBlacklist": true
}
```

---

## 13. Documentation API dans Scalar

La documentation API a ete amelioree pour qu'une personne arrive sur le projet et comprenne directement l'usage de l'API.

Ce qui a ete ajoute :

- `tags` OpenAPI
- `summary`
- `description`
- types de reponses explicites
- DTOs documentes
- endpoint racine `/` documente

Tags actuellement utilises :

- `Informations`
- `Voitures`
- `Locations`
- `Loueurs`

Scalar :

- `https://localhost:7491/scalar/v1`

OpenAPI brut :

- `https://localhost:7491/openapi/v1.json`

---

## 14. Structure finale du code

### Dossiers de l'API

- `Data`
- `Domain`
- `Features/Voitures`
- `Features/Locations`
- `Features/Loueurs`
- `Features/OpenApi`
- `Features/Validation`
- `Migrations`
- `Services`

### Idee de maintenabilite

Le projet a ete structure pour qu'un developpeur puisse :

- retrouver vite une fonctionnalite
- lire la logique metier sans parcourir toute la solution
- separer la persistance, la validation, les DTOs et les endpoints
- documenter proprement les contrats HTTP

---

## 15. Commandes utiles du projet

### Lancer l'orchestrateur Aspire

```bash
dotnet run --project LocationVoitures.AppHost
```

### Lancer uniquement l'API

```bash
dotnet run --project LocationVoitures.ApiService
```

### Lancer les tests

```bash
dotnet test LocationVoitures.Tests/LocationVoitures.Tests.csproj
```