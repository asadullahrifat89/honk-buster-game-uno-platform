# honk-buster-game-uno-platform
An isometric shooter bomber game built in C# and XAML with Uno Platform WebAssembly.
Play here: https://asadullahrifat89.github.io/honk-buster-game-uno-platform/

# 2D Game engine
A simple game engine built with C#. XAML, HTML, CSS, and JavaScript that runs on Uno Platform & WebAssembly.
This repository contains an arcade honk busting game.

Play here: https://asadullahrifat89.github.io/honk-buster-game-uno-platform/

## Game Features

- Isometric movement control with keyboard and Joystick.
- Game loop with scene, constructs, generators, and game on screen game controller.
- Fully reposive view port.
- Cross platform compatibilty supporting desktop and mobile OS: Windows, Mac, iOS, Android.
- Post procesing effects such as blur, grayscale, pop, shrink, explode, tilt, vibrate.
- Day and night time scenes.
- Scoring mechanism.
- Health bar mechanism.
- Game pause and resume capabiltiy.
- Level and difficulty scaling with score.
- Contextual background music switch and sound effects.
- Game object collision detection, intersection, gravity, and flight.
- Drop shadow, softshadow effects.
- Depth of field, godray lighting and cast shadows.

## Game Objects
- Two unique players.
- Two unique honk bombs with sound effects.
- Four unique bosses with attack patterns and ammunition.
- Groud and air enemies.
- Road side walks, trees, and billboards.

## Game Engine Parts

The game engine has four main classes.

### Construct

This is used to define a game element. This can be added to a Scene. A construct can contain two basic functions. Animate and Recycle. These functions can be set in code.

### Generator

This is used to define a generation sequence of a particular construct. This can be added to a Scene. This accepts two functions. Startup and Generate. The Startup function is executed as soon as a generator is added to a scene.

### Scene

This is used to host and render game elements in view. It will execute each construct's Animate and Recycle function that has been added to it. Additionally, it will also execute the Generate function defined in the code.

### Controller

This is used to intercept user input from the keyboard and touch-screen. It exposes properties that can be used to detect player movement directions.

### Screenshots

![honk-trooper-2](https://user-images.githubusercontent.com/25480176/229379005-7e8b35ec-7088-4dc7-baa1-cb33304d23cf.png)

