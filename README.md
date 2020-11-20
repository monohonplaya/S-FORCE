# S-FORCE GAME

I will probably never finish this so I'll leave this here with some notes.

- To build, download Godot Mono 3.2.3 and import the .csproj file or scan the directory, and run. It may break with 3.2.4 and almost definitely will with 4.0 when those are out. It'd be nice to fix for 4.0 since it should improve graphics a lot.
- The bearies all have pick up and throw animations (you can make a drop anim by reversing the pick up animation) and I was originally planning to have stuff you could pick up and place/throw for puzzles/combat and that should be relatively easy to implement. The sitwiggle animation also ended up unused in gameplay.
- If you try to view Spikedog's animation player or make any changes there, the mesh will get messed up, but just go back to the mesh (spikedog > Armature004 > Skeleton > spikedog) and reset the rotation and scale to fix it.
- The 3D models are available in the glb directory as glb files. I could only get them to work by importing from within blender, not by just opening the file with it.
- The really bad save file format is because I worked on the project in 3.2.2 and I couldn't get Newsoft.Json to work in it, but it is supposedly fixed in 3.2.3.
- The code is generally messy idk glhf

# Game Download
https://monohonplaya.itch.io/s-force
