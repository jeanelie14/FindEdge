# FindEdge - Application de recherche de fichiers avancée

## 🎯 Description

FindEdge est une application desktop moderne de recherche de fichiers inspirée d'Agent Ransack, mais avec des fonctionnalités avancées et une interface utilisateur moderne.

## ✨ Fonctionnalités principales

### Phase 2 - MVP (Version actuelle)
- ✅ **Recherche hybride** : Recherche par nom de fichier et contenu
- ✅ **Interface moderne** : UI WPF avec thème sombre/clair
- ✅ **Aperçu intégré** : Visualisation du contenu avec surlignage
- ✅ **Filtres avancés** : Par extension, taille, date de modification
- ✅ **Export CSV** : Export des résultats de recherche
- ✅ **Recherche en temps réel** : Progression et annulation

### Fonctionnalités prévues (Phases suivantes)
- 🔄 **Indexation hybride** : Mode live scan + indexation optionnelle
- 📄 **Parsers avancés** : PDF, Office, archives
- 🔍 **Détection de doublons** : Hash classique + perceptuel pour images
- 📊 **Exports enrichis** : JSON, PDF avec rapports
- 🔌 **Système de plugins** : Extensibilité pour nouveaux formats
- 🌐 **Cross-platform** : Portage vers .NET MAUI

## 🏗️ Architecture

```
FindEdge/
├── src/
│   ├── FindEdge.Core/           # Logique métier
│   │   ├── Models/              # Modèles de données
│   │   ├── Interfaces/          # Contrats d'interface
│   │   └── Services/            # Services métier
│   ├── FindEdge.Infrastructure/ # Couche d'infrastructure
│   │   ├── Services/            # Implémentations concrètes
│   │   └── Parsers/             # Parseurs de contenu
│   ├── FindEdge.Presentation/   # Interface utilisateur WPF
│   │   ├── ViewModels/          # ViewModels MVVM
│   │   ├── Themes/              # Thèmes UI
│   │   └── Views/               # Vues XAML
│   └── FindEdge.Plugins/        # Système de plugins
├── tests/                       # Tests unitaires
├── docs/                        # Documentation
└── tools/                       # Scripts de build
```

## 🚀 Installation et exécution

### Prérequis
- .NET 8.0 SDK ou supérieur
- Windows 10/11 (pour la version WPF)

### Compilation
```bash
# Cloner le projet
git clone <repository-url>
cd FindEdge

# Restaurer les packages
dotnet restore

# Compiler la solution
dotnet build

# Exécuter l'application
dotnet run --project src/FindEdge.Presentation
```

### Exécution directe
```bash
# Depuis le répertoire racine
dotnet run --project src/FindEdge.Presentation
```

## 🎨 Interface utilisateur

### Thème sombre par défaut
- Interface moderne avec couleurs sombres
- Accents bleus pour les éléments interactifs
- Typographie claire et lisible

### Fonctionnalités UI
- **Barre de recherche** : Champ de recherche principal avec boutons d'action
- **Liste des résultats** : Affichage des fichiers trouvés avec informations détaillées
- **Panneau d'aperçu** : Visualisation du contenu des fichiers
- **Barre de statut** : Progression de la recherche et statistiques

## 🔧 Configuration

### Options de recherche
- **Terme de recherche** : Texte à rechercher
- **Recherche dans** : Nom de fichier et/ou contenu
- **Options avancées** : Regex, sensibilité à la casse, mot entier
- **Filtres** : Extensions, taille, date de modification

### Répertoires de recherche
- Par défaut : Répertoire Documents de l'utilisateur
- Configuration : Ajout/suppression de répertoires
- Exclusions : Dossiers système et personnalisés

## 📊 Performance

### Optimisations actuelles
- **Recherche asynchrone** : Interface non bloquante
- **Annulation** : Possibilité d'arrêter la recherche
- **Limitation des résultats** : Évite la surcharge mémoire
- **Filtrage intelligent** : Exclusion des fichiers système

### Métriques
- **Fichiers traités** : Compteur en temps réel
- **Temps écoulé** : Durée de la recherche
- **Résultats trouvés** : Nombre de correspondances

## 🧪 Tests

### Tests unitaires
```bash
# Exécuter tous les tests
dotnet test

# Tests avec couverture
dotnet test --collect:"XPlat Code Coverage"
```

### Tests d'intégration
- Tests de recherche sur différents types de fichiers
- Tests de performance avec gros volumes
- Tests d'interface utilisateur

## 🔮 Roadmap

### Phase 3 - Indexation (Q1 2024)
- [ ] Moteur d'index Lucene.NET
- [ ] Mode hybride (live/indexé)
- [ ] Configuration utilisateur avancée

### Phase 4 - Parsers (Q2 2024)
- [ ] Support PDF (PdfPig)
- [ ] Support Office (Open XML)
- [ ] Support archives (SharpCompress)
- [ ] Système de plugins

### Phase 5 - Features avancées (Q3 2024)
- [ ] Détection de doublons intelligents
- [ ] Exports JSON/PDF
- [ ] Visualisation interactive

### Phase 6 - Optimisation (Q4 2024)
- [ ] Multithreading avancé
- [ ] Portage .NET MAUI
- [ ] Tests de performance

### Phase 7 - Distribution (Q1 2025)
- [ ] Installateur MSI
- [ ] Signature numérique
- [ ] Documentation utilisateur

## 🤝 Contribution

### Développement
1. Fork le projet
2. Créer une branche feature (`git checkout -b feature/nouvelle-fonctionnalite`)
3. Commit les changements (`git commit -am 'Ajout nouvelle fonctionnalité'`)
4. Push vers la branche (`git push origin feature/nouvelle-fonctionnalite`)
5. Créer une Pull Request

### Standards de code
- **C#** : Suivre les conventions Microsoft
- **XAML** : Indentation 2 espaces
- **Tests** : Couverture minimale 80%
- **Documentation** : XML comments pour les APIs publiques

## 📄 Licence

Ce projet est sous licence MIT. Voir le fichier `LICENSE` pour plus de détails.

## 📞 Support

- **Issues** : Utiliser GitHub Issues pour signaler des bugs
- **Discussions** : GitHub Discussions pour les questions
- **Email** : [votre-email@domain.com]

## 🙏 Remerciements

- **Agent Ransack** : Inspiration pour l'interface de recherche
- **Everything** : Référence pour la performance d'indexation
- **.NET Community** : Bibliothèques et outils open source
- **Contributors** : Tous les contributeurs du projet

---

**FindEdge** - Recherche de fichiers moderne et performante 🚀
