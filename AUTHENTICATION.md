# Authentification - Erasmove

## Architecture

```
DatabaseService (connexion SQL Server, initialisation BD/tables)
    └── UserService (inscription, connexion, déconnexion, session)
        ├── AuthenticationViewModel (formulaire login/inscription)
        └── AccountViewModel (affichage compte, déconnexion)
```

## Flux d'inscription

1. L'utilisateur remplit le formulaire (nom complet, nom d'utilisateur, mot de passe, confirmation)
2. **Validation côté client** :
   - Nom d'utilisateur : 3+ caractères, uniquement lettres, chiffres et `_`
   - Mot de passe : 8+ caractères, 1 majuscule, 1 minuscule, 1 chiffre
   - Confirmation identique au mot de passe
3. **Vérification de l'unicité** du nom d'utilisateur en base de données
4. **Hachage du mot de passe** avec PBKDF2-SHA512 (100 000 itérations) + sel aléatoire de 32 octets
5. **Insertion en base** : `Id`, `Username`, `PasswordHash`, `PasswordSalt`, `FullName`, `CreatedAt`
6. **Sauvegarde de la session** locale via `Preferences` (stocke uniquement l'`Id`)
7. **Redirection** vers la page d'accueil

## Flux de connexion

1. L'utilisateur saisit son nom d'utilisateur et mot de passe
2. **Recherche en base** par username (requête paramétrée)
3. **Vérification du mot de passe** :
   - Récupère le sel et le hash stockés
   - Recalcule le hash avec le mot de passe saisi + sel stocké
   - Comparaison en temps constant (`CryptographicOperations.FixedTimeEquals`)
4. Si valide : session sauvegardée + redirection vers l'accueil
5. Si invalide : message générique "Nom d'utilisateur ou mot de passe incorrect"

## Flux de déconnexion

1. Suppression de l'`Id` utilisateur des `Preferences` locales
2. Redirection vers la page d'authentification

## Session et démarrage

Au lancement de l'application (`AppShell.OnNavigated`) :

1. `DatabaseService.InitializeAsync()` crée la base et les tables si nécessaire
2. `UserService.GetCurrentUserAsync()` vérifie si un utilisateur est connecté via `Preferences`
3. Si connecté → redirection vers l'accueil, sinon → page d'authentification

## Sécurité

| Mesure | Détail |
|---|---|
| Hachage | PBKDF2-SHA512, 100 000 itérations |
| Sel | 32 octets aléatoires par utilisateur (`RandomNumberGenerator`) |
| Taille du hash | 64 octets |
| Comparaison | Temps constant (`CryptographicOperations.FixedTimeEquals`) |
| Requêtes SQL | Paramétrées (protection injection SQL) |
| Messages d'erreur | Génériques (anti-énumération d'utilisateurs) |
| Session | Stocke uniquement l'`Id` localement, jamais le mot de passe |

## Table Users

```sql
CREATE TABLE Users (
    Id NVARCHAR(36) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(128) NOT NULL,
    PasswordSalt NVARCHAR(128) NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
```

## Infrastructure

- **SQL Server 2022** dans un conteneur Docker (`docker-compose.yml`)
- **Port** : 1433
- **Base de données** : `erasmove`
- **Volume persistant** : `sqlserver_data`

## Fichiers

| Fichier | Rôle |
|---|---|
| `Services/DatabaseService.cs` | Connexion SQL Server, initialisation BD et tables |
| `Services/UserService.cs` | Inscription, connexion, déconnexion, gestion session |
| `ViewModels/AuthenticationViewModel.cs` | Logique UI login/inscription avec validation |
| `ViewModels/AccountViewModel.cs` | Affichage infos utilisateur, déconnexion |
| `Views/AuthenticationPage.xaml` | Interface login/inscription |
| `Views/AccountPage.xaml` | Interface compte utilisateur |
| `Models/User.cs` | Modèle (Id, Username, FullName) |
