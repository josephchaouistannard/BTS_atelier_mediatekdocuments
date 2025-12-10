# MediatekDocuments
Cette application permet de gérer les documents (livres, DVD, revues) d'une médiathèque. Elle a été codée en C# sous Visual Studio 2022. C'est une application de bureau, prévue d'être installée sur une poste accédant à une base de données distante.<br>
L'application exploite une API REST pour accéder à la BDD MySQL. Des explications sont données plus loin, ainsi que le lien de récupération.
## Présentation
Voici les fonctionnalités : 
- recherches et affichage d'informations sur les documents de la médiathèque (livres, DVD, revues)
- réception de nouveaux numéros de revues.
- gestion de commande de documents
- gestion d'état d'exemplaires
<br>

![cas utilisation c](https://github.com/user-attachments/assets/97f1fbbc-d502-463b-8c55-abc71cca53e7)

<br>L'application ne comporte qu'une seule fenêtre (en plus d'une fenêtre d’authentification) divisée en plusieurs onglets.

## Les différents onglets

### Onglet 1 : Livres

Cet onglet présente la liste des livres, triée par défaut sur le titre.<br>
La liste comporte les informations suivantes : titre, auteur, collection, genre, public, rayon.

![capture ecran](https://github.com/user-attachments/assets/76626ee2-ff1d-4083-83a9-e658c91b9edb)

#### Recherches

<strong>Par le titre :</strong> Il est possible de rechercher un ou plusieurs livres par le titre. La saisie dans la zone de recherche se fait en autocomplétions sans tenir compte de la casse. Seuls les livres concernés apparaissent dans la liste.<br>
<strong>Par le numéro :</strong> il est possible de saisir un numéro et, en cliquant sur "Rechercher", seul le livre concerné apparait dans la liste (ou un message d'erreur si le livre n'est pas trouvé, avec la liste remplie à nouveau).

#### Filtres

Il est possible d'appliquer un filtre (un seul à la fois) sur une de ces 3 catégories : genre, public, rayon.<br>
Un combo par catégorie permet de sélectionner un item. Seuls les livres correspondant à l'item sélectionné, apparaissent dans la liste (par exemple, en choisissant le genre "Policier", seuls les livres de genre "Policier" apparaissent).<br>
Le fait de sélectionner un autre filtre ou de faire une recherche, annule le filtre actuel.<br>
Il est possible aussi d'annuler le filtre en cliquant sur une des croix.

#### Tris

Le fait de cliquer sur le titre d'une des colonnes de la liste des livres, permet de trier la liste par rapport à la colonne choisie.

#### Affichage des informations détaillées

Si la liste des livres contient des éléments, par défaut il y en a toujours un de sélectionné. Il est aussi possible de sélectionner une ligne (donc un livre) en cliquant n'importe où sur la ligne.<br>
La partie basse de la fenêtre affiche les informations détaillées du livre sélectionné (numéro de document, code ISBN, titre, auteur(e), collection, genre, public, rayon, chemin de l'image) ainsi que l'image.

#### Gestion des livres

On peut ajouter, modifier ou supprimer des livres.


#### Gestion des exemplaires
On peut modifier l'état d'un exemplaire ou le supprimer.

### Onglet 2 : DVD

Cet onglet présente la liste des DVD, triée par titre.<br>
La liste comporte les informations suivantes : titre, durée, réalisateur, genre, public, rayon.<br>
Le fonctionnement est identique à l'onglet des livres.<br>
La seule différence réside dans certaines informations détaillées, spécifiques aux DVD : durée (à la place de ISBN), réalisateur (à la place de l'auteur), synopsis (à la place de collection).

### Onglet 3 : Revues

Cet onglet présente la liste des revues, triées par titre.<br>
La liste comporte les informations suivantes : titre, périodicité, délai mise à dispo, genre, public, rayon.<br>
Le fonctionnement est identique à l'onglet des livres, sauf il n'y a pas de zone pour gérer les exemplaires.<br>
La seule différence réside dans certaines informations détaillées, spécifiques aux revues : périodicité (à la place de l'auteur), délai mise à dispo (à la place de collection).

### Onglet 4 : Parutions des revues

Cet onglet permet d'enregistrer la réception de nouvelles parutions d'une revue.<br>
Il se décompose en 2 parties (groupbox).

#### Partie "Recherche revue"

Cette partie permet, à partir de la saisie d'un numéro de revue (puis en cliquant sur le bouton "Rechercher"), d'afficher toutes les informations de la revue (comme dans l'onglet précédent), ainsi que son image principale en petit, avec en plus la liste des parutions déjà reçues (numéro, date achat, chemin photo). Sur la sélection d'une ligne dans la liste des parutions, la photo de la parution correspondante s'affiche à droite.<br>
Dès qu'un numéro de revue est reconnu et ses informations affichées, la seconde partie ("Nouvelle parution réceptionnée pour cette revue") devient accessible.<br>
Si une modification est apportée au numéro de la revue, toutes les zones sont réinitialisées et la seconde partie est rendue inaccessible, tant que le bouton "Rechercher" n'est pas utilisé.

#### Partie "Nouvelle parution réceptionnée pour cette revue"

Cette partie n'est accessible que si une revue a bien été trouvée dans la première partie.<br>
Il est possible alors de réceptionner une nouvelle parution en saisissant son numéro, en sélectionnant une date (date du jour proposée par défaut) et en cherchant l'image correspondante (optionnel) qui doit alors s'afficher à droite.<br>
Le clic sur "Valider la réception" va permettre d'ajouter un tuple dans la table Exemplaire de la BDD. La parution correspondante apparaitra alors automatiquement dans la liste des parutions et les zones de la partie "Nouvelle parution réceptionnée pour cette revue" seront réinitialisées.<br>
Si le numéro de la parution existe déjà, il n’est pas ajouté et un message est affiché.

![capture ecran parution](https://github.com/user-attachments/assets/9aae5e29-9583-44b3-a27b-54e4cf932521)

### Onglet 5 : Commandes Livres

Cet onglet permet de consulter l'ensemble des commandes pour les livres, ou de rechercher que les commandes pour un livre spécifique.
Il est possible de créer une commande, de modifier l'étape de suivi d'une commande, ou de le supprimer. Des règles dans la BDD protègent contre des erreurs de manipulation, par exemple supprimer une commande en attente de paiement.

![capture ecran commandes](https://github.com/user-attachments/assets/19608c43-49b3-46ab-981b-a0fc214160f1)
### Onglet 6 : Commandes DVD

Cet onglet est identique au précédent, sauf les détails qui sont affichés du document après recherche.

### Onglet 7 : Abonnements Revues

Très similaire aux deux derniers, cet onglet permet de gérer les abonnements des revues. Un abonnement est un peut différent à une commande et surtout on ne peut pas modifier un abonnement, seulement ajouter ou supprimer.

## La base de données

La base de données 'mediatek86 ' est au format MySQL.<br>
Voici sa structure :<br>

![mcd_finale](https://github.com/user-attachments/assets/63474fc5-7817-4f0f-a536-ac6571f9dad9)

<br>On distingue les documents "génériques" (ce sont les entités Document, Revue, Livres-DVD, Livre et DVD) des documents "physiques" qui sont les exemplaires de livres ou de DVD, ou bien les numéros d’une revue ou d’un journal.<br>
Chaque exemplaire est numéroté à l’intérieur du document correspondant, et a donc un identifiant relatif. Cet identifiant est réel : ce n'est pas un numéro automatique. <br>
Un exemplaire est caractérisé par :<br>
. un état d’usure, les différents états étant mémorisés dans la table Etat ;<br>
. sa date d’achat ou de parution dans le cas d’une revue ;<br>
. un lien vers le fichier contenant sa photo de couverture de l'exemplaire, renseigné uniquement pour les exemplaires des revues, donc les parutions (chemin complet) ;
<br>
Un document a un titre (titre de livre, titre de DVD ou titre de la revue), concerne une catégorie de public, possède un genre et est entreposé dans un rayon défini. Les genres, les catégories de public et les rayons sont gérés dans la base de données. Un document possède aussi une image dont le chemin complet est mémorisé. Même les revues peuvent avoir une image générique, en plus des photos liées à chaque exemplaire (parution).<br>
Une revue est un document, d’où le lien de spécialisation entre les 2 entités. Une revue est donc identifiée par son numéro de document. Elle a une périodicité (quotidien, hebdomadaire, etc.) et un délai de mise à disposition (temps pendant lequel chaque exemplaire est laissé en consultation). Chaque parution (exemplaire) d'une revue n'est disponible qu'en un seul "exemplaire".<br>
Un livre a aussi pour identifiant son numéro de document, possède un code ISBN, un auteur et peut faire partie d’une collection. Les auteurs et les collections ne sont pas gérés dans des tables séparées (ce sont de simples champs textes dans la table Livre).<br>
De même, un DVD est aussi identifié par son numéro de document, et possède un synopsis, un réalisateur et une durée. Les réalisateurs ne sont pas gérés dans une table séparée (c’est un simple champ texte dans la table DVD).
Enfin, 3 tables permettent de mémoriser les données concernant les commandes de livres ou DVD et les abonnements. Une commande est effectuée à une date pour un certain montant. Un abonnement est une commande qui a pour propriété complémentaire la date de fin de l’abonnement : il concerne une revue.  Une commande de livre ou DVD a comme caractéristique le nombre d’exemplaires commandé et concerne donc un livre ou un DVD.<br>
<br>
La base de données est remplie de quelques exemples pour pouvoir tester son application. Dans les champs image (de Document) et photo (de Exemplaire) doit normalement se trouver le chemin complet vers l'image correspondante.

## L'API REST

L'accès à la BDD se fait à travers une API REST protégée par une authentification basique.<br>
Le code de l'API se trouve ici :<br>
https://github.com/josephchaouistannard/BTS_atelier_mediatekdocuments_api<br>
avec toutes les explications pour l'utiliser (dans le readme).

## Installation de l'application

Ce mode opératoire permet d'installer l'application pour pouvoir travailler dessus.<br>
- Installer Visual Studio 2022 entreprise et les extension Specflow et newtonsoft.json (pour ce dernier, voir l'article "Accéder à une API REST à partir d'une application C#" dans le wiki de ce dépôt : consulter juste le début pour la configuration, car la suite permet de comprendre le code existant).<br>
- Télécharger le code et le dézipper puis renommer le dossier en "mediatekdocuments".<br>
- Récupérer et installer l'API REST nécessaire (https://github.com/josephchaouistannard/BTS_atelier_mediatekdocuments_api) ainsi que la base de données (les explications sont données dans le readme correspondant).
