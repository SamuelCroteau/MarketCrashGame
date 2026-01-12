# Market Crash: Simulateur de Sabotage Boursier 📉

> Un jeu satirique en vue première personne où le chaos est votre meilleur investissement

## 🎮 Description du Projet

**Market Crash** est un jeu de puzzle/action en vue première personne qui propose une critique satirique du monde de la finance. Incarnez un investisseur sans scrupules prêt à tout pour faire grimper ses actions, même si cela signifie semer le chaos dans un environnement de bureau rempli d'employés aux routines quotidiennes bien établies.

## 🎯 Concept de Gameplay

Le joueur doit manipuler le marché boursier en sabotant stratégiquement les entreprises concurrentes. Chaque action a des conséquences : casser des objets, perturber les employés, ou même des actions plus... définitives. Mais attention ! Les témoins peuvent appeler à l'aide et ruiner vos plans.

Le temps avance heure par heure, et chaque PNJ suit une routine réaliste (travail, pause café, toilettes, socialisation), créant des fenêtres d'opportunité pour vos manœuvres.

## ✨ Fonctionnalités Principales

### Système de Marché Boursier Dynamique
- Les prix fluctuent en temps réel selon vos actions
- Panel d'investissement intuitif avec capital de départ limité
- Système d'achat/vente stratégique

### Intelligence Artificielle des PNJ
- Comportements pilotés par machine d'états (State Machine)
- Routines quotidiennes réalistes et variées
- Chaque personnage a ses préférences d'actions et dialogues uniques
- Système de témoins et d'alertes

### Gameplay et Mécaniques
- Actions à la première personne (frapper, interagir, saboter)
- Système de temps progressif (heure par heure)
- 3 environnements de bureau distincts avec layouts stratégiques
- Mélange de furtivité, gestion du temps et humour noir

## 🛠️ Technologies Utilisées

| Composant | Technologie |
|-----------|-------------|
| Moteur de jeu | Unity (URP - Universal Render Pipeline) |
| Langage | C# |
| Système d'entrée | Unity New Input System |
| Navigation IA | Unity NavMesh |
| Patterns | State Machine, Object Pooling |
| Interface | TextMesh Pro |
| Audio | Système personnalisé avec audio spatial |

## 👥 Équipe de Développement

Projet réalisé en duo dans le cadre d'un projet collégial.

**Responsabilités partagées:**
- Programmation gameplay et systèmes
- Design et création des niveaux
- Implémentation de l'IA des personnages
- Optimisation et performance

**Assets:**
- 🎵 Musique et design sonore entièrement composés par un membre de l'équipe
- 🎨 Assets visuels: ressources gratuites de la communauté (Unity Asset Store, open-source)
- 🏢 Niveaux et environnements: design et agencement personnalisés

## 📂 Structure du Projet

```
MarketCrashGame/
├── Assets/
│   ├── Scripts/          # Scripts C# du jeu
│   ├── Scenes/           # 3 niveaux de bureau
│   ├── Audio/            # Musique et effets sonores
│   ├── Materials/        # Matériaux et shaders
│   └── Prefabs/          # PNJ, objets interactifs
├── Packages/             # Dépendances Unity
└── ProjectSettings/      # Configuration Unity
```

## 🚀 Installation et Lancement

1. Clonez le repository
```bash
git clone https://github.com/SamuelCroteau/MarketCrashGame.git
```

2. Ouvrez le projet avec Unity

3. Ouvrez la scène principale (Main) dans `Assets/Scenes/`

4. Appuyez sur Play pour lancer le jeu

## 🎮 Comment Jouer

- **WASD** : Déplacement
- **Souris** : Regarder autour
- **Clic gauche** : Interagir / Frapper
- **E** : Ouvrir le panel d'investissement
- **Tab** : Afficher l'état du marché

**Objectif:** Maximisez vos profits avant la fin de la journée en manipulant le marché à votre avantage !

## 🎓 Contexte Académique

Ce projet a été développé dans le cadre de notre formation collégiale en développement de jeux vidéo. Il représente l'aboutissement de nos apprentissages en:
- Programmation orientée objet en C#
- Design de systèmes de jeu complexes
- Intelligence artificielle pour jeux vidéo
- Optimisation et performance

## 📝 Notes

Ce jeu est une œuvre de satire et ne reflète en aucun cas nos opinions sur les pratiques financières réelles. Il s'agit d'un projet éducatif avec une approche humoristique.

---

*Développé avec passion (et beaucoup de café) ☕*
