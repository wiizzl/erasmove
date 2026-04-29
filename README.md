# Erasmove

Erasmove est une application multiplateforme (.NET MAUI) de gestion des mobilités internationales. Elle permet aux gestionnaires de suivre les voyages des étudiants, d'organiser les transports et de répertorier les lieux de destination.

## Environnement d'exécution

Le projet utilise une architecture hybride : l'application s'exécute nativement (Windows/macOS), tandis que les données sont conteneurisées via Docker.

## Installation

1. Lancer la base de données

```sh
docker compose up -d
```

2. Suite à venir...

## Se connecter

Après avoir exécuté les [scripts SQL](./sql/), vous pourrez utiliser ces deux comptes à destination du product owner :

| Identifiant  | Mot de passe | Rôle           |
| ------------ | ------------ | -------------- |
| bocba@cba.fr | mdp          | `Gestionnaire` |
| focba@cba.fr | mdp          | `Voyageur`     |
