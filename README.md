# Erasmove

<img src="./Erasmove/Resources/AppIcon/appiconfg.svg" alt="Logo" width="90" />

Erasmove est un projet scolaire réalisé en équipe. Il s'agit d'une application de bureau de gestion de voyages permettant de consulter et administrer les lieux, les transports, les trajets, les voyages et les utilisateurs liés aux déplacements.

## Environnement d'exécution

Le projet fonctionne avec deux briques distinctes :

- L'application MAUI, qui s'exécute en local sur Windows ou macOS.
- La base de données SQL Server, lancée via Docker Compose en environnement de développement.

| Programme  | Version |
| ---------- | ------- |
| .NET       | 10      |
| SQL Server | 2025    |

## Installation

### 1. Installer le workload MAUI

```sh
dotnet workload install maui
```

### 2. Démarrer la base de données

```sh
docker compose up -d
```

### 3. Initialiser la base de données

Exécuter les scripts présents dans le dossier [sql](./sql/).

### 4. Compiler et lancer le projet

```sh
dotnet build

dotnet run --project Erasmove
```

## Se connecter

Après avoir importé les scripts SQL, vous pouvez utiliser les comptes de démonstration suivants :

| Identifiant  | Mot de passe | Rôle           |
| ------------ | ------------ | -------------- |
| bocba@cba.fr | mdp          | `Gestionnaire` |
| focba@cba.fr | mdp          | `Voyageur`     |
