LIFPROJET SA1 - Groupe: Gamologie

Doggy Wars : tactical RPG

p1911202 : Erkan Aksoy

p1925996 : Finaritra Randriamitandrina

p1801885 : Badreddine Ouar

=================== OBJECTIF ===================

L’objectif de ce projet est de proposer une modélisation distribuée d’un jeu stratégique.
Nous prendrons comme exemple le tactical RPG. Nous considérerons des personnages qui évoluent sur un environnement dynamique.
Les personnages appartiennent à plusieurs groupes.Les personnages peuvent se déplacer en même temps selon des règles préétablies. 
Nous implémenterons une méthode de résolution distribuée avec des entités autonomes dotée de comportements intelligents.


=================== INSTALLATION ===================

Pour éxécuter le projet, le logiciel Unity doit être installé.
Lien: https://unity.com/download

=================== DEMARRAGE ===================

Lorsque Unity est téléchargé, il faut cliquer sur "Open" et ouvrir ce dossier "Gamologie".
Lors de l'ouverture du projet si la scène n'est pas déjà chargé, il faut aller l'ouvrir dans le dossier /Scenes/SampleScene, la scène est nommée "Doggywars".
Pour lancer la simulation, cliquez sur le bouton "play" en haut.
Vous pourrez ensuite déplacer la caméra avec les flèches directionnelles du clavier.

=================== HIERARCHIE DES OBJETS ===================

Lorsqu'on ouvre Unity, à gauche de l'écran se trouve la hiérarchie des objets représentés (dans la rubrique "Hierarchy"). 

Voici les objets importants:

- Level: objet vide contenant environnement

     Plane: le plateau/sol 

     Trees: les arbres

Main camera: la camera

Canvas: objet vide contenant affichage textuel

     Coins1Text : affichage du nb de pièces récupérées par Team 1

     Coins2Text : par _ _ _ Team 2

Teams: objet vide contenant les équipes

     Team1

          des Dog1: des personnages du soldat chien

               Canvas Life : objet vide

                    Health Bar : La barre de vie verte au-dessus  de leur tête

     Team2

          _ _ _  pareil

Tower1

     Canvas Life :La barre de vie verte au-dessus de leur tête

Tower2

     Canvas Life : La barre de vie verte au-dessus de leur tête

Items: objet vide contenant les objets collectables

     Bonus Coins : objet vide contenant des zones de pièces

          Circle Coins: cercle des Pièces

     Bonus Healths : objet vide contenant des zones de pilules vertes

          Health line: ligne de pilules vertes

     Malus Healths : objet vide contenant des zones de poisons

          Poison: groupe de poison


=================== SCRIPTS ===================

Pour les scripts, on peut retrouver l'ensemble du code dans le dossier "Gamologie\Assets\Scenes\Scripts". 
Et ce sont tous les fichiers avec extensions .cs .
Chaque classe (ou script) est une composante d'un objet (ex: le script TowerLife est un composant de l'objet Tower).
Vous pouvez également regarder le diagramme de classe générée par Visual Studio dans le fichier "ClassDiagram_LIFPROJET_Gamologie.png".

Voici l'explication de l'ensemble des fonctions: 

Les classes utilisées par le canvas de ma barre de vie:
     BillboardHealth : LateUpdate() :fonction permettant de toujours positionner l'objet face à la caméra.

     HealthBar : SetMaxHealth(int health) : Fonction permettant de modifier la valeur maximal de la barre de vie et la valeur initiale 

     HealthBar : SetHealth(int health) : Fonction permettant de modifier la valeur courante de la barre de vie 


Les classes utilisées par les items collectables:
     ItemCollector : OnTriggerEnter(Collider other) :Fonction permettant de detruire l'item s'il est touché et de mettre à jour les valeurs sur l'affichage textuel (l'objet other est l'item touché)

     Rotate : Update() : Fonction permettant la rotation de l'objet

La classe TowerLife utilisée par l'objet tour:
     TowerLife : Start() : Fonction qui initialise la vie maximal de HealthBar

     TowerLife :Update() : Fonction qui met à jour l' Health Bar

     TowerLife :TakeDamage(int damage) : diminue la vie de la tour

     TowerLife : DestroyEnemy() : fonction qui détruit de la tour

La classe utilisée par la caméra:

     CameraMove :Update() : Fonction qui gère le déplacement de la caméra selon les touches claviers

Les classes utilisées par les personnages:

     Character_AI : importData() : importe les données du personnages

     Character_AI : leaderOrNot() : test si le personnage est un leader ou non

     Character_AI : setId(int id) : modifie l'id du personnage 

     Character_AI : getAlly(int identity) : récupère un allié grâce à son id 

     Character_AI : GetEnemy() : Récupère un ennemi proche

     Character_AI : Move(Vector3 position) : Fonction permettant de deplacer le personnage vers une position en animant ses mouvements 

     Character_AI : GoToHealth() : Gestion de la priorité de la santé: 
          *Se dirige vers une pilule verte connue via la messagerie si le personnage possède peu de vie. 
          *Rajoute la position d'une pilule rouge connue via la messagerie dans l'avoid zone

     Character_AI : AskForHelp() : Fonction qui demande de l'aide aux alliés via la messagerie

     Character_AI : GiveHelp() : Fonction qui fait deplacer le personnage vers la l'allié qui a demandé de l'aide via la messagerie

     Character_AI : AddMember(GameObject any) : Fonction qui rajoute un membre any dans le groupe

     Character_AI : RemoveMember(GameObject any) : Fonction qui enlève un membre any du groupe

     Character_AI : CreateGroup() : Fonction qui crée un groupe

     Character_AI : FollowTeamTarget() : Fonction permettant à un groupe de suivre une même cible

     Character_AI : OnTriggerEnter(Collider other) : Fonction permettant de gerer le comportement lorsque le personnage touche un item
          *il envoie un message si c'est une pillule avec le sujet adapté.
          *On augmente le nombre de pièce de l'équipe si c'est une pièce

     Character_AI : BuyHealthTeam() : Fonction permettant le regain de santé de toute l'équipe s'il atteint un certain nombre de pièce

     Character_AI : GoToTower() : Fonction permettant au personnage de se dirriger vers la tour 

     Character_AI : AttackTower(GameObject tower) : Fonction permettant au personnage d'attaquer la tour

     Character_AI : Chase() : Fonction permettant de suivre une cible 

     Character_AI : Attack() : Fonction permettant au personnage d'attaquer 

     Character_AI : ResetAttack() : Fonction permettant au personnage d' annuler une attaque

     Character_AI : TakeDamage(int damage) : Fonction permettant au personnage de perdre de la vie 

     Character_AI : DestroyEnemy() : Fonction permettant au personnage de mourir

     LeadManagement : enum Subject : Enumération des sujets de message possible

     LeadManagement : getEnnemies() : Fonction qui récupère la liste des ennemies

     LeadManagement : getAllies() : Fonction qui récupère la liste des alliés

     LeadManagement : getLeaders() :Fonction qui récupère la liste des leaders

     LeadManagement : giveId() : Fonction qui répartie les Id aux personnage

     LeadManagement : defineLeader(GameObject any) : Fonction qui définit un leader

     LeadManagement : setLeader() : Procédure qui gère l'affectation des leaders

     LeadManagement : leadersHaveGroup() : Fonction booléenne qui vérifie que tous les leaders ont un groupe

     LeadManagement : globalLetterBoxReloading() Fonction qui met à jour la boite au lettre

     Group : Group() :constructeur de groupe

     Group : Group(GameObject obj) : constructeur de groupe avec paramètre

     Group : GetMembres() : Fonction qui récupère la liste des membres du groupe

     Group : HasGroup() : Fonction qui teste si le groupe est vide ou non

     Group : CheckGroup() : Fonction qui met à jour si le groupe a des membres ou non

     Group : IsInGroup(GameObject any) :Fonction qui teste si le personnage any fait partie du groupe ou non

     Group : AddToGroup(GameObject any) : Fonction qui ajoute le personnage any au groupe

     Group : RemoveFromGroup(GameObject any) : Fonction qui enlève le personnage any du groupe
