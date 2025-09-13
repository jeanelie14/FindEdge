# FindEdge Professional - Application de recherche de fichiers avancée

## 🎯 Description

FindEdge Professional est une application desktop moderne de recherche de fichiers inspirée d'Agent Ransack, mais avec des fonctionnalités avancées et une interface utilisateur professionnelle. L'interface a été complètement repensée pour rivaliser avec les meilleures applications de recherche de fichiers du marché.

## ✨ Fonctionnalités principales

### 🚀 Version actuelle - Interface Professionnelle
- ✅ **Interface Agent Ransack** : Design professionnel identique à Agent Ransack
- ✅ **Menu bar complet** : Fichier, Édition, Recherche, Affichage, Outils, Aide
- ✅ **Toolbar moderne** : Icônes et raccourcis clavier
- ✅ **Panneau de configuration** : Onglets Principal, Options, Dates
- ✅ **Recherche avancée** : Filtres par type, taille, date, expressions régulières
- ✅ **Zone de résultats** : GridView avec colonnes (Nom, Emplacement, Taille, etc.)
- ✅ **Panneau de statistiques** : Sommaire, Concordances, Rapports
- ✅ **Status bar** : Indicateurs de statut et clavier
- ✅ **Recherche hybride** : Recherche par nom de fichier et contenu
- ✅ **Export CSV** : Export des résultats de recherche
- ✅ **Recherche en temps réel** : Progression et annulation
- ✅ **Architecture MVVM** : Pattern Model-View-ViewModel avec injection de dépendances
- ✅ **Services modulaires** : Système de services extensible avec implémentations mock
- ✅ **Fenêtres spécialisées** : Accès aux fonctionnalités avancées via boutons dédiés
- ✅ **Gestion des doublons** : Interface dédiée pour la détection de fichiers dupliqués
- ✅ **Gestion des plugins** : Interface pour la gestion des extensions
- ✅ **Gestion de l'index** : Interface pour la configuration de l'indexation

### 🎯 Fonctionnalités avancées (Accessibles via bouton ⚙️)
- ✅ **Recherche sémantique** : Recherche intelligente basée sur le sens
- ✅ **Recherche en langage naturel** : Requêtes en français naturel
- ✅ **Génération de visualisations** : Graphiques et diagrammes des résultats
- ✅ **Analytics et rapports** : Statistiques détaillées et rapports
- ✅ **Partage et collaboration** : Partage de recherches et résultats
- ✅ **Aide et apprentissage** : Système d'aide intégré
- ✅ **Personnalisation** : Personnalisation de l'interface utilisateur

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

### Architecture technique
- **Pattern MVVM** : Séparation claire entre la logique métier et l'interface
- **Injection de dépendances** : Conteneur de services personnalisé
- **Services modulaires** : Architecture extensible avec interfaces
- **Implémentations mock** : Services de test et de développement
- **Fenêtres spécialisées** : Architecture modulaire pour les fonctionnalités avancées

## 🚀 Installation et exécution

### Prérequis
- .NET 8.0 SDK ou supérieur
- Windows 10/11 (pour la version WPF)
- Visual Studio 2022 ou VS Code (recommandé)

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

### Résolution des problèmes
```bash
# Nettoyer et reconstruire
dotnet clean
dotnet build

# Vérifier les erreurs de compilation
dotnet build --verbosity normal
```

## 🎨 Interface utilisateur

### Interface Professionnelle Style Agent Ransack
- **Design identique à Agent Ransack** : Interface professionnelle et familière
- **Layout en 3 colonnes** : Configuration, Résultats, Statistiques
- **Couleurs professionnelles** : Thème clair avec accents bleus
- **Typographie Segoe UI** : Police moderne et lisible
- **Fenêtre unique** : Interface optimisée avec une seule fenêtre principale

### Fonctionnalités UI Avancées
- **Menu bar complet** : Tous les menus standard (Fichier, Édition, Recherche, etc.)
- **Toolbar avec icônes** : Boutons d'action avec tooltips et raccourcis
- **Onglets de recherche** : Interface à onglets pour gérer plusieurs recherches
- **Panneau de configuration** : 
  - **Principal** : Nom fichier, contenu, répertoire, taille, dates
  - **Options** : Sensibilité casse, mots entiers, sous-dossiers, regex
  - **Dates** : Filtres par date de modification et création
- **Zone de résultats** : GridView avec colonnes (Nom, Emplacement, Taille, Concordances, Type, Modifié, Créé)
- **Panneau de statistiques** : 
  - **Sommaire** : Statistiques de recherche et actions rapides
  - **Concordances** : Aperçu des correspondances
  - **Rapports** : Génération de rapports
- **Status bar** : Message de statut et indicateurs clavier (CAP NUM SCR)
- **Bouton fonctionnalités avancées** : Accès aux fonctionnalités avancées via bouton ⚙️
- **Fenêtres spécialisées** : Accès aux outils spécialisés (doublons, plugins, index)

## 🔧 Configuration

### Options de recherche avancées
- **Nom de fichier** : Recherche par nom avec patterns
- **Contenu de fichier** : Recherche dans le texte des fichiers
- **Répertoire de recherche** : Sélection du dossier à examiner
- **Filtres de taille** : Min/Max en Ko avec opérateurs < et >
- **Filtres de date** : Modification et création (Avant/Aujourd'hui/Après)
- **Options avancées** : 
  - Sensibilité à la casse
  - Mots entiers seulement
  - Inclure les sous-dossiers
  - Recherche binaire
  - Expressions régulières
- **Modes de recherche** : Expert, Aa (casse), ? (aide)

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

### État actuel des tests
- ✅ **Tests de build** : Compilation réussie avec warnings mineurs
- ✅ **Tests de démarrage** : Application démarre sans erreurs
- ✅ **Tests d'interface** : Toutes les fenêtres s'ouvrent correctement
- 🔄 **Tests unitaires** : À implémenter pour les services
- 🔄 **Tests d'intégration** : À implémenter pour les fonctionnalités

## 🔮 Roadmap

### Phase 3 - Aperçu et Coloration (Q1 2024)
- [ ] **Aperçu de fichiers** : Visualisation du contenu avec coloration syntaxique
- [ ] **Coloration syntaxique** : Support pour C#, JavaScript, Python, etc.
- [ ] **Aperçu binaire** : Visualisation hexadécimale pour fichiers binaires
- [ ] **Navigation dans les résultats** : F3/Shift+F3 pour naviguer

### Phase 4 - Indexation (Q2 2024)
- [ ] **Moteur d'index Lucene.NET** : Indexation rapide et performante
- [ ] **Mode hybride** : Live scan + indexation optionnelle
- [ ] **Configuration utilisateur avancée** : Paramètres d'indexation
- [ ] **Recherche instantanée** : Résultats en temps réel

### Phase 5 - Parsers (Q3 2024)
- [ ] **Support PDF** : Recherche dans les documents PDF (PdfPig)
- [ ] **Support Office** : Word, Excel, PowerPoint (Open XML)
- [ ] **Support archives** : ZIP, RAR, 7-Zip (SharpCompress)
- [ ] **Système de plugins** : Extensibilité pour nouveaux formats

### Phase 6 - Features avancées (Q4 2024)
- [ ] **Détection de doublons intelligents** : Hash + perceptuel pour images
- [ ] **Exports enrichis** : JSON, PDF avec rapports détaillés
- [ ] **Interface en ruban** : Style Office 365 moderne
- [ ] **Thème sombre** : Mode sombre optionnel

### Phase 7 - Optimisation (Q1 2025)
- [ ] **Multithreading avancé** : Recherche parallèle optimisée
- [ ] **Portage .NET MAUI** : Version cross-platform
- [ ] **Tests de performance** : Benchmarks et optimisations

### Phase 8 - Distribution (Q2 2025)
- [ ] **Installateur MSI** : Installation professionnelle
- [ ] **Signature numérique** : Sécurité et confiance
- [ ] **Documentation utilisateur** : Guide complet

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

## 🎯 Comparaison avec Agent Ransack

### ✅ Avantages de FindEdge Professional
- **Interface identique** : Même look & feel qu'Agent Ransack
- **Architecture moderne** : .NET 8, WPF, MVVM
- **Extensibilité** : Système de plugins intégré
- **Code source ouvert** : Transparence et contribution communautaire
- **Gratuit** : Aucun coût de licence
- **Évolutif** : Roadmap claire pour les futures fonctionnalités
- **Fenêtre unique** : Interface optimisée sans encombrement
- **Fonctionnalités avancées** : Accès facile aux outils spécialisés

### 🔄 Fonctionnalités similaires
- **Recherche hybride** : Nom de fichier + contenu
- **Filtres avancés** : Taille, date, type de fichier
- **Interface professionnelle** : Menu bar, toolbar, panneaux
- **Export des résultats** : CSV et autres formats
- **Recherche en temps réel** : Progression et annulation

### 🆕 Fonctionnalités supplémentaires
- **Fenêtres spécialisées** : Gestion des doublons, plugins, index
- **Fonctionnalités avancées** : Recherche sémantique, visualisations
- **Architecture modulaire** : Services extensibles et testables
- **Interface optimisée** : Une seule fenêtre principale avec accès aux outils

## 📋 État actuel du projet

### ✅ Corrections récentes (Décembre 2024)
- **Problème des fenêtres multiples** : Résolu - une seule fenêtre au démarrage
- **Services manquants** : Tous les services sont maintenant enregistrés
- **Architecture MVVM** : Pattern correctement implémenté
- **Injection de dépendances** : Conteneur de services fonctionnel
- **Fenêtres spécialisées** : Accès via boutons dédiés dans l'interface

### 🔧 Problèmes résolus
- **Erreur de compilation** : `MainWindow` constructor corrigé
- **Services non enregistrés** : `IIndexedSearchEngine`, `IAnalyticsService`, etc.
- **Fichiers dupliqués** : Suppression des fichiers `MainWindow.xaml.cs` en double
- **StartupUri** : Suppression de l'ouverture automatique de fenêtre
- **Architecture** : Séparation claire entre fenêtre principale et fonctionnalités avancées

### 🎯 Fonctionnalités opérationnelles
- ✅ **Interface principale** : Recherche de fichiers avec tous les filtres
- ✅ **Fonctionnalités avancées** : Accessibles via bouton ⚙️
- ✅ **Gestion des doublons** : Interface dédiée fonctionnelle
- ✅ **Gestion des plugins** : Interface de gestion des extensions
- ✅ **Gestion de l'index** : Interface de configuration d'indexation
- ✅ **Export CSV** : Export des résultats de recherche
- ✅ **Recherche en temps réel** : Progression et annulation

## 🙏 Remerciements

- **Agent Ransack** : Inspiration majeure pour l'interface de recherche
- **Everything** : Référence pour la performance d'indexation
- **.NET Community** : Bibliothèques et outils open source
- **Contributors** : Tous les contributeurs du projet

---

**FindEdge Professional** - Alternative moderne et open source à Agent Ransack 🚀
