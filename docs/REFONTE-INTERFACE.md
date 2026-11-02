# Première intégration de l'interface PetTracker Aion

## Accès pendant le jeu

La petite fenêtre de suivi donne directement accès à Collection, Catalogue,
Réglages, Session et Comptes. Il n'est plus nécessaire de passer par l'icône
de la zone de notification. Les anciens accès restent disponibles.

Si le passage des clics au jeu est activé, l'overlay ne peut pas recevoir les
clics sur ces boutons : utiliser le raccourci ou la zone de notification pour
revenir au mode interactif, comme auparavant.

## Collection

La fenêtre existante conserve la recherche, les filtres, les favoris de filtres,
les familles, les détails, les épingles, les objectifs, les cartes et le bilan
de session. Elle reçoit une navigation latérale sur les grandes fenêtres et des
accès directs au catalogue et aux réglages. La navigation latérale se replie
sur les fenêtres de moins de 1050 unités WPF. Le thème conserve les paramètres
existants et utilise l'accent doré du projet.

## Catalogue dans les réglages

La section Catalogue réunit les 299 entrées nommées utilisables du catalogue
embarqué, y compris celles exclues des recommandations de farming. La recherche
porte sur les noms, familles, monstres et régions ; un filtre permet de choisir
une région, indépendamment du personnage et de sa faction.

Les colonnes indiquent les régions, les sources et les réserves sur leur
disponibilité Global. Faire défiler horizontalement pour les colonnes longues ;
les sources complètes sont également disponibles au survol des lignes. La
fiche de collection peut être ouverte pour les entrées dont les sources ou la
progression permettent déjà l'affichage dans la collection.

Les informations proviennent des données embarquées existantes. Cette étape
n'importe pas un nouveau wiki et ne certifie pas les régions non vérifiées.
« À compléter » ne signifie pas qu'un familier est exclusif au marchand ou aux
coffres. Les familiers payants consultables dans ce catalogue ne sont pas
réintroduits dans les recommandations ni les totaux de farming.

## Vérifications

- Compilation Release : aucun avertissement ni erreur.
- 358 vérifications WPF, dont recherche du catalogue, régions des deux factions,
  absence de résultat, intégration aux réglages et accès de l'overlay.
- 869 vérifications du moteur et 61 du système de mise à jour.
- Captures inspectées sous `test-results/refonte-ui-4/` :
  `collection-redesign.png`, `settings-catalogue.png`, `overlay-navigation.png`.
- Les assertions de prix du test WPF respectent maintenant le format numérique
  de la culture courante, plutôt que d'imposer une virgule anglaise.

Les tests utilisent des données fictives isolées. La capture du jeu, le stockage
des utilisateurs, les formats de sauvegarde et la configuration des mises à jour
n'ont pas été modifiés. Cette intégration ne constitue pas encore une validation
en jeu ni un paquet de distribution indépendant de l'installation d'origine.
