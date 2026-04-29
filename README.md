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
- `pgAdmin` : interface web d'administration PostgreSQL exposee par Aspire

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

En plus, `pgAdmin` apparait dans le dashboard Aspire avec une URL HTTP dediee.

### Point important sur WSL / proxy

Le dashboard Aspire a pose probleme a cause d'un proxy global.La correction a ete faite dans `LocationVoitures.AppHost/Properties/launchSettings.json` avec :

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

- `Features/Voitures/Endpoints`
- `Features/Voitures/Requests`
- `Features/Voitures/Responses`
- `Features/Voitures/Validators`
- `Features/Voitures/Mappings`
- `Features/Locations/Endpoints`
- `Features/Locations/Requests`
- `Features/Locations/Responses`
- `Features/Locations/Validators`
- `Features/Loueurs/Endpoints`
- `Features/Loueurs/Requests`
- `Features/Loueurs/Responses`
- `Features/Loueurs/Validators`
- `Features/Loueurs/Mappings`

Les elements transverses ont ete deplaces dans :

- `Common/OpenApi`
- `Common/Validation`

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
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin(pgAdmin =>
    {
        pgAdmin.WithEnvironment("PGADMIN_DEFAULT_EMAIL", "admin@locationvoitures.fr");
        pgAdmin.WithEnvironment("PGADMIN_DEFAULT_PASSWORD", "Admin123!");
    });
var rentalDb = postgres.AddDatabase("RentalDb");
```

Cette configuration a deux effets :

- Aspire demarre PostgreSQL dans un conteneur
- Aspire demarre aussi `pgAdmin` pour visualiser les donnees de la base

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

### 6.9. Visualisation avec pgAdmin

`pgAdmin` permet de verifier visuellement les tables, les colonnes et les donnees seedees sans requete SQL manuelle.

#### Acces

1. lancer l'orchestrateur :

```bash
dotnet run --project LocationVoitures.AppHost
```

2. ouvrir le dashboard Aspire :

- `https://localhost:17030`

3. dans `Resources`, cliquer sur la ligne `pgadmin`
4. se connecter avec :

- email : `admin@locationvoitures.fr`
- mot de passe : `Admin123!`

L'URL de `pgAdmin` n'est pas fixe. Elle est allouee dynamiquement par Aspire et s'affiche dans la colonne `URL` de la ressource `pgadmin` dans le dashboard.

#### Connexion au serveur PostgreSQL

Une fois dans `pgAdmin`, ajouter un serveur si besoin avec les informations visibles dans Aspire :

- Host : `postgres`
- Port : `5432`
- Username : `postgres`
- Password : mot de passe genere par Aspire pour la ressource PostgreSQL

Dans la pratique, le plus simple est de recuperer les informations exactes depuis le dashboard Aspire dans la ressource `postgres`.

#### Donnees a verifier

Une fois connecte, on peut parcourir :

- `Databases`
- `RentalDb`
- `Schemas`
- `public`
- `Tables`

Puis verifier en particulier :

- `loueur`
- `voiture`
- `location`

On doit retrouver :

- les loueurs seedes
- les voitures seedees
- les locations seedees
- l'etat `annule`
- l'etat `est_blacklist`

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

- `Requests/CreateLoueurRequest`
- `Requests/UpdateBlacklistLoueurRequest`
- `Responses/LoueurDto`
- `Validators/CreateLoueurRequestValidator`
- `Mappings/LoueurMappings`

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

### Outil complementaire

En plus de Scalar pour la partie HTTP, `pgAdmin` permet de verifier la persistance reelle des donnees en base.

---

## 14. Structure finale du code

### Dossiers de l'API

- `Data`
- `Domain`
- `Features/Voitures/Endpoints`
- `Features/Voitures/Requests`
- `Features/Voitures/Responses`
- `Features/Voitures/Validators`
- `Features/Voitures/Mappings`
- `Features/Locations/Endpoints`
- `Features/Locations/Requests`
- `Features/Locations/Responses`
- `Features/Locations/Validators`
- `Features/Loueurs/Endpoints`
- `Features/Loueurs/Requests`
- `Features/Loueurs/Responses`
- `Features/Loueurs/Validators`
- `Features/Loueurs/Mappings`
- `Common/OpenApi`
- `Common/Validation`
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

### Lancer les tests

```bash
dotnet test LocationVoitures.Tests/LocationVoitures.Tests.csproj
```

### Lancer uniquement l'API

```bash
dotnet run --project LocationVoitures.ApiService
```

### Ouvrir pgAdmin

1. lancer `LocationVoitures.AppHost`
2. ouvrir le dashboard Aspire
3. cliquer sur la ressource `pgadmin`
4. se connecter avec :

```text
admin@locationvoitures.fr
Admin123!
```

---

## 16. Partie 2 - Etape 1 : ajout de la gateway YARP

La premiere etape de la partie 2 consiste a intercaler une gateway devant l'API.
L'objectif est d'obtenir un point d'entree HTTP unique capable de router les requetes vers `LocationVoitures.ApiService`.

### 16.1. Pourquoi ajouter une gateway

Meme si notre application reste monolithique, la gateway apporte deja plusieurs avantages :

- un point d'entree unique pour les clients
- une future centralisation de la securite
- une future centralisation du CORS
- la possibilite d'ajouter d'autres services sans changer l'URL connue par les clients

### 16.2. Creation du projet

Le projet a ete cree comme une application ASP.NET Core vide :

```bash
/home/hugo/.dotnet/dotnet new web -n LocationVoitures.Gateway -f net10.0
/home/hugo/.dotnet/dotnet sln LocationVoitures.sln add LocationVoitures.Gateway/LocationVoitures.Gateway.csproj
```

### 16.3. Packages et references ajoutes

Packages YARP :

```bash
/home/hugo/.dotnet/dotnet add LocationVoitures.Gateway/LocationVoitures.Gateway.csproj package Yarp.ReverseProxy
/home/hugo/.dotnet/dotnet add LocationVoitures.Gateway/LocationVoitures.Gateway.csproj package Microsoft.Extensions.ServiceDiscovery.Yarp
```

Reference au projet commun Aspire :

```bash
/home/hugo/.dotnet/dotnet add LocationVoitures.Gateway/LocationVoitures.Gateway.csproj reference LocationVoitures.ServiceDefaults/LocationVoitures.ServiceDefaults.csproj
```

Cette reference est importante, car elle permet a la gateway de beneficier de :

- la telemetrie OpenTelemetry
- les health checks
- la decouverte de services Aspire

### 16.4. Configuration de la gateway

Le fichier `LocationVoitures.Gateway/Program.cs` a ete modifie pour :

- activer `AddServiceDefaults()`
- charger la configuration YARP depuis `appsettings.json`
- activer le resolver de service discovery Aspire
- exposer un endpoint `/` informatif
- mapper le reverse proxy YARP
- exposer les endpoints de health check

### 16.5. Routes YARP configurees

Dans `LocationVoitures.Gateway/appsettings.json`, trois prefixes sont routes vers `apiservice` :

- `/voitures/{**catch-all}`
- `/locations/{**catch-all}`
- `/loueurs/{**catch-all}`

Le cluster cible utilise :

```json
"Address": "https+http://apiservice"
```

Cela permet a YARP de s'appuyer sur la decouverte de services Aspire au lieu d'une URL fixe.

### 16.6. Integration dans AppHost

Le projet `LocationVoitures.Gateway` a ete reference dans `LocationVoitures.AppHost`.

Dans `AppHost.cs`, la ressource Aspire suivante a ete ajoutee :

```csharp
var gateway = builder.AddProject<Projects.LocationVoitures_Gateway>("gateway")
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");
```

Cette configuration signifie que :

- la gateway connait `apiservice`
- elle attend que l'API soit disponible
- elle est exposee publiquement
- elle apparait proprement dans Aspire avec son endpoint de sante

### 16.7. Fichiers ajoutes ou modifies

- `LocationVoitures.Gateway/Program.cs`
- `LocationVoitures.Gateway/appsettings.json`
- `LocationVoitures.Gateway/LocationVoitures.Gateway.csproj`
- `LocationVoitures.Gateway/LocationVoitures.Gateway.http`
- `LocationVoitures.AppHost/AppHost.cs`
- `LocationVoitures.AppHost/LocationVoitures.AppHost.csproj`
- `LocationVoitures.sln`

### 16.8. Fichier HTTP de test

Un fichier `LocationVoitures.Gateway/LocationVoitures.Gateway.http` a ete ajoute pour tester rapidement :

- `/`
- `/voitures`
- `/locations`
- `/loueurs`

### 16.9. Validation de compilation

Commandes de verification :

```bash
/home/hugo/.dotnet/dotnet build LocationVoitures.Gateway/LocationVoitures.Gateway.csproj
/home/hugo/.dotnet/dotnet build LocationVoitures.AppHost/LocationVoitures.AppHost.csproj
```

### 16.10. Prochaine verification a faire

Apres lancement de l'AppHost :

```bash
/home/hugo/.dotnet/dotnet run --project LocationVoitures.AppHost
```

il faudra verifier :

- que la ressource `gateway` apparait dans Aspire
- que l'URL de la gateway repond bien
- que `GET /voitures`, `GET /locations` et `GET /loueurs` fonctionnent via la gateway
- que les traces Aspire montrent bien le passage `gateway -> apiservice`

---

## 17. TP1 Partie 2 - Etape 2 : premier frontend Blazor branche sur la gateway

Le sujet demande ensuite de coder une application Blazor capable, au minimum, d'afficher le catalogue des voitures.
Nous avons commence par la plus petite brique utile : faire parler le projet `LocationVoitures.Web` a la gateway, puis afficher une premiere page `Catalogue`.

### 17.1. Changement d'architecture

Avant cette etape, le projet Web parlait directement au service `apiservice`.
Desormais, il depend de la gateway.

Dans `LocationVoitures.AppHost/AppHost.cs`, `webfrontend` reference maintenant `gateway` :

```csharp
builder.AddProject<Projects.LocationVoitures_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(gateway)
    .WaitFor(gateway);
```

Cela correspond mieux a l'architecture du TP2 :

- client Blazor
- gateway YARP
- API metier

### 17.2. HttpClient du frontend

Dans `LocationVoitures.Web/Program.cs`, le `HttpClient` n'utilise plus `apiservice` mais `gateway` :

```csharp
builder.Services.AddHttpClient<VoituresApiClient>(client =>
{
    client.BaseAddress = new("https+http://gateway");
});
```

### 17.3. Premiere Vertical Slice frontend

Pour rester coherent avec la logique du sujet, une premiere structure verticale a ete ajoutee :

- `LocationVoitures.Web/Features/Voitures/Models/VoitureDto.cs`
- `LocationVoitures.Web/Features/Voitures/Services/VoituresApiClient.cs`
- `LocationVoitures.Web/Features/Voitures/Components/VoitureCard.razor`
- `LocationVoitures.Web/Features/Voitures/Pages/Catalogue.razor`

Roles :

- `Models`
  - contient le DTO utilise par le frontend
- `Services`
  - contient le client HTTP typé
- `Components`
  - contient un composant presentational pour afficher une voiture
- `Pages`
  - contient la page Razor du catalogue

### 17.4. Premiere fonctionnalite implementee

La page `Catalogue.razor` :

- est accessible via la navigation du projet Web
- appelle `GET /voitures` via la gateway
- affiche les voitures dans une grille Bootstrap
- montre la categorie, l'immatriculation, le prix journalier et la disponibilite

### 17.5. Fichiers modifies ou remplaces

- `LocationVoitures.Web/Program.cs`
- `LocationVoitures.Web/Components/_Imports.razor`
- `LocationVoitures.Web/Components/Layout/NavMenu.razor`
- `LocationVoitures.Web/Components/Pages/Home.razor`
- suppression de `LocationVoitures.Web/WeatherApiClient.cs`
- suppression de `LocationVoitures.Web/Components/Pages/Weather.razor`

Remplacements et ajouts :

- `LocationVoitures.Web/Features/Voitures/Models/VoitureDto.cs`
- `LocationVoitures.Web/Features/Voitures/Services/VoituresApiClient.cs`
- `LocationVoitures.Web/Features/Voitures/Components/VoitureCard.razor`
- `LocationVoitures.Web/Features/Voitures/Pages/Catalogue.razor`

### 17.6. Verification

Commandes de compilation :

```bash
/home/hugo/.dotnet/dotnet build LocationVoitures.Web/LocationVoitures.Web.csproj
/home/hugo/.dotnet/dotnet build LocationVoitures.AppHost/LocationVoitures.AppHost.csproj
```

### 17.7. Validation attendue

Apres lancement de l'orchestrateur :

```bash
/home/hugo/.dotnet/dotnet run --project LocationVoitures.AppHost
```

il faut verifier :

- que `webfrontend` demarre correctement
- que la page d'accueil s'ouvre
- que le menu contient bien `Catalogue`
- que la page `Catalogue` affiche les voitures
- que les donnees proviennent bien de la gateway

---

## 18. TP1 Partie 2 - Etape 3 : frontend Blazor complet avant le paiement

Le sujet demande ensuite d'aller plus loin que le simple catalogue :

- rechercher une voiture par immatriculation
- visualiser ses locations passees, en cours ou futures
- creer une reservation
- afficher les erreurs et validations de facon lisible

Cette etape finalise ce parcours utilisateur, sans encore ajouter la partie paiement.

### 18.1. Objectif fonctionnel

Avant de coder le paiement, l'application Web doit deja permettre :

1. de consulter le catalogue
2. de filtrer les voitures par categorie
3. de rechercher une voiture precise par immatriculation
4. d'ouvrir une fiche detaillee sur cette voiture
5. d'afficher l'historique de ses locations
6. de reserver cette voiture en selectionnant un loueur et des dates

### 18.2. Organisation frontend retenue

Le projet `LocationVoitures.Web` reste structure par fonctionnalite :

- `Features/Voitures`
  - `Models`
  - `Services`
  - `Components`
  - `Pages`
- `Features/Locations`
  - `Models`
  - `Services`
  - `Components`
- `Features/Loueurs`
  - `Models`
  - `Services`

Un `_Imports.razor` a egalement ete ajoute a la racine du projet Web pour que les composants situes hors du dossier `Components` beneficiient eux aussi des bons `@using`.

### 18.3. Catalogue ameliore

La page `LocationVoitures.Web/Features/Voitures/Pages/Catalogue.razor` ne se contente plus d'afficher une liste brute.

Elle permet maintenant :

- de filtrer le catalogue par categorie avec `GET /voitures?categorie=...`
- de rechercher une voiture par immatriculation avec `GET /voitures/immatriculation/{immatriculation}`
- d'afficher quelques indicateurs simples :
  - nombre de voitures chargees
  - nombre de voitures disponibles aujourd'hui
  - filtre actif

Chaque carte voiture contient aussi un bouton qui ouvre la fiche detaillee de la voiture.

### 18.4. Fiche detaillee d'une voiture

Une nouvelle page a ete ajoutee :

- `LocationVoitures.Web/Features/Voitures/Pages/VoitureDetails.razor`

Route :

- `/catalogue/{immatriculation}`

Cette page charge :

- la voiture choisie via `VoituresApiClient`
- les locations associees via `LocationsApiClient`
- les loueurs via `LoueursApiClient`

Elle sert de point d'entree unique pour :

- lire les informations de la voiture
- visualiser l'historique de location
- reserver la voiture

### 18.5. Composants metier ajoutes

Pour garder un front lisible et maintenable, les morceaux de l'interface ont ete separes :

- `LocationVoitures.Web/Features/Locations/Components/LocationsHistory.razor`
  - affiche les locations d'une voiture
  - calcule un statut utilisateur :
    - `A venir`
    - `En cours`
    - `Terminee`
    - `Annulee`

- `LocationVoitures.Web/Features/Locations/Components/ReservationForm.razor`
  - encapsule le formulaire de reservation
  - appelle `POST /locations`
  - remonte les erreurs renvoyees par l'API
  - affiche le message de succes et le prix total

### 18.6. Services frontend ajoutes ou etendus

Le frontend utilise maintenant plusieurs clients HTTP typés :

- `VoituresApiClient`
  - `GetCatalogueAsync`
  - `GetByImmatriculationAsync`

- `LocationsApiClient`
  - `GetByVoitureAsync`
  - `CreateAsync`

- `LoueursApiClient`
  - `GetAllAsync`

Une aide commune a ete ajoutee dans :

- `LocationVoitures.Web/Services/ApiResponseHelper.cs`

Son role est de :

- lire les `ProblemDetails`
- lire les `ValidationProblemDetails`
- convertir une erreur HTTP en message lisible dans l'interface

Cela evite de dupliquer la gestion d'erreur dans chaque composant Razor.

### 18.7. Navigation et ergonomie

La navigation et les styles ont ete ajustes pour rendre l'application plus claire :

- `Components/Pages/Home.razor`
  - devient une vraie page d'accueil du parcours utilisateur
- `Components/Layout/MainLayout.razor`
  - affiche une entete plus explicite
- `Components/Layout/NavMenu.razor`
  - ne garde que les entrees utiles
- `wwwroot/app.css`
  - ajoute des styles pour les panneaux, indicateurs, zones vides et formulaires

Le but est que quelqu'un qui decouvre le projet comprenne rapidement :

- ou commencer
- ou chercher une voiture
- ou reserver

### 18.8. Fichiers principaux ajoutes ou modifies

Ajouts :

- `LocationVoitures.Web/_Imports.razor`
- `LocationVoitures.Web/Features/Locations/Components/LocationsHistory.razor`
- `LocationVoitures.Web/Features/Locations/Components/ReservationForm.razor`
- `LocationVoitures.Web/Features/Voitures/Pages/VoitureDetails.razor`

Modifications importantes :

- `LocationVoitures.Web/Components/Layout/MainLayout.razor`
- `LocationVoitures.Web/Components/Layout/MainLayout.razor.css`
- `LocationVoitures.Web/Components/Layout/NavMenu.razor`
- `LocationVoitures.Web/Components/Pages/Home.razor`
- `LocationVoitures.Web/Features/Voitures/Components/VoitureCard.razor`
- `LocationVoitures.Web/Features/Voitures/Pages/Catalogue.razor`
- `LocationVoitures.Web/Features/Voitures/Services/VoituresApiClient.cs`
- `LocationVoitures.Web/Features/Locations/Services/LocationsApiClient.cs`
- `LocationVoitures.Web/Features/Loueurs/Services/LoueursApiClient.cs`
- `LocationVoitures.Web/wwwroot/app.css`

### 18.9. Verification

Commande de verification la plus fiable :

```bash
/home/hugo/.dotnet/dotnet build LocationVoitures.AppHost/LocationVoitures.AppHost.csproj
```

Cette compilation reconstruit :

- la gateway
- l'API
- le frontend Web
- l'orchestrateur Aspire

### 18.10. Validation attendue

Apres lancement :

```bash
/home/hugo/.dotnet/dotnet run --project LocationVoitures.AppHost
```

il faut verifier :

- que la page d'accueil Web s'ouvre correctement
- que le menu contient bien `Accueil` et `Catalogue`
- que la page `Catalogue` affiche les voitures via la gateway
- que le filtre par categorie fonctionne
- qu'une recherche par immatriculation ouvre la fiche de la voiture
- que la fiche affiche ses locations
- que le formulaire de reservation fonctionne
- qu'une erreur metier de l'API remonte proprement dans l'interface

### 18.11. Etat a ce stade

Avant le paiement, le frontend couvre maintenant les besoins principaux du sujet :

- catalogue
- recherche
- detail d'une voiture
- historique des locations
- reservation

---

## 19. TP1 Partie 2 - Etape 4 : service de paiement fake et reservation payee

Le sujet propose ensuite d'ajouter un service de paiement fictif.  
Cette partie a ete implementée comme un vrai service separe, afin de rester coherente avec l'architecture Aspire mise en place avec la gateway.

### 19.1. Objectif

Lorsqu'un utilisateur cree une reservation :

1. l'API valide les regles metier habituelles
2. l'API calcule le prix total
3. l'API appelle un service de paiement fake
4. si le paiement est autorise, la location est creee avec `Paye = true`
5. si le paiement est refuse, la reservation n'est pas creee

### 19.2. Nouveau projet ajoute

Un nouveau projet a ete cree :

- `LocationVoitures.PaymentService`

Commande utilisee :

```bash
/home/hugo/.dotnet/dotnet new web -n LocationVoitures.PaymentService -f net10.0
/home/hugo/.dotnet/dotnet sln LocationVoitures.sln add LocationVoitures.PaymentService/LocationVoitures.PaymentService.csproj
```

Ce service reference `LocationVoitures.ServiceDefaults` pour profiter de :

- la decouverte de services Aspire
- la telemetrie
- les health checks

### 19.3. Structure du service de paiement

Le projet de paiement suit la meme logique de structure que les autres services :

- `Features/Payments/Endpoints`
- `Features/Payments/Requests`
- `Features/Payments/Responses`
- `Features/Payments/Validators`
- `Features/Payments/Services`

Fichiers principaux :

- `LocationVoitures.PaymentService/Program.cs`
- `LocationVoitures.PaymentService/Features/Payments/Endpoints/AuthorizePaymentEndpoint.cs`
- `LocationVoitures.PaymentService/Features/Payments/Requests/PaymentAuthorizationRequest.cs`
- `LocationVoitures.PaymentService/Features/Payments/Responses/PaymentAuthorizationResponse.cs`
- `LocationVoitures.PaymentService/Features/Payments/Validators/PaymentAuthorizationRequestValidator.cs`
- `LocationVoitures.PaymentService/Features/Payments/Services/CardValidationService.cs`

### 19.4. Endpoint du service de paiement

Le service expose :

- `POST /payments/authorize`

Il verifie :

- la presence des donnees de paiement
- le numero de carte via l'algorithme de Luhn
- la date d'expiration

Le retour reste volontairement simple :

- `IsAuthorized`
- `Code`
- `Message`

### 19.5. Integration dans AppHost

Le service de paiement a ete ajoute a l'orchestrateur dans `LocationVoitures.AppHost/AppHost.cs` :

- la ressource `paymentservice` est declaree
- `apiservice` depend maintenant de `paymentservice`

Cela permet a l'API metier d'attendre le service de paiement avant de demarrer completement.

### 19.6. Integration dans l'API metier

Dans `LocationVoitures.ApiService`, une integration HTTP a ete ajoutee :

- `Features/Paiements/Requests/AutoriserPaiementRequest.cs`
- `Features/Paiements/Responses/AutoriserPaiementResponse.cs`
- `Features/Paiements/Services/PaiementApiClient.cs`

Dans `Program.cs`, l'API enregistre maintenant :

```csharp
builder.Services.AddHttpClient<PaiementApiClient>(client =>
{
    client.BaseAddress = new("https+http://paymentservice");
});
```

Le endpoint `POST /locations` appelle donc desormais le service de paiement avant d'enregistrer la location.

### 19.7. Evolution du modele Location

Le modele `Location` a ete enrichi avec :

- `Paye`

Cette propriete est :

- ajoutee dans `Domain/Location.cs`
- configuree avec une valeur par defaut dans `RentalDbContext`
- alimentee dans le seed avec `true` pour les locations deja existantes

### 19.8. Evolution de la reservation

Le DTO de reservation contient maintenant :

- `NumeroCarte`
- `MoisExpiration`
- `AnneeExpiration`

Ces champs ont ete ajoutes :

- dans l'API
- dans le frontend Blazor
- dans les validations FluentValidation

En cas de paiement accepte :

- `ReserverResponse` indique que le paiement est confirme
- la location est enregistree avec `Paye = true`

En cas de paiement refuse :

- l'API renvoie un `402 Payment Required`
- le frontend affiche une notification claire

En cas d'indisponibilite du service :

- l'API renvoie un `503 Service Unavailable`
- le frontend affiche un message indiquant que le service de paiement est indisponible

### 19.9. Frontend et paiement

Le formulaire de reservation a ete complete dans :

- `LocationVoitures.Web/Features/Locations/Components/ReservationForm.razor`

Il contient maintenant :

- le numero de carte
- le mois d'expiration
- l'annee d'expiration

L'historique des locations affiche aussi desormais l'etat de paiement :

- `Paye`
- `En attente`

### 19.10. Gestion d'erreurs cote frontend

Le mecanisme de notification global mis en place precedemment a ete reutilise pour le paiement.

Cas geres :

- numero de carte invalide
- carte expiree
- paiement refuse
- service de paiement indisponible
- erreurs inattendues pendant le rafraichissement de la fiche voiture

### 19.11. Tests ajoutes ou etendus

Les tests ont ete etendus pour couvrir cette partie :

- `LocationVoitures.Tests/Features/Locations/ReserverRequestValidatorTests.cs`
  - couverture des champs de paiement
- `LocationVoitures.Tests/Services/CardValidationServiceTests.cs`
  - tests Luhn
  - tests d'expiration

### 19.12. Migration EF Core

Une migration a ete generee pour ajouter le champ `paye` :

```bash
/home/hugo/.dotnet/dotnet ef migrations add AddPaymentSupport --project LocationVoitures.ApiService --startup-project LocationVoitures.ApiService
```

Fichiers generes :

- `LocationVoitures.ApiService/Migrations/20260429113709_AddPaymentSupport.cs`
- `LocationVoitures.ApiService/Migrations/20260429113709_AddPaymentSupport.Designer.cs`
- mise a jour de `RentalDbContextModelSnapshot.cs`

### 19.13. Verification

Compilation :

```bash
/home/hugo/.dotnet/dotnet build LocationVoitures.AppHost/LocationVoitures.AppHost.csproj
```

Tests :

```bash
/home/hugo/.dotnet/dotnet test LocationVoitures.Tests/LocationVoitures.Tests.csproj
```

### 19.14. Validation attendue

Apres lancement :

```bash
/home/hugo/.dotnet/dotnet run --project LocationVoitures.AppHost
```

il faut verifier :

- que `paymentservice` apparait dans Aspire
- qu'une reservation avec une carte valide fonctionne
- qu'une reservation avec une carte invalide est refusee
- qu'une reservation avec une carte expiree est refusee
- que l'historique affiche l'etat `Paye`
