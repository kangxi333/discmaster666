hellooo here are thoughts and stuff, probably worth reading so that u get what is running through my mind then we can DISCUSS ! -suop

really sorry i didnt get enough done today i'll have ball shooting and stuff done by 23rd evening 

~~ 23-04-2025 ~~
take everything i say with a solid handful of salt but here is what im thinkin about atm (i will implement as much as i can asap but it's 3am rn and i gotta sleep so tomorrow)

1. you may be asking: "why are there so many cameras and render textures and jesus isn't that a fucking mess?"
	- so the cameras and whatnot are as follows:
		- there is a camera rendering to a render texture for the actual peggle bit of things, that was because i didn't know if we wanted the game to be in a 3d environment or just 2d drawn onto the screen. it should be trivial to remove the render texture step and just render it with the world camera if we think that's a better direction artistically. basically, if we want 3d, GOOD! i set this up! if not? SCRAP AND SIMPLIFY ! 

		- i sent a diagram in the discord group but what is basically happening right now is:
			1. there is a "player camera" (PlayerCam) rendering the 3d world to the PCamRT render texture

			2. this is then drawn on, along with other UI elements (text, borders, etc), to the FinalRenderTex (see PlayerHUDRoot -> GUICamera). the FinalRenderTex is currently set to a fixed resolution of 640x480, so it will always render at that res

			3. The FinalRenderTex is finally drawn by the FinalRenderCamera, indiscriminately stretched to whatever size the game window is at (currently with bilinear sampling). this means the game is always run at a set resolution of 640x480, but is fuzzily stretched to the user's screen.
				- this is an artistic choice and i think it'll work with the style of assets and game we're making, but if you guys hate it it won't be hard to rip this stuff out and just render it normally!

2. how will levels work? ideas&alternatives welcome, but here's what's i'm thinking rn: 
	- BoardRoot prefab with a BoardManager class that handles the pegs in each level. 

	- each level will have its pegs referenced in an array through an initial search through the Pegboard's children on level start.

	- boardroot tracks score and stuff, when pegs are tagged/hit they tell the boardroot who calculates score (which is cumulative per each ball shot, so it has to track that per each "round")

	- we should get score saving setup super simple asap so it isnt a pain later

	- we should have a chat about style and r we doing 3d/2d/hybrid(&if so how)

3. do we want a super short story? could even just be 80 words in a text box each level select (think cruelty squad level select screen). or not, either or

4. shooting mechanic should just be getting the angle from where the ball spawns to where your mouse is and shooting with a little force, simple

5. what we thinking aesthetic wise? i have put some things in a "ref" folder for things i think could be interesting, just as a starting off point! also the stuff in the ref folder is NOT from discmaster just fyi

6. those are my thoughts for now please @ me and give me ur opinions ur LOVE ur HATE and all the rest also sorry again for not getting enough done yesterday i shall make up for it today after i sleep

~~ ~ ~~