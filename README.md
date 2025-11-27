# IMG-420-Final

When cloning use this command:
	git clone --recurse-submodules https://github.com/DorianOklahoma/IMG-420-Final
If already cloned without the submodules flag, do this command in the repo:
	git submodule update --init --recursive

When the repo is cloned, build the project by using this command in the root of the repo:
	On mac:
		scons platform=macos
	On windows:
		scons platform=windows
	On linux/others:
		scons platform=linux

When the build is finished, import the godot project into godot. The project.godot file is located in final/.

To modify or add to the extension, the extension files are located in src/.