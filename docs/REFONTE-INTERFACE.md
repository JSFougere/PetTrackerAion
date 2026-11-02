# Première intégration de l'interface PetTracker Aion

## Accès pendant le jeu

La petite fenêtre de suivi donne directement accès à Collection, Catalogue,
Réglages, Session et Comptes. Il n'est plus nécessaire de passer par l'icône
de la zone de notification. Les anciens accès restent disponibles.

## Réglages, taille et noms français/anglais

Le tracker est temporairement masqué quand sa fenêtre de réglages est visible.
Il redevient éligible à l'affichage à la fermeture ou à la réduction des réglages,
en respectant toujours le masquage manuel, le mode réservé au jeu et les règles
d'expiration. Cette suppression temporaire ne change pas le paramètre `Visible`.

Les huit bords et coins de la fenêtre permettent son redimensionnement en mode
interactif déverrouillé. La largeur, la hauteur et la position sont enregistrées
par les commandes existantes. Le contenu peut défiler si la hauteur choisie est
petite. Les actualisations du worker ne réinitialisent pas une manipulation en
cours. `Settings → Overlay → Reset tracker size` revient à la taille automatique.

`Settings → Tracking → Langue des noms / Name language` permet de choisir
English ou Français pour les noms de familiers et de monstres. Il ne s'agit pas
encore d'une traduction intégrale des textes de l'interface. La recherche de la
collection, du marchand et du catalogue reste bilingue quel que soit ce choix ;
elle ignore la casse et les accents, y compris les accents Unicode décomposés.
Par exemple, `tortue etoilee` et `Starturtle` trouvent le même familier, et
`soldat arme krall` permet une recherche par nom de monstre français.

Les tables embarquées couvrent 207 identifiants de familiers et 468 identifiants
de monstres connus du projet. Les noms manquants gardent leur libellé d'origine.
Les noms sont issus des tables publiques du client de SkeeveAN/Aion-DPS-Meter,
au commit `72d409a5eef653d0a9e40637bb3854e2ee2382c8`, avec contrôle de la clé
interne du familier. Les notices et la licence du projet source sont conservées.
`tools/Import-LocalizedNames.ps1` reproduit cet import explicite ; aucun
téléchargement de noms n'est effectué pendant l'utilisation du tracker.

Validation de cette évolution : compilation sans avertissement ni erreur,
881 contrôles moteur, 374 contrôles WPF et 61 contrôles de mise à jour réussis.
Les tests de l'overlay simulent les événements de glissement hors écran et
vérifient le verrouillage, les contraintes de taille, la sauvegarde, le masquage
temporaire et le sélecteur de langue. Les essais en jeu restent nécessaires.

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
