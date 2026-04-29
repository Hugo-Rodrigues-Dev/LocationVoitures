# LocationVoitures - Acces rapide

Ce fichier sert uniquement a retrouver rapidement :

- les commandes de lancement
- les URLs utiles
- les identifiants de demo
- ce que l'on peut faire dans chaque interface

## Lancement

Depuis la racine du projet :

```bash
dotnet run --project LocationVoitures.AppHost
```

C'est la commande principale a utiliser.

Elle demarre :

- Le dashboard Aspire
- PostgreSQL
- PgAdmin
- L'API
- La gateway
- Le front
- Le service de paiement fictif

## URLs principales

### Dashboard Aspire

- `https://localhost:17030`
- `http://localhost:15124`

Permet de :

- Voir les ressources lancees
- Verifier l'etat des services
- Ouvrir les URLs exposees
- Consulter les traces

### API

- `https://localhost:7491`
- `http://localhost:5346`

Permet de :

- Tester les endpoints directement
- Acceder a Scalar
- Acceder au document OpenAPI

### Scalar

- `https://localhost:7491/scalar/v1`

Permet de :

- Lire la documentation de l'API
- Tester les routes `voitures`
- Tester les routes `locations`
- Tester les routes `loueurs`
- Verifier les schemas de requetes et reponses

### OpenAPI JSON

- `https://localhost:7491/openapi/v1.json`

Permet de :

- Recuperer la specification OpenAPI brute

### Front Blazor

- `https://localhost:7130`
- `http://localhost:5209`

Permet de :

- Consulter la page d'accueil
- Parcourir le catalogue
- Consulter la fiche d'une voiture
- Gerer les reservations
- Annuler une reservation
- Gerer les utilisateurs
- Consulter la vue tableau des vehicules

### Gateway YARP

- `https://localhost:7134`
- `http://localhost:5195`

Permet de :

- Verifier le routage vers l'API
- Tester les routes proxifiees :
  - `/voitures`
  - `/locations`
  - `/loueurs`

### Service de paiement fictif

Il est consommé automatiquement lors d'une reservation.

Fichier de test manuel :

- `LocationVoitures.PaymentService/LocationVoitures.PaymentService.http`

## PgAdmin

L'URL de `pgAdmin` est exposee depuis le dashboard Aspire.

Pour y acceder :

1. Ouvrir `https://localhost:17030`
2. Aller dans `Resources`
3. Ouvrir la ligne `pgadmin`

Identifiants :

- email : `admin@locationvoitures.fr`
- mot de passe : `Admin123!`

Permet de :

- Explorer la base `RentalDb`
- Voir les tables :
  - `loueur`
  - `voiture`
  - `location`
- Verifier les donnees seedees
- Verifier les reservations, annulations et paiements

## Ce que l'on peut faire dans le front

### Accueil

Page de synthese avec acces vers :

- `Catalogue`
- `Vehicules`
- `Reservations`
- `Utilisateurs`

### Catalogue

Permet de :

- Filtrer les voitures par categorie
- Rechercher une voiture par immatriculation
- Ouvrir la fiche d'une voiture
- Aller vers la reservation

### Fiche voiture

Permet de :

- Consulter les informations d'un vehicule
- Voir son historique de reservations
- Aller vers la page dediee de gestion des reservations

### Reservations

Permet de :

- Choisir une voiture
- Creer une reservation
- Payer via le service de paiement fictif
- Consulter l'historique des reservations
- Annuler une reservation future ou en cours

Carte de test conseillee :

- Numero : `4242 4242 4242 4242`
- Mois : `12`
- Annee : `2030`

Exemples de cas d'erreur utiles :

- Carte invalide : `4242 4242 4242 4241`
- Carte expiree : un mois ou une annee deja passes

### Utilisateurs

Permet de :

- Lister les loueurs
- Rechercher un loueur
- Creer un nouveau loueur
- Blacklister un loueur
- Retirer un loueur de la blacklist

### Vehicules

Permet de :

- Voir le parc sous forme de tableau
- Filtrer par categorie
- Filtrer par disponibilite
- Rechercher par marque, modele ou immatriculation
- Ouvrir la fiche d'un vehicule
- Aller directement vers sa reservation
