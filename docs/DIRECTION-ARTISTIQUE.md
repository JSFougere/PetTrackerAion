# PetTracker Aion — proposition de direction artistique

Proposition du 2 novembre 2026, à discuter avant la réalisation des nouveaux
écrans. Elle accompagne [l'analyse technique](ANALYSE-REFONTE.md) et ne modifie
pas le thème actuel.

## Intention

Un compagnon de jeu sobre : fond ardoise, surfaces légèrement différenciées,
typographie claire et accent doré discret. La progression des familiers doit
dominer ; les éléments décoratifs restent secondaires. Cette proposition
prolonge les couleurs déjà présentes dans PetTracker et peut être réalisée
avec des contrôles WPF standards.

Deux usages guident la composition : consulter et organiser sa collection dans
une fenêtre de gestion ; suivre quelques informations pendant le jeu dans un
overlay compact. Ils partagent leurs composants et leur vocabulaire visuel.

## Écrans proposés

### Fenêtre de gestion

Dimensions de travail proposées : environ 1120 × 760 unités WPF à 100 %, avec
une disposition adaptable. À faible largeur, replier les filtres et présenter
les détails dans un panneau ouvrable. Éviter de fixer une taille minimale qui
dépasserait la zone utile d'un écran avec mise à l'échelle élevée.

- Navigation latérale : Collection, Session, Paramètres. Les cartes et le
  marchand restent accessibles depuis la collection ; les comptes depuis le
  sélecteur de personnage.
- Bandeau supérieur : personnage et serveur, état de synchronisation et accès
  à l'overlay. Afficher « Collection enregistrée » si les données ne sont pas
  encore confirmées en direct.
- Zone Collection : recherche, filtres famille/rang/région/source, filtres
  enregistrés et résumé de progression.
- Liste principale : portrait, nom, famille, rang, compteur d'âmes, progression
  et épingle. Proposer les mêmes tris que le moteur actuel.
- Panneau de détails : objectif, âmes restantes, sources connues et onglets
  Détails/Carte. Conserver une sélection commune à la liste et à la carte.

Schéma de composition indicatif :

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ PetTracker Aion     Personnage · Serveur       Synchronisé   [Overlay]   │
├──────────────┬──────────────────────────────────┬────────────────────────┤
│ Collection   │ Rechercher un familier…         │ Familier sélectionné   │
│ Session      │ Famille · Rang · Région · Source│ [Détails] [Carte]      │
│ Paramètres   │ Résumé de la collection         │                        │
│              ├──────────────────────────────────┤ Rang et âmes           │
│              │ Portrait  Nom       Rang  ★     │ Objectif de rang       │
│              │           42 / 75  ━━━━━───      │ Sources et lieux       │
│              │ …                                │ [Épingler]             │
└──────────────┴──────────────────────────────────┴────────────────────────┘
```

Le compteur de ce schéma est fictif. Les valeurs réelles viennent exclusivement
de Core et du worker.

### Overlay en jeu

Partir des largeurs appliquées actuellement : 340 unités WPF en mode normal,
300 en mode compact, avant mise à l'échelle, avec une hauteur ajustée au contenu.
Préserver la mise à l'échelle existante, le nombre maximal de lignes,
les modes compact et normal et la limite d'épingles configurée.

- En-tête court : état de connexion, verrouillage, paramètres et réduction.
- Deux modes explicites : « Récent » et « À proximité ».
- Ligne de familier : portrait facultatif, nom, rang, âmes et barre de progression.
- Épingles identifiables sans ajouter de ligne à chaque actualisation.
- Bloc marchand visible selon les observations disponibles, avec accès aux
  détails ; éviter de transformer l'overlay en grand tableau de gestion.
- Déplacement par l'en-tête, position fixe et passage des clics au jeu conservés.
  Le raccourci et la zone de notification doivent permettre de récupérer les
  commandes lorsque l'overlay ne reçoit plus les clics.

### Paramètres et aide

Conserver les groupes Suivi, Overlay, Compte, Mises à jour et Aide. Présenter
les options d'apparence avec une prévisualisation utilisant des données
fictives. Rendre l'état d'enregistrement visible, y compris une erreur et la
possibilité de réessayer ou d'abandonner les changements en attente.

L'aide doit distinguer Npcap absent, jeu non détecté, attente de synchronisation
et interruption de connexion. Elle réutilise les diagnostics existants ; le
nouveau texte ne doit pas masquer leurs différences.

## Palette et composants

| Usage | Valeur proposée |
| --- | --- |
| Fond principal | `#0B1017` |
| Surface de panneau | `#141D29` |
| Surface interactive | `#202A39` |
| Bordure | `#354254` |
| Texte principal | `#DFE6EF` |
| Texte secondaire | `#A7B3C4` |
| Accent doré | `#D9B24C` |
| Focus clavier | `#80D7EA` |
| Réussite | `#67D4B0` |
| Avertissement | `#F0C46A` |
| Erreur | `#F18B98` |

Les accents bleu et turquoise existants restent des variantes possibles. Garder
les valeurs de paramètres `Gold`, `Blue` et `Teal` pour la compatibilité des
sauvegardes, même si leurs libellés affichés sont traduits. Le choix de langue
et la traduction intégrale des textes du client restent à définir ; ces
libellés français illustrent la proposition.

Utiliser Segoe UI pour éviter une dépendance de police. Proposition de tailles :
13–14 pour le texte de gestion, 20–24 pour les titres, 12–13 pour l'overlay selon
son échelle. Prévoir un rythme d'espacement de 4, 8, 12, 16 et 24 unités et des
angles de 6 à 8 unités. Les valeurs devront être vérifiées dans les vrais
contrôles, en particulier en mode compact et avec des noms longs.

Centraliser les couleurs et dimensions dans les ressources WPF. Remplacer
progressivement les valeurs codées dans les constructeurs. Un composant de
ligne doit gérer les variantes compacte, normale, sélectionnée et épinglée
sans dupliquer les règles de progression.

## États et interactions à montrer dans la maquette

| État | Présentation attendue |
| --- | --- |
| En attente de connexion | Statut explicite et accès à l'aide |
| Collection sauvegardée, pas encore synchronisée | Mention « Collection enregistrée » et prudence sur les compteurs |
| Rang ou âmes inconnus | Tiret ou « Inconnu », jamais un zéro fabriqué |
| Aucun résultat | Résumé des filtres et action pour les effacer |
| Observation de proximité ancienne | Âge de l'observation, sans rayon ni distance annoncée |
| Rang maximal | Libellé « MAX » et épingle selon les règles existantes |
| Objectif atteint | Retour visuel discret ; son et notification selon les paramètres |
| Sauvegarde refusée | Erreur visible, valeur non présentée comme enregistrée |
| Carte indisponible | Sources externes existantes accessibles |

Les régions avec cartes embarquées restent Verteron et Altgard. Ne pas ajouter
de point de position du joueur, de flèche de marche ou de distance : le moteur
actuel ne fournit pas ces fonctions utilisables. Le marchand indique des offres
observées ; une progression d'âmes ne constitue pas une confirmation d'achat.

Les contrôles doivent avoir un focus visible, des noms accessibles et des
libellés ou infobulles pour les icônes. Préserver Ctrl+F et Échap dans la
recherche. La couleur ne doit pas être l'unique indication d'état. Vérifier
les contrastes sur les surfaces finales et sur l'overlay transparent. Respecter
le réglage de réduction des animations et éviter les animations permanentes.

## Comparaison des deux approches

| Approche | Gain | Limite |
| --- | --- | --- |
| A : apparence seule | Changement plus contenu, écrans familiers | Navigation et couplages actuels conservés |
| B : nouvelle expérience sur moteur conservé | Organisation cohérente, composants réutilisables, évolution facilitée | Migration des vues et vérifications plus importantes |

L'option B est la recommandation de ce dossier. Une première maquette de
l'overlay et de la collection permettra de juger la direction concrètement,
avant d'étendre les choix aux paramètres et aux comptes. Les illustrations
décoratives, nouveaux logos et ressources de jeu supplémentaires ne sont pas
nécessaires pour cette première version.
