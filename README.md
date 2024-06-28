# C-Sharp-OOP-Game
## University Project, First Year Semester 2

Demonstration video: https://www.youtube.com/watch?v=VbjjJcnQecA&ab_channel=DanhNg%C3%B4

Overview: This is a simple game I created with C# and SplashKit library, using Object-Oriented Programming design principles.

- The game's called ”Half Super Mario”, where there’s a hero (player) fighting with enemies, bugs and rescuing a princess. When the game is started, it will prompt the player to enter the player’s name, this is for storing the player’s record when the game is over.
![image](https://github.com/DanNgo4/C-Sharp-OOP-Game/assets/127183060/0b2f1356-6c1c-46ae-88da-4e0b9233d28d)

 ![image](https://github.com/DanNgo4/C-Sharp-OOP-Game/assets/127183060/eeb63c73-2a8b-4e6b-b4fe-2ee478419710)
-	This is the interface of the game, a player (Hero) has some attributes such as HP, Armour, Score, can move forward, backward and jump. Hero also has a Weapon (either a Sword or a Bow), by default it is a Sword. 
-	A Bug or an Enemy is automatically spawned from the right side of the screen, moving towards the player. An Enemy has a Staff as its Weapon and can automatically spawn Fireballs to attack the player. If Hero is hit by a Bug, it loses 10 HP or 10 Armour if it has Armour, this value is 15 if the Hero is hit by a Fireball. The Fireball/Bug will disappear if it hits the Hero, the player will not earn any points from hitting a Bug that makes it disappear.
-	Hero can use its Weapon to destroy these obstacles. Sword can destroy Bugs and Fireballs with 1 hit, but it needs to hit an Enemy twice to kill it. When an Enemy dies, all Fireballs that it has launched will disappear as well. The player will earn 5 points when killed a Bug and 10 points when killed an Enemy.

![image](https://github.com/DanNgo4/C-Sharp-OOP-Game/assets/127183060/b5ba19e3-ab94-4c60-ac8a-0fd52c6dc0ce)
-	This is Hero having Bow as its Weapon, a Bow can launch Arrows to attack the opponents. An Arrow can kill a Bug, but it will be destroyed if hit by a Fireball. Unlike a Sword, an Arrow can kill an Enemy by 1 hit.

![image](https://github.com/DanNgo4/C-Sharp-OOP-Game/assets/127183060/1e536ab4-5b56-45d8-adee-02592d185a09)
-	Falling from the sky are the Spells that can support Hero, there are 3 types of Spells: ChangeWeapon, Heal and Armour. ChangeWeapon will change the Hero’s current Weapon, e.g. from Sword to Bow and vice versa. Heal will add 20 HP to the Hero’s HP if it is below 100, the maximum value of HP that Hero can have is 100. Armour will set Hero’s Armour to 100 if it’s below 100.

![image](https://github.com/DanNgo4/C-Sharp-OOP-Game/assets/127183060/7c8fde5b-5547-4222-ad3e-cb2015152a0d)
-	When the player’s Score hits 100, a Princess trapped in a Cage will appear, meaning the player’s mission now is to destroy the Cage and rescue the Princess. The Cage can only be hit by a Sword and Hero has to hit it 10 times to rescue the Princess. 

![image](https://github.com/DanNgo4/C-Sharp-OOP-Game/assets/127183060/670c82db-381b-4a73-81bc-24475133f8a2)
-	When the Princess is rescued, it will follow the Hero to enter the Final Round of the game, which is player has to achieve 300 points to win the game.
-	A Princess also has a Weapon, which is a Wand that can randomly launch Spells for the Hero.
 
 ![image](https://github.com/DanNgo4/C-Sharp-OOP-Game/assets/127183060/2ff53953-e20f-4007-95eb-8ed5f8195544)
 ![image](https://github.com/DanNgo4/C-Sharp-OOP-Game/assets/127183060/23b93488-14c0-432b-bebd-c05e73ea36e9)
-	When the player wins or loses (HP <= 0), the game will pause, prompt a message as above, along with the 5 most recent scores of the game including the current one, containing the players’ name and score. These records are stored in a text file by the program:
 ![image](https://github.com/DanNgo4/C-Sharp-OOP-Game/assets/127183060/f89a0819-51f0-43f9-b292-6ca846817ba7)

-	The final message from the program when the game is over:
 ![image](https://github.com/DanNgo4/C-Sharp-OOP-Game/assets/127183060/4f080e17-064a-49a8-b494-a00bbbac02de)

- In summary, this game was designed to implement the 4 basic principles of Object Oriented Programming paradigm, with heavy concentrations on Inheritance and Polymorphism. The game's UML class diagram:
![UML Class Diagram](https://github.com/DanNgo4/C-Sharp-OOP-Game/assets/127183060/59cff340-00c7-42ef-ab05-2e21331a5c88)


