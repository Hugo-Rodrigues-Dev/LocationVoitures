# TP1 - Suivi d'avancement

Ce fichier sert de journal d'avancement du TP1.
Il est mis a jour a chaque etape.

## Etat actuel

Ce qui est expose dans l'API pour l'instant :

- `GET /`
- `GET /voitures`
- `POST /locations`

## Avancement par partie

### 1. Creation de la solution

Statut : `Partiellement fait`

Fait :

- solution Aspire creee avec les projets `LocationVoitures.AppHost`, `LocationVoitures.ApiService`, `LocationVoitures.Web`, `LocationVoitures.ServiceDefaults`
- depot Git initialise
- application lancable avec Aspire

### 2. Creation des couches de l'API

Statut : `Fait`

Dossiers actuellement presents dans `LocationVoitures.ApiService` :

- `Data`
- `Domain`
- `Features`
- `Migrations`

### 3. Couche BD

Statut : `Fait`

Fait :

- packages EF Core / PostgreSQL installes
- ressource PostgreSQL declaree dans `AppHost`
- base logique `RentalDb` declaree
- `RentalDbContext` cree
- entites `Loueur`, `Voiture`, `Location` creees
- migration EF creee
- seed de donnees ajoute
- migration automatique au demarrage
- connexion `builder.AddNpgsqlDbContext<RentalDbContext>("RentalDb")` ajoutee dans l'API

### 4. Ajout d'un code de test

Statut : `Remplace par la suite propre`

### 5.1. Couche DTO et Service - Voitures

Statut : `Fait`

Fait :

- `VoitureDto` cree
- endpoint `GET /voitures` cree
- filtre optionnel `categorie` gere
- disponibilite calculee en fonction des locations en cours
- endpoint mappe dans `Program.cs`

### 5.2. Couche DTO et Service - Locations

Statut : `Fait`

Fait :

- `ReserverRequest` cree
- `ReserverResponse` cree
- `LocationService` cree
- endpoint `POST /locations` cree
- verification de la disponibilite de la voiture ajoutee
- calcul du prix total ajoute
- creation d'une location en base ajoutee
- endpoint mappe dans `Program.cs`

### Dashboard Aspire

Lancer :

```bash
dotnet run --project LocationVoitures.AppHost
```

URLs de l'AppHost :

- `https://localhost:17030`
- `http://localhost:15124`

Si le dashboard ne s'ouvre pas automatiquement, il faut ouvrir l'une de ces URLs manuellement.

### API

API :

- `https://localhost:7491/`
- `http://localhost:5346/`

Scalar :

- `https://localhost:7491/scalar/v1`

OpenAPI :

- `https://localhost:7491/openapi/v1.json`


Traces :
https://localhost:17030/traces

